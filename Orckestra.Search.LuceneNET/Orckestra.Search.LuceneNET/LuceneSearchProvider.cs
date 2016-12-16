using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoboBrowse.Net;
using BoboBrowse.Net.Facets;
using BoboBrowse.Net.Facets.Impl;
using Composite;
using Composite.C1Console.Search;
using Composite.Core.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace Orckestra.Search.LuceneNET
{
    internal class LuceneSearchProvider : ISearchProvider
    {
        private readonly LuceneSearchIndex _searchIndex;

        public LuceneSearchProvider(ISearchIndex searchIndex)
        {
            _searchIndex = (LuceneSearchIndex) searchIndex;
        }

        public Task<SearchResult> SearchAsync(SearchQuery searchQuery)
        {
            return Task.FromResult(Search(searchQuery));
        }

        private SearchResult Search(SearchQuery searchQuery)
        {
            var culture = searchQuery.CultureInfo;
            Verify.ArgumentCondition(culture != null, nameof(searchQuery), $"Property {nameof(searchQuery.CultureInfo)} is not set");

            var directory = _searchIndex.GetCollection<Directory>(culture);

            return Search(searchQuery, directory);
        }


        private SearchResult Search(SearchQuery searchQuery, Directory directory)
        {
            var facetFields = searchQuery.Facets?.Evaluate() ??
                              Enumerable.Empty<KeyValuePair<string, DocumentFieldFacet>>();

            var facetHandlers = from facetField in facetFields
                let name = Constants.FacetFieldPrefix + facetField.Key
                let info = facetField.Value
                select GetFacetHandler(name, info);

            var selectedValues = searchQuery.Selection ?? Enumerable.Empty<SearchQuerySelection>();
            foreach (var selection in selectedValues)
            {
                if (!facetFields.Any(f => f.Key == selection.FieldName))
                {
                    throw new ArgumentException($"Search query contains a selection for field '{selection.FieldName}'"
                     + $", which is not specified in the '{nameof(searchQuery)}.{nameof(SearchQuery.Facets)}' collection.", 
                     nameof(searchQuery));
                }
            }

            var query = GetTextQuery(searchQuery.Query);

            using (var reader = IndexReader.Open(directory, true))
            {
                using (var boboReader = BoboIndexReader.GetInstance(reader, facetHandlers))
                {
                    var browseRequest = new BrowseRequest
                    {
                        Query = query,
                        Offset = searchQuery.SearchResultOffest,
                        Count = searchQuery.MaxDocumentsNumber
                    };

                    if (searchQuery.Selection != null)
                    {
                        foreach (var selection in searchQuery.Selection)
                        {
                            string fieldName = Constants.FacetFieldPrefix + selection.FieldName;
                            var sel = new BrowseSelection(fieldName);
                            foreach (var value in selection.Values)
                            {
                                sel.AddValue(value);
                            }
                            sel.SelectionOperation = selection.Operation == SearchQuerySelectionOperation.Or
                                ? BrowseSelection.ValueOperation.ValueOperationOr
                                : BrowseSelection.ValueOperation.ValueOperationAnd;

                            browseRequest.AddSelection(sel);
                        }
                    }

                    // add the facet output specs
                    foreach (var facetField in facetFields)
                    {
                        browseRequest.SetFacetSpec(Constants.FacetFieldPrefix + facetField.Key, new FacetSpec
                        {
                            MinHitCount = facetField.Value.MinHitCount,
                            MaxCount = facetField.Value.Limit,
                            OrderBy = facetField.Value.FacetSorting == FacetSorting.HitCount 
                                ? FacetSpec.FacetSortSpec.OrderHitsDesc
                                : FacetSpec.FacetSortSpec.OrderValueAsc
                        });
                    }

                    var sortFields = new List<SortField>();
                    if (searchQuery.SortOptions != null)
                    {
                        foreach (var sortOption in searchQuery.SortOptions)
                        {
                            sortFields.Add(new SortField(
                                Constants.PreviewFieldPrefix + sortOption.FieldName,
                                ToFieldTypeId(sortOption.SortTermsAs), 
                                sortOption.ReverseOrder));
                        }
                    }

                    browseRequest.Sort = sortFields.ToArray();

                    // perform browse
                    using (IBrowsable browser = new BoboBrowser(boboReader))
                    {
                        using (var browseResult = browser.Browse(browseRequest))
                        {
                            return new SearchResult
                            {
                                Documents = ToSearchDocuments(reader, browseResult.Hits),
                                Facets = GetFacets(browseResult),
                                TotalHits = browseResult.NumHits
                            };
                        }
                    }
                }
            }
        }

        private int ToFieldTypeId(SortTermsAs sortTermsAs)
        {
            switch (sortTermsAs)
            {
                case SortTermsAs.String:
                    return SortField.STRING;
                case SortTermsAs.Int:
                    return SortField.INT;
                case SortTermsAs.Long:
                    return SortField.LONG;
                case SortTermsAs.Float:
                    return SortField.FLOAT;
                case SortTermsAs.Double:
                    return SortField.DOUBLE;
            }

            throw new InvalidOperationException($"Unexpected {nameof(SortTermsAs)} value: {sortTermsAs}");
        }

        private IFacetHandler GetFacetHandler(string fieldName, DocumentFieldFacet info)
        {
            switch (info.FacetType)
            {
                case FacetType.SingleValue:
                    return new SimpleFacetHandler(fieldName);
                case FacetType.MultipleValues:
                    return new MultiValueFacetHandler(fieldName, fieldName);
            }

            throw new NotImplementedException("Not supported facet type: " + info.FacetType);
        }

        private Query GetTextQuery(string query)
        {
            using (var analyzer = new StandardAnalyzer(Constants.LuceneVersion))
            {
                var searchFields = new[] { Constants.FieldNames.label, Constants.FieldNames.fulltext };
                var boosts = new Dictionary<string, float>
                {
                    {Constants.FieldNames.label, 2},
                    {Constants.FieldNames.fulltext, 1}
                };

                var mfQP = new MultiFieldQueryParser(Constants.LuceneVersion, searchFields, analyzer, boosts)
                {
                    DefaultOperator = QueryParser.Operator.AND
                };
                return mfQP.Parse(query);
            }
        }

        private ICollection<SearchDocument> ToSearchDocuments(IndexReader reader, BrowseHit[] hits)
        {
            var resultDocs = new List<SearchDocument>(hits.Length);
            var docs = hits.Select(h => reader.Document(h.DocId));

            foreach (var document in docs)
            {
                var fieldValues = new Dictionary<string, object>();
                foreach (var field in document.GetFields().Where(f => f.Name.StartsWith(Constants.PreviewFieldPrefix)))
                {
                    string fieldName = field.Name.Substring(Constants.PreviewFieldPrefix.Length);

                    fieldValues[fieldName] = field.StringValue;
                }

                Func<string, string> getString = name => document.GetField(name)?.StringValue;
                resultDocs.Add(new SearchDocument(
                    getString(Constants.FieldNames.source),
                    getString(Constants.FieldNames.id),
                    getString(Constants.FieldNames.label),
                    getString(Constants.FieldNames.entityToken))
                {
                    ElementBundleName = getString(Constants.FieldNames.version),
                    Url = getString(Constants.FieldNames.url),
                    FieldValues = fieldValues,
                });
            }

            return resultDocs;
        }

        private Dictionary<string, Facet[]> GetFacets(BrowseResult browseResult)
        {
            var facets = new Dictionary<string, Facet[]>();

            foreach (var key in browseResult.FacetMap.Keys)
            {
                if (!key.StartsWith(Constants.FacetFieldPrefix)) continue;

                string facetFieldName = key.Substring(Constants.FacetFieldPrefix.Length);

                var facetData = browseResult.FacetMap[key];

                facets.Add(facetFieldName,
                    facetData.GetFacets()
                        .Select(f => new Facet
                        {
                            Value = f.Value,
                            HitCount = f.FacetValueHitCount
                        }).ToArray());
            }

            return facets;
        }

    }
}
