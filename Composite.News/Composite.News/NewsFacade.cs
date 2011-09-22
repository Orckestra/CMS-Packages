using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using Composite.Core;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Data;

namespace Composite.News
{
	public class NewsFacade
	{
		public static int GetPageNumber()
		{
			var pathInfoParts = GetPathInfoParts();
			if (pathInfoParts != null && pathInfoParts.Length == 2)
			{
				int pageNumber;
				if (int.TryParse(pathInfoParts[1], out pageNumber))
					return pageNumber;
			}
			return 1;
		}

		public static string GetUrlFromTitle(string title)
		{
			return Regex.Replace(title, @"[^\w\d ]+", string.Empty).Replace(" ","-");
		}

		internal static void SetTitleUrl(object sender, DataEventArgs dataEventArgs)
		{
			NewsItem news = (NewsItem)dataEventArgs.Data;
			news.TitleUrl = GetUrlFromTitle(news.Title);
		}

		internal static string GetPathInfo(string titleUrl, DateTime dateTime)
		{
			return string.Format("/{0:yyyy}/{0:MM}/{0:dd}/{1}", dateTime, titleUrl);
		}

		public static Expression<Func<NewsItem, bool>> GetNewsFilterFromUrl()
		{
			Guid currentPageId = PageRenderer.CurrentPageId;
			Expression<Func<NewsItem, bool>> filter = f => f.PageId == currentPageId;

			var pathInfoParts = GetPathInfoParts();
			if (pathInfoParts != null)
			{
				if (pathInfoParts.Length > 4)
				{
					int year, month, day = 0;
					if (
						int.TryParse(pathInfoParts[1], out year)
						&& int.TryParse(pathInfoParts[2], out month)
						&& int.TryParse(pathInfoParts[3], out day)
						)
					{
						var titleUrl = pathInfoParts[4];
						DateTime date = new DateTime(year, month, day);
						filter = f => f.Date.Date == date && f.TitleUrl == titleUrl && f.PageId == currentPageId;
					}
				}
			}
			return filter;

		}
		public static string[] GetPathInfoParts()
		{
            var pathInfo = GetPathInfo();
			return pathInfo != null && pathInfo.Contains("/") ? pathInfo.Split('/') : null;
		}

        private static string GetPathInfo()
        {
            Type pageRoute = typeof(Composite.Data.IData).Assembly.GetType("Composite.Core.Routing.Pages.C1PageRoute", false);
            string result = string.Empty;

            if (pageRoute == null)
            {
                // Support for version 2.1.2
                result = HttpContext.Current.Request.PathInfo;
                return !string.IsNullOrEmpty(result) ? result: new UrlBuilder(HttpContext.Current.Request.RawUrl).PathInfo;
            }

            // Support for version 2.1.3+
             result = (pageRoute
                .GetMethod("GetPathInfo", BindingFlags.Public | BindingFlags.Static)
                .Invoke(null, new object[0]) as string) ?? string.Empty;

            if (result != string.Empty)
            {
                pageRoute
                    .GetMethod("RegisterPathInfoUsage", BindingFlags.Public | BindingFlags.Static)
                    .Invoke(null, new object[0]);
            }

            return result;
        }

	}
}
