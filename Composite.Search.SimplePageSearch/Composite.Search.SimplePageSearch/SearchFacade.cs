using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Composite.Core.Linq;
using Composite.Core.Routing;
using Composite.Core.Types;
using Composite.Data;
using Composite.Data.ProcessControlled;
using Composite.Data.Types;

namespace Composite.Search.SimplePageSearch
{
    public class SearchFacade
    {
        const int MaximumTermCount = 10;
        const int ResultsMaxLength = 200;

        private static readonly MethodInfo String_ToLower;
        private static readonly MethodInfo String_Contains;

        static SearchFacade()
        {
            var stringMethods = typeof (string).GetMethods();
            String_ToLower = stringMethods.Single(x => x.Name == "ToLower" && !x.GetParameters().Any());
            String_Contains = stringMethods.Single(x => x.Name == "Contains" && x.GetParameters().Count() == 1);
        }

        public static ICollection<SearchResultEntry> Search(string[] keywords, bool currentSiteOnly)
        {
            // If there's only one website, no need to check whether the results belong to current site only
            if (currentSiteOnly && PageManager.GetChildrenIDs(Guid.Empty).Count < 2)
            {
                currentSiteOnly = false;
            }

            keywords = keywords.Distinct().OrderByDescending(keyword => keyword.Length).Take(MaximumTermCount).ToArray();

            return SearchPages(keywords, currentSiteOnly)
                .Concat(SearchInData(keywords, currentSiteOnly))
                .Take(ResultsMaxLength)
                .ToList();
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
                        new Expression[] { Expression.Constant(keyword) });

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
