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

        private readonly IEnumerable<ISearchDocumentSourceProvider> _dsProviders;

        public LuceneSearchProvider(ISearchIndex searchIndex, AnalyzerFactory analyzerFactory, IEnumerable<ISearchDocumentSourceProvider> providers)
        {
            _searchIndex = (LuceneSearchIndex) searchIndex;
            _analyzerFactory = analyzerFactory;
            _dsProviders = providers;
        }

        private IEnumerable<DocumentField> AllDocumentFields =>
            _dsProviders.SelectMany(sp => sp.GetDocumentSources()).SelectMany(ds => ds.CustomFields);

        public Task<SearchResult> SearchAsync(SearchQuery searchQuery)
        {
            if (!_searchIndex.IsInitialized)
            {
                return Task.FromResult<SearchResult>(null);
            }

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
            if (fieldName == DocumentFieldNames.Source)
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
                return DocumentFieldNames.Source;
            }

            Verify.That(facetFieldName.StartsWith(Constants.FacetFieldPrefix), $"Facet fields should start with prefix '{Constants.FacetFieldPrefix}'");

            return facetFieldName.Substring(Constants.FacetFieldPrefix.Length);
        }

        private SearchResult Search(SearchQuery searchQuery, Directory directory)
        {
            if (!TryParseTextQuery(searchQuery.Query, searchQuery.CultureInfo, out Query query))
            {
                return null;
            }

            var facetFields = searchQuery.Facets?.ToArray() ??
                              Array.Empty<KeyValuePair<string, DocumentFieldFacet>>();

            var fieldsForFacetHandlers = new List<KeyValuePair<string, DocumentFieldFacet>>(facetFields);

            var selectedValues = searchQuery.Selection ?? Enumerable.Empty<SearchQuerySelection>();

            // Adding facet handlers for the filtering by fields
            var allFields = new Lazy<IEnumerable<DocumentField>>(() => AllDocumentFields);
            foreach (var selection in selectedValues.Where(s => !facetFields.Any(f => f.Key == s.FieldName)))
            {
                var field = allFields.Value.FirstOrDefault(f => f.Name == selection.FieldName);

                if (field == null)
                {
                    throw new ArgumentException($"Field with name '{selection.FieldName}' is not defined in the document sources");
                }

                if (field.Facet == null)
                {
                    throw new ArgumentException($"Search query contains a selection for field '{selection.FieldName}'"
                     + ", which does not have facetted search enabled.");
                }

                fieldsForFacetHandlers.Add(new KeyValuePair<string, DocumentFieldFacet>(selection.FieldName, field.Facet));
            }

            var facetHandlers = from facetField in fieldsForFacetHandlers
                                let name = ToFacetFieldName(facetField.Key)
                                let info = facetField.Value
                                select GetFacetHandler(name, info);

            var sortOptions = searchQuery.SortOptions?.ToArray() ?? Array.Empty<SearchQuerySortOption>();
            facetHandlers = facetHandlers.Concat(
                sortOptions.Select(so => new SimpleFacetHandler(ToPreviewFieldName(so.FieldName))));

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

                            browseRequest.AddSelection(new BrowseSelection(fieldName)
                            {
                                SelectionOperation = selection.Operation == SearchQuerySelectionOperation.Or
                                    ? BrowseSelection.ValueOperation.ValueOperationOr
                                    : BrowseSelection.ValueOperation.ValueOperationAnd,
                                Values = selection.Values ?? Array.Empty<string>(),
                                NotValues = selection.NotValues ?? Array.Empty<string>()
                            });
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
                                : FacetSpec.FacetSortSpec.OrderValueAsc,
                            ExpandSelection = true
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

                            if (searchQuery.HighlightSettings.Enabled)
                            {
                                buildHighlights = (line, doc) => AddHighlights(query, searchQuery.HighlightSettings, searchQuery.CultureInfo, doc, line);
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

        private void AddHighlights(Query query, SearchQueryHighlightSettings settings, CultureInfo culture, Document doc, SearchResultItem item)
        {
            var highlighter = new Highlighter(
                new SimpleHTMLFormatter(HighlightWordOpenTag, HighlightWordCloseTag),
                new HTMLEncoder(),
                new QueryScorer(query))
            {
                MaxDocCharsToAnalyze = settings.MaxAnalyzedChars,
                TextFragmenter = new SimpleFragmenter(settings.FragmentSize)
            };

            var labelText = doc.GetField(Constants.FieldNames.label)?.StringValue;
            var fullText = doc.GetField(Constants.FieldNames.fulltext)?.StringValue;

            using (var analyzer = _analyzerFactory.GetAnalyzer(culture))
            {
                string[] labelHighlights = Array.Empty<string>();

                if (!string.IsNullOrEmpty(labelText))
                {
                    labelHighlights = highlighter.GetBestFragments(analyzer, Constants.FieldNames.label, labelText, 1);
                    labelHighlights = PrettifyHighlights(labelHighlights);
                }

                item.LabelHtmlHighlight = labelHighlights.Length > 0
                    ? labelHighlights[0] 
                    : HttpUtility. HtmlEncode(item.Document.Label);

                // Text highlights
                string[] textHighlights = null;
                if (!string.IsNullOrEmpty(fullText))
                {
                    textHighlights = highlighter.GetBestFragments(analyzer, Constants.FieldNames.fulltext, fullText, settings.FragmentsCount);
                    textHighlights = PrettifyHighlights(textHighlights);
                }

                item.FullTextHtmlHighlights = textHighlights ?? Array.Empty<string>();
            }
        }

        private static string[] PrettifyHighlights(string[] lines)
        {
            if (lines == null) return null;

            // Removing white spaces and punctuation characters form the beginning of the highlight
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                int position = 0;
                while(position < line.Length 
                    && (char.IsPunctuation(line[position]) || char.IsWhiteSpace(line[position])))
                {
                    if (line[position] == '&')
                    {
                        break;
                    }
                    position++;
                }

                if (position < line.Length)
                {
                    lines[i] = line.Substring(position);
                }
            }
            return lines;
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

        private bool TryParseTextQuery(string query, CultureInfo cultureInfo, out Query parsedQuery)
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

                try
                {
                    parsedQuery = mfQP.Parse(query);
                    return true;
                }
                catch (Exception ex)
                {
                    parsedQuery = null;
                    return false;
                }
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
