using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using Composite;
using Composite.C1Console.Security;
using Composite.Core.Linq;
using Composite.Core.ResourceSystem;
using Composite.Core.Routing;
using Composite.Core.Types;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Data.ProcessControlled;
using Composite.Data.Types;
using Composite.Search;
using Composite.Search.Crawling;

namespace Orckestra.Search.WebsiteSearch
{
    public class WebsiteSearchFacade
    {
        public static string PageTypeIdFieldName = DocumentFieldNames.GetFieldName(typeof(IPage), nameof(IPage.PageTypeId));
        
        const int MaximumTermCount = 10;
        const int ResultsMaxLength = 200;

        

        private static readonly MethodInfo String_ToLower;
        private static readonly MethodInfo String_Contains;

        static WebsiteSearchFacade()
        {
            var stringMethods = typeof (string).GetMethods();
            String_ToLower = stringMethods.Single(x => x.Name == nameof(string.ToLower) && !x.GetParameters().Any());
            String_Contains = stringMethods.Single(x => x.Name == nameof(string.Contains) && x.GetParameters().Length == 1);
        }

        public static WebsiteSearchResult Search(WebsiteSearchQuery query)
        {
            Verify.ArgumentNotNull(query, nameof(query));

            // If there's only one website, no need to check whether the results belong to current site only
            if (query.CurrentSiteOnly && NotMoreThanOneSitePresent(query.Culture))
            {
                query.CurrentSiteOnly = false;
            }

            if (SearchFacade.SearchEnabled)
            {
                return SearchUsingSearchProvider(query);
            }

            return SimpleSearch(query);
        }

        private static bool NotMoreThanOneSitePresent(CultureInfo queryCulture)
        {
            using (new DataConnection(queryCulture))
            {
                return PageManager.GetChildrenIDs(Guid.Empty).Count < 2;
            }
        }

        private static WebsiteSearchResult SearchUsingSearchProvider(WebsiteSearchQuery query)
        {
            string text = string.Join(" ", query.Keywords);
            var searchQuery = new SearchQuery(text, query.Culture)
            {
                MaxDocumentsNumber = query.PageSize,
                SearchResultOffset = query.PageSize * query.PageNumber,
                SortOptions = query.SortOptions,
                HighlightSettings =
                {
                    Enabled = true,
                    FragmentsCount = 1,
                    FragmentSize = 120
                }
            };

            var allFields = SearchFacade.DocumentSources.SelectMany(ds => ds.CustomFields).ToList();

            foreach (var facet in query.Facets ?? Enumerable.Empty<WebsiteSearchQueryFacet>())
            {
                // TODO: use an overload for SearchQuery.AddFieldFacet(...) once C1 6.2 is out
                searchQuery.AddFieldFacet(facet.Name);

                var field = allFields.Where(f => f.Facet != null).FirstOrDefault(f => f.Name == facet.Name);

                if (facet.Selections != null && facet.Selections.Length > 0)
                {
                    searchQuery.Selection.Add(new SearchQuerySelection
                    {
                        FieldName = facet.Name,
                        Values = facet.Selections,
                        Operation = field.Facet.FacetType == FacetType.SingleValue 
                        ? SearchQuerySelectionOperation.Or
                        : SearchQuerySelectionOperation.And
                    });
                }

            }

            searchQuery.ShowOnlyDocumentsWithUrls();

            var filterByAncestors = new List<EntityToken>();
            if (query.CurrentSiteOnly)
            {
                filterByAncestors.Add(GetRootPageEntityToken());
            }

            if (query.FilterByAncestorEntityTokens?.Any() ?? false)
            {
                filterByAncestors.AddRange(query.FilterByAncestorEntityTokens);
            }

            if (filterByAncestors.Any())
            {
                searchQuery.FilterByAncestors(filterByAncestors.ToArray());
            }

            if (query.DataTypes != null && query.DataTypes.Length > 0)
            {
                searchQuery.FilterByDataTypes(query.DataTypes);
            }

            if(query.PageTypes != null && query.PageTypes.Length > 0)
            {
                searchQuery.Selection.Add(new SearchQuerySelection
                {
                    FieldName = PageTypeIdFieldName,
                    Operation = SearchQuerySelectionOperation.Or,
                    Values = query.PageTypes
                });
            }
            
            var result = SearchFacade.SearchProvider.SearchAsync(searchQuery).Result;
            if (result == null) return new WebsiteSearchResult();

            return new WebsiteSearchResult
            {
                Entries = result.Items.Select(ToSearchResultEntry).ToList(),
                Facets = GetFacets(query, result, allFields),
                ResultsFound = result.TotalHits
            };
        }

        private static IReadOnlyCollection<SearchResultFacet> GetFacets(
            WebsiteSearchQuery query,
            SearchResult result,
            IEnumerable<DocumentField> allFields)
        {
            if (query.Facets == null || !query.Facets.Any()) return Array.Empty<SearchResultFacet>();

            var facetFields = IgnoreDuplicates(
                allFields
                    .Where(f => f.FacetedSearchEnabled && f.Label != null),
                f => f.Name).ToDictionary(f => f.Name);
            // Returning exactly the requested fasets if no documents were found.
            if (result.TotalHits == 0)
            {
                return (from facet in query.Facets
                    let facetField = facetFields[facet.Name]
                    let previewFunc = facetField.Facet.PreviewFunction ?? (value => value?.ToString())
                    select new SearchResultFacet
                    {
                        Name = facet.Name,
                        Hits = facet.Selections
                            .Select(value => new SearchResultFacetHit
                            {
                                Value = value,
                                Label = previewFunc(value),
                                HitCount = 0
                            })
                            .ToArray()
                    }).ToArray();
            }

            return (from facet in result.Facets
                    let facetField = facetFields[facet.Key]
                    let previeFunc = facetField.Facet.PreviewFunction ?? (value => value?.ToString())
                    select new SearchResultFacet
                    {
                        Name = facet.Key,
                        Hits = facet.Value
                               .Select(v => new SearchResultFacetHit
                               {
                                    Value = v.Value,
                                    Label = previeFunc(v.Value),
                                    HitCount = v.HitCount
                               })
                               .ToArray()
                    }).ToArray();
        }

        private static DataEntityToken GetRootPageEntityToken()
        {
            var currentPageId = PageRenderer.CurrentPageId;

            for(int i=0; i<100; i++)
            {
                var parentPageId = PageManager.GetParentId(currentPageId);
                if (parentPageId == Guid.Empty)
                {
                    using (new DataConnection(PublicationScope.Unpublished))
                    {
                        var rootPage = PageManager.GetPageById(currentPageId);
                        return rootPage?.GetDataEntityToken();
                    }
                }

                currentPageId = parentPageId;
            }

            throw new InvalidOperationException("A page inheritance loop detected.");
        }

        private static SearchResultEntry ToSearchResultEntry(SearchResultItem line)
        {
            object desc;

            var doc = line.Document;
            doc.FieldValues.TryGetValue(DocumentFieldNames.Description, out desc);

            return new SearchResultEntry
            {
                Title = 
                    line.LabelHtmlHighlight ?? HttpUtility.HtmlEncode(doc.Label),
                Url = doc.Url,
                Highlight = 
                    line.FullTextHtmlHighlights != null && line.FullTextHtmlHighlights.Length > 0 
                    ? string.Join("<br />", line.FullTextHtmlHighlights)
                    : HttpUtility.HtmlEncode(desc as string),
                FieldValues = doc.FieldValues
            };
        }

        internal static WebsiteSearchResult SimpleSearch(WebsiteSearchQuery query)
        {
            var keywords = query.Keywords;

            keywords = keywords.Distinct().OrderByDescending(keyword => keyword.Length).Take(MaximumTermCount).ToArray();

            var allResults =
                 SearchPages(keywords, query.CurrentSiteOnly)
                .Concat(SearchInData(keywords, query.CurrentSiteOnly))
                .Take(ResultsMaxLength).ToList();

            return new WebsiteSearchResult
            {
                Entries = allResults
                    .Skip(query.PageSize * query.PageNumber)
                    .Take(query.PageSize).ToList(),
                ResultsFound = allResults.Count
            };
        }

        public static IEnumerable<SearchResultEntry> SearchPages(string[] keywords, bool currentSiteOnly)
        {
            Verify.That(keywords.Any(), "No keywords specified");
            if (keywords.Length == 0)
            {
                return Enumerable.Empty<SearchResultEntry>();
            } 

            using (new DataConnection())
            {
                var pages = DataFacade.GetData<IPage>(false);
                var placeholders = DataFacade.GetData<IPagePlaceholderContent>(false);

                bool isInMemoryQuery = IsCachedQuery(pages) && IsCachedQuery(placeholders);

                var searchQuery = from page in pages
                                  join placeholder in placeholders on page.Id equals placeholder.PageId
                                  group new {page, placeholder} by page into groups
                                  let page = groups.Key
                                  select new { page, placeholders = groups.Select(g => g.placeholder) };

                foreach (string keyword in keywords)
                {
                    if (!isInMemoryQuery)
                    {
                        searchQuery = searchQuery.Where(pair =>
                            pair.page.Title.ToLower().Contains(keyword)
                            || pair.page.Description.ToLower().Contains(keyword)
                            || pair.placeholders.Any(p => p.Content.ToLower().Contains(keyword)));
                    }
                    else
                    {
                        // optimized for in-memory queries xml
                        searchQuery = searchQuery.Where(pair =>
                            pair.page.Title.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0
                            || pair.page.Description.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0
                            || pair.placeholders.Any(p => p.Content.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0));
                    }
                }

                var combinedSearchResult = searchQuery.Select(s => s.page.Id).ToList();

                if (currentSiteOnly)
                {
                    using (var c = new DataConnection())
                    {
                        List<Guid> pagesIdsOfCurrentSite = c.SitemapNavigator.CurrentPageNode
                                        .GetPageIds(SitemapScope.Level1AndDescendants).ToList();
                        combinedSearchResult = pagesIdsOfCurrentSite.Intersect(combinedSearchResult).ToList();
                    }
                }

                var resultHashSet = new HashSet<Guid>(combinedSearchResult);
                return pages.Evaluate()
                    .Where(p => resultHashSet.Contains(p.Id))
                    .Select(page => new SearchResultEntry
                    {
                        Title = page.Title,
                        Url = PageUrls.BuildUrl(page, UrlKind.Internal),
                        Highlight = page.Description
                    })
                    .Evaluate();
            }
        }

        private static bool IsCachedQuery(IQueryable query)
        {
            return query.GetType().FullName.StartsWith("Composite.Data.Caching");
        }

        private static ICollection<SearchResultEntry> SearchInData(string[] keywords, bool currentWebsiteOnly)
        {
            var references = new List<SearchResultEntry>();

            foreach (var type in DataFacade.GetAllInterfaces().Where(InternalUrls.DataTypeSupported))
            {
                references.AddRange(SearchInType(type, keywords, currentWebsiteOnly) ?? Enumerable.Empty<SearchResultEntry>());
            }

            return references;
        }

        private static IEnumerable<SearchResultEntry> SearchInType(Type type, string[] keywords, bool currentWebsiteOnly)
        {
            var method = StaticReflection.GetGenericMethodInfo(() => SearchInType<IData>(null, false));

            return (List<SearchResultEntry>)method.MakeGenericMethod(type).Invoke(null, new object[] { keywords, currentWebsiteOnly });
        }

        private static IEnumerable<SearchResultEntry> SearchInType<T>(string[] keywords, bool currentWebsiteOnly) where T : class, IData
        {
            var stringFields = typeof (T).GetPropertiesRecursively()
                .Where(p => p.PropertyType == typeof(string)
                            && p.ReflectedType != typeof(IPublishControlled)
                            && p.ReflectedType != typeof(ILocalizedControlled)
                            && p.ReflectedType != typeof(IPageMetaData)
                            && !p.GetCustomAttributes<ForeignKeyAttribute>().Any()).ToList();

            if (stringFields.Count == 0)
            {
                return null;
            }


            Expression searchExpr = null;
            var parameter = Expression.Parameter(typeof(T), "p");

            foreach (string keyword in keywords)
            {
                Expression keywordSearchExpr = null;

                foreach (var stringField in stringFields)
                {
                    // Building the following expression:
                    // p => (p.{stringField} != null && p.{stringField}.ToLower().Contains(keyword))

                    var propertyExpression = Expression.Property(parameter, stringField);

                    var notNullExpression = Expression.NotEqual(propertyExpression, Expression.Constant(null, typeof(string)));

                    var toLowerExpression = Expression.Call(propertyExpression, String_ToLower);
                    var containsExpression = Expression.Call(toLowerExpression, String_Contains, 
                        Expression.Constant(keyword));

                    var andExpression = Expression.AndAlso(notNullExpression, containsExpression);

                    keywordSearchExpr = keywordSearchExpr.OrElse(andExpression);
                }

                searchExpr = searchExpr.AndAlso(keywordSearchExpr);
            }

            if (searchExpr == null)
            {
                return null;
            }

            var searchPredicate = Expression.Lambda<Func<T, bool>>(searchExpr, parameter);

            HashSet<Guid> pagesIdsOfCurrentSite = null;
            if (currentWebsiteOnly)
            {
                using (var c = new DataConnection())
                {
                    pagesIdsOfCurrentSite = new HashSet<Guid>(c.SitemapNavigator.CurrentPageNode
                                .GetPageIds(SitemapScope.Level1AndDescendants));
                }
            }

            var result = new List<SearchResultEntry>();

            foreach (var data in DataFacade.GetData(searchPredicate))
            {
                if (currentWebsiteOnly 
                    && data is IPageRelatedData
                    && !pagesIdsOfCurrentSite.Contains((data as IPageRelatedData).PageId))
                {
                    continue;
                }

                var dataReference = data.ToDataReference();

                string url;

                if (currentWebsiteOnly && !(data is IPageRelatedData))
                {
                    // Getting public url data to see if it is pointing to the website
                    var pageUrlData = DataUrls.TryGetPageUrlData(dataReference);
                    if (pageUrlData == null || !pagesIdsOfCurrentSite.Contains(pageUrlData.PageId))
                    {
                        continue;
                    }

                    url = PageUrls.BuildUrl(pageUrlData);
                }
                else
                {
                    url = InternalUrls.TryBuildInternalUrl(dataReference);
                }
                
                if (url == null)
                {
                    continue;
                }

                string label = data.GetLabel();

                // TODO: support for current website only for global data types
                result.Add(new SearchResultEntry
                {
                    Url = url,
                    Title = label
                });
            }

            return result;
        }

        public static Tuple<string, string>[] GetSearchableDataTypeOptions()
        {
            var result = new List<Tuple<string, string>>();

            var dataTypes = DataFacade.GetAllInterfaces().Where(type =>
                type.GetCustomAttributes(typeof(SearchableTypeAttribute), false).Length > 0
                && InternalUrls.DataTypeSupported(type)).ToList();

            dataTypes.Add(typeof(IPage));
            dataTypes.Add(typeof(IMediaFile));

            foreach (var dataType in dataTypes)
            {
                var descriptor = DynamicTypeManager.GetDataTypeDescriptor(dataType);
                result.Add(new Tuple<string, string>(
                    dataType.FullName,
                    descriptor.Title ?? dataType.Name));
            }

            return result.OrderBy(r => r.Item2).ToArray();
        }

        public static Tuple<string, string>[] GetSearchablePageTypesOptions()
        {
            var result = new List<Tuple<string, string>>();

            var pageTypes = DataFacade.GetData<IPageType>().Where(p => p.Available);
 
            foreach (var pageType in pageTypes)
            {
               result.Add(new Tuple<string, string>(
                    pageType.Name,
                    pageType.Id.ToString()));
            }

            return result.ToArray();
        }


        public static Tuple<string, string>[] GetFacetOptions()
        {
            var toSkip = new []
            {
                DocumentFieldNames.GetFieldName(typeof(IPublishControlled),nameof(IPublishControlled.PublicationStatus)),
                DocumentFieldNames.GetFieldName(typeof(IChangeHistory), nameof(IChangeHistory.ChangedBy)),
                DocumentFieldNames.DataType
            };

            var facetFields = SearchFacade.DocumentSources
                .SelectMany(ds => ds.CustomFields)
                .Where(f => f.FacetedSearchEnabled && f.Label != null
                    && !toSkip.Contains(f.Name))
                .Select(f => new Tuple<string, string>(f.Name, GetFacetLabel(f)));

            return IgnoreDuplicates(facetFields, t => t.Item1).OrderBy(t => t.Item2).ToArray();
        }

        private static string GetFacetLabel(DocumentField field)
        {
            return StringResourceSystemFacade.ParseString(field.Label);
        }


        private static IEnumerable<T> IgnoreDuplicates<T>(IEnumerable<T> items, Func<T, string> getKey)
        {
            var keys = new HashSet<string>();
            foreach (var item in items)
            {
                string key = getKey(item);
                if (keys.Contains(key)) continue;

                keys.Add(key);
                yield return item;
            }
        }
    }
}
