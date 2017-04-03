using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Composite.Core.Linq;
using Composite.Core.Routing;
using Composite.Core.Types;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Data;
using Composite.Data.ProcessControlled;
using Composite.Data.Types;
using Composite.Search.Crawling;

namespace Composite.Search.SimplePageSearch
{
    public class SimpleSearchFacade
    {
        const int MaximumTermCount = 10;
        const int ResultsMaxLength = 200;

        private static readonly MethodInfo String_ToLower;
        private static readonly MethodInfo String_Contains;

        static SimpleSearchFacade()
        {
            var stringMethods = typeof (string).GetMethods();
            String_ToLower = stringMethods.Single(x => x.Name == nameof(string.ToLower) && !x.GetParameters().Any());
            String_Contains = stringMethods.Single(x => x.Name == nameof(string.Contains) && x.GetParameters().Length == 1);
        }

        public static SimpleSearchResult Search(SimpleSearchQuery query)
        {
            Verify.ArgumentNotNull(query, nameof(query));

            // If there's only one website, no need to check whether the results belong to current site only
            if (query.CurrentSiteOnly && NotMoreThanOneSitePresent(query.Culture))
            {
                query.CurrentSiteOnly = false;
            }

            // Current search api does not support searching for a given website only
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

        private static SimpleSearchResult SearchUsingSearchProvider(SimpleSearchQuery query)
        {
            string text = string.Join(" ", query.Keywords);
            var searchQuery = new SearchQuery(text, query.Culture)
            {
                MaxDocumentsNumber = query.PageSize,
                SearchResultOffset = query.PageSize * query.PageNumber
            };

            searchQuery.ShowOnlyDocumentsWithUrls();

            if (query.CurrentSiteOnly)
            {
                searchQuery.FilterByAncestors(GetRootPageEntityToken());
            }

            var result = SearchFacade.SearchProvider.SearchAsync(searchQuery).Result;
            return new SimpleSearchResult
            {
                Entries = result.Documents.Select(ToSearchResultEntry).ToList(),
                ResultsFound = result.TotalHits
            };
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

        private static SearchResultEntry ToSearchResultEntry(SearchDocument doc)
        {
            object desc;

            doc.FieldValues.TryGetValue(DefaultDocumentFieldNames.Description, out desc);

            return new SearchResultEntry
            {
                Title = doc.Label,
                Description = desc as string,
                Url = doc.Url
            };
        }

        internal static SimpleSearchResult SimpleSearch(SimpleSearchQuery query)
        {
            var keywords = query.Keywords;

            keywords = keywords.Distinct().OrderByDescending(keyword => keyword.Length).Take(MaximumTermCount).ToArray();

            var allResults =
                 SearchPages(keywords, query.CurrentSiteOnly)
                .Concat(SearchInData(keywords, query.CurrentSiteOnly))
                .Take(ResultsMaxLength).ToList();

            return new SimpleSearchResult
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
                        Url = PageUrls.BuildUrl(page, UrlKind.Internal),
                        Title = page.Title,
                        Description = page.Description
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
    }
}
