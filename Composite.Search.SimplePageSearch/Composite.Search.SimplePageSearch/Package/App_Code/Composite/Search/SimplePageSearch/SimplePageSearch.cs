using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Search.SimplePageSearch
{
	/// <summary>
	/// Type Composite.Search.SimplePageSearch.Function in adding C# function form
	/// </summary>
	public class Function
	{
		public static IEnumerable<XElement> GetSearchResult(string searchQuery, int pageSize, int pageCount, bool currentSite)
		{
			/* Function explained:
			 * Takes a SearchQuery and splits it up into tokens and searches page title, page abstract and page content
			 * for occurrences of these these tokens. The search tokens are dynamically made into OR statements in the WHERE
			 * clause of the linq statements giving a broader result (as opposed to AND statements). After a sub-result-set have
			 * been made (one for title/abstract and one for content) they are united to a single set (consisting of only page guids).
			 * Lastly this set picks out page id, title, abstract and URL to the page.
			 * There are no ranking of search results.
			 * Only giving a result with the provided Culture i.e. da-DK and page status "published". */

			// Sanity check, dont do anything unless
			if (searchQuery.Length < 2)
			{
				// return empty result set
				yield return new XElement("PagingInfo",
									 new XAttribute("PageCount", pageCount),
									 new XAttribute("PageSize", pageSize),
									 new XAttribute("PageTotal", 0),
									 new XAttribute("HitsTotal", 0),
									 new XAttribute("SearchQuery", searchQuery),
									 new XAttribute("CurrentSite", currentSite));
			}
			else
			{
				using (DataConnection dataConnection = new DataConnection())
				{
					SitemapNavigator sitemapNavigator = new SitemapNavigator(dataConnection);

					// Tokenize
					searchQuery = searchQuery.Trim().ToLower();
					string[] keywords = searchQuery.Split(' ');

					// Build dynamic where clause
					Expression<Func<IPage, bool>> predicate = PredicateBuilder.False<IPage>();
					Expression<Func<IPagePlaceholderContent, bool>> predicate2 = PredicateBuilder.False<IPagePlaceholderContent>();

					foreach (string keyword in keywords)
					{
						string temp = keyword;
						predicate = predicate.Or(p => p.Title.ToLower().Contains(temp) || p.Description.ToLower().Contains(temp));
						predicate2 = predicate2.Or(p => p.Content.ToLower().Contains(temp));
					}

					var pages = dataConnection.Get<IPage>();

					// Search the title and abstract attributes
					var pagesTitleAbstract = from page in pages.Where(predicate)
											 select page.Id;

					// Search the content "attribute"
					var pagesContent = from page in pages
									   join content in dataConnection.Get<IPagePlaceholderContent>().Where(predicate2)
									   on page.Id equals content.PageId
									   select page.Id;

					// Union the different resultsets (and thereby eliminitate duplicates)
					List<Guid> combinedSearchResult = (pagesTitleAbstract.Union(pagesContent)).ToList();

					if (currentSite == true)
					{
						List<Guid> currentSitePages = sitemapNavigator.CurrentPageNode.GetPageIds(SitemapScope.Level1AndDescendants).ToList();

						combinedSearchResult = currentSitePages.Intersect(combinedSearchResult).ToList();
					}

					// no ranking - just get results in mixed fasion
					var pagesGetResultSet = from page in dataConnection.Get<IPage>()
											join CombinedResult in combinedSearchResult
											on page.Id equals CombinedResult
											select new PageResult { Title = page.Title, Description = page.Description, Id = page.Id };

					// The rest is output part

					// Pageing data
					List<PageResult> pagesToShow = pagesGetResultSet.Skip((pageCount - 1) * pageSize).Take(pageSize).ToList();

					yield return new XElement("PagingInfo",
										new XAttribute("PageCount", pageCount),
										new XAttribute("PageSize", pageSize),
										new XAttribute("PageTotal", (int)Math.Ceiling((decimal)pagesGetResultSet.Count() / (decimal)pageSize)),
										new XAttribute("HitsTotal", pagesGetResultSet.Count()),
										new XAttribute("SearchQuery", searchQuery),
										new XAttribute("CurrentSite", currentSite)
										);

					foreach (PageResult page in pagesToShow)
					{
						PageNode resultPageNode = sitemapNavigator.GetPageNodeById(page.Id);

						yield return new XElement("Page",
											new XAttribute("Id", page.Id),
											new XAttribute("Title", page.Title),
											new XAttribute("Description", page.Description),
											new XAttribute("Url", resultPageNode.Url));
					}
				}
			}
		}
	}

	public class PageResult
	{
		public string Title;
		public string Description;
		public Guid Id;
	}

	static class PredicateBuilder
	{
		public static Expression<Func<T, bool>> True<T>() { return f => true; }
		public static Expression<Func<T, bool>> False<T>() { return f => false; }

		public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
		{
			var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
			return Expression.Lambda<Func<T, bool>>(Expression.Or(expr1.Body, invokedExpr), expr1.Parameters);
		}

		public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
		{
			var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
			return Expression.Lambda<Func<T, bool>>(Expression.And(expr1.Body, invokedExpr), expr1.Parameters);
		}
	}
}
