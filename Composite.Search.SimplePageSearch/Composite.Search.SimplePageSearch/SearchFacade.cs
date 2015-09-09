using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Composite.Core;
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
            return SearchPages(keywords, currentSiteOnly).Concat(SearchInData(keywords, currentSiteOnly)).ToList();
        }

        private static Expression<Func<T, bool>> PredicateOr<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            if(expr1 == null) return expr2;

            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
            return Expression.Lambda<Func<T, bool>>(Expression.Or(expr1.Body, invokedExpr), expr1.Parameters);
        }

        private static Expression<Func<T, bool>> PredicateAnd<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            if (expr1 == null) return expr2;

            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
            return Expression.Lambda<Func<T, bool>>(Expression.And(expr1.Body, invokedExpr), expr1.Parameters);
        }


        public static IEnumerable<SearchResultEntry> SearchPages(string[] keywords, bool currentSiteOnly)
        {
            Verify.That(keywords.Any(), "No keywords specified");
            if (keywords.Length == 0)
            {
                return Enumerable.Empty<SearchResultEntry>();
            } 

            using (var conn = new DataConnection())
            {
                // Build dynamic where clause
                Expression<Func<IPage, bool>> predicate1 = null;
                Expression<Func<IPagePlaceholderContent, bool>> predicate2 = null;

                foreach (string keyword in keywords)
                {
                    string temp = keyword;
                    predicate1 = PredicateOr(predicate1, p => p.Title.ToLower().Contains(temp)
                                                        || p.Description.ToLower().Contains(temp));

                    //predicate2 = PredicateOr(predicate2, p => p.Content.IndexOf(temp,  StringComparison.OrdinalIgnoreCase) >= 0);
                    predicate2 = PredicateOr(predicate2, p => p.Content.ToLower().Contains(temp));
                }

                var pages = conn.Get<IPage>();//DataFacade.GetData<IPage>(); 
                var placeholders = conn.Get<IPagePlaceholderContent>();

                var pagesIdsByPredicate1 = pages.Where(predicate1).Select(p => p.Id).ToList();
                var pagesIdsByPredicate2 = placeholders.Where(predicate2).Select(p => p.PageId).ToList();

                // Union the different resultsets (and thereby eliminate duplicates)
                var combinedSearchResult = pagesIdsByPredicate1.Union(pagesIdsByPredicate2).ToList();

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

                string url = InternalUrls.TryBuildInternalUrl(data.ToDataReference());
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
