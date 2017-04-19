using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BoboBrowse.Net;
using BoboBrowse.Net.Facets;
using BoboBrowse.Net.Facets.Impl;
using Composite;
using Composite.Search;
using Composite.Search.Crawling;
using Composite.Core.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;

namespace Orckestra.Search.LuceneNET
{
    internal class LuceneSearchProvider : ISearchProvider
    {
        private const string HighlightWordOpenTag = "<mark>";
        private const string HighlightWordCloseTag = "</mark>";

        private readonly LuceneSearchIndex _searchIndex;
        private readonly AnalyzerFactory _analyzerFactory;

        public LuceneSearchProvider(ISearchIndex searchIndex, AnalyzerFactory analyzerFactory)
        {
            _searchIndex = (LuceneSearchIndex) searchIndex;
            _analyzerFactory = analyzerFactory;
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


        private string ToFacetFieldName(string fieldName)
        {
            if (fieldName == DefaultDocumentFieldNames.Source)
            {
                return Constants.FieldNames.source;
            }

            return Constants.FacetFieldPrefix + fieldName;
        }

        private string ToPreviewFieldName(string fieldName)
        {
            return Constants.PreviewFieldPrefix + fieldName;
        }

        private string FromFacetFieldName(string facetFieldName)
        {
            if (facetFieldName == Constants.FieldNames.source)
            {
                return DefaultDocumentFieldNames.Source;
            }

            Verify.That(facetFieldName.StartsWith(Constants.FacetFieldPrefix), $"Facet fields should start with prefix '{Constants.FacetFieldPrefix}'");

            return facetFieldName.Substring(Constants.FacetFieldPrefix.Length);
        }

        private SearchResult Search(SearchQuery searchQuery, Directory directory)
        {
            var facetFields = searchQuery.Facets?.Evaluate() ??
                              Enumerable.Empty<KeyValuePair<string, DocumentFieldFacet>>();

            var facetHandlers = from facetField in facetFields
                let name = ToFacetFieldName(facetField.Key)
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

            var sortOptions = searchQuery.SortOptions?.ToArray() ?? Array.Empty<SearchQuerySortOption>();
            facetHandlers = facetHandlers.Concat(
                sortOptions.Select(so => new SimpleFacetHandler(ToPreviewFieldName(so.FieldName))));

            var query = GetTextQuery(searchQuery.Query, searchQuery.CultureInfo);

            using (var reader = IndexReader.Open(directory, true))
            {
                using (var boboReader = BoboIndexReader.GetInstance(reader, facetHandlers))
                {
                    var browseRequest = new BrowseRequest
                    {
                        Query = query,
                        Offset = searchQuery.SearchResultOffset,
                        Count = searchQuery.MaxDocumentsNumber
                    };

                    if (searchQuery.Selection != null)
                    {
                        foreach (var selection in searchQuery.Selection)
                        {
                            string fieldName = ToFacetFieldName(selection.FieldName);
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
                        browseRequest.SetFacetSpec(ToFacetFieldName(facetField.Key), new FacetSpec
                        {
                            MinHitCount = facetField.Value.MinHitCount,
                            MaxCount = facetField.Value.Limit,
                            OrderBy = facetField.Value.FacetSorting == FacetSorting.HitCount 
                                ? FacetSpec.FacetSortSpec.OrderHitsDesc
                                : FacetSpec.FacetSortSpec.OrderValueAsc
                        });
                    }

                    browseRequest.Sort = sortOptions
                        .Select(sortOption => new SortField(
                            ToPreviewFieldName(sortOption.FieldName),
                            ToFieldTypeId(sortOption.SortTermsAs),
                            sortOption.ReverseOrder))
                        .ToArray();

                    // perform browse
                    using (IBrowsable browser = new BoboBrowser(boboReader))
                    {
                        using (var browseResult = browser.Browse(browseRequest))
                        {
                            Action<SearchResultItem, Document> buildHighlights = null;

                            if (searchQuery.IncludeHighlights)
                            {
                                buildHighlights = (line, doc) => AddHighlights(query, searchQuery.CultureInfo, doc, line);
                            }

                            return new SearchResult
                            {
                                Items = ToSearchDocuments(reader, browseResult.Hits, buildHighlights),
                                Facets = GetFacets(browseResult),
                                TotalHits = browseResult.NumHits
                            };
                        }
                    }
                }
            }
        }

        private void AddHighlights(Query query, CultureInfo culture, Document doc, SearchResultItem item)
        {
            var highlighter = new Highlighter(
                new SimpleHTMLFormatter(HighlightWordOpenTag, HighlightWordCloseTag),
                new HTMLEncoder(),
                new QueryScorer(query));

            var labelText = doc.GetField(Constants.FieldNames.label)?.StringValue;
            var fullText = doc.GetField(Constants.FieldNames.fulltext)?.StringValue;

            using (var analyzer = _analyzerFactory.GetAnalyzer(culture))
            {
                string[] labelHighlights = Array.Empty<string>();

                if (!string.IsNullOrEmpty(labelText))
                {
                    labelHighlights = highlighter.GetBestFragments(analyzer, Constants.FieldNames.label, labelText, 1);
                }

                item.LabelHtmlHighlight = labelHighlights.Length > 0
                    ? labelHighlights[0] 
                    : HttpUtility. HtmlEncode(item.Document.Label);

                // Text highlights
                string[] textHighlights = null;
                if (!string.IsNullOrEmpty(fullText))
                {
                    textHighlights = highlighter.GetBestFragments(analyzer, Constants.FieldNames.fulltext, fullText, 2);
                }

                item.FullTextHtmlHighlights = textHighlights ?? Array.Empty<string>();
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

        private Query GetTextQuery(string query, CultureInfo cultureInfo)
        {
            using (var analyzer = _analyzerFactory.GetAnalyzer(cultureInfo))
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

        private ICollection<SearchResultItem> ToSearchDocuments(
            IndexReader reader, 
            BrowseHit[] hits, 
            Action<SearchResultItem, Document> buildHighlights)
        {
            var resultDocs = new List<SearchResultItem>(hits.Length);
            var docs = hits.Select(h => reader.Document(h.DocId));

            foreach (var document in docs)
            {
                var fieldValues = new Dictionary<string, object>();
                foreach (var field in document.GetFields().Where(f => f.Name.StartsWith(Constants.PreviewFieldPrefix)))
                {
                    string fieldName = field.Name.Substring(Constants.PreviewFieldPrefix.Length);

                    fieldValues[fieldName] = field.StringValue;
                }

                string GetString(string name) => document.GetField(name)?.StringValue;

                var resultLine = new SearchResultItem
                {
                    Document = new SearchDocument(
                        GetString(Constants.FieldNames.source),
                        GetString(Constants.FieldNames.id),
                        GetString(Constants.FieldNames.label),
                        GetString(Constants.FieldNames.entityToken))
                    {
                        ElementBundleName = GetString(Constants.FieldNames.version),
                        Url = GetString(Constants.FieldNames.url),
                        FieldValues = fieldValues
                    }
                };

                buildHighlights?.Invoke(resultLine, document);

                resultDocs.Add(resultLine);
            }

            return resultDocs;
        }

        private Dictionary<string, Facet[]> GetFacets(BrowseResult browseResult)
        {
            var facets = new Dictionary<string, Facet[]>();

            foreach (var key in browseResult.FacetMap.Keys)
            {
                string facetFieldName = FromFacetFieldName(key);

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
