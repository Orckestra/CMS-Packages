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
        public static bool IsNewsItem { get; private set; }

        public static int PageNumber { get; private set; }

        public static string GetUrlFromTitle(string title)
        {
            return Regex.Replace(title, @"[^\w\d ]+", string.Empty).Replace(" ", "-");
        }

        internal static void SetTitleUrl(object sender, DataEventArgs dataEventArgs)
        {
            NewsItem news = (NewsItem)dataEventArgs.Data;
            news.TitleUrl = GetUrlFromTitle(news.Title);
        }

        public static string GetPathInfo(string titleUrl, DateTime dateTime)
        {
            return string.Format("/{0:yyyy}/{0:MM}/{0:dd}/{1}", dateTime, titleUrl);
        }

        public static string GetPathInfo()
        {
            var pathInfo = Composite.Core.Routing.Pages.C1PageRoute.GetPathInfo();

            if (pathInfo != null)
            {
                Composite.Core.Routing.Pages.C1PageRoute.RegisterPathInfoUsage();
            }

            return pathInfo;
        }

        public static Expression<Func<NewsItem, bool>> GetNewsFilterFromUrl()
        {
            var currentPageId = SitemapNavigator.CurrentPageId;
            Expression<Func<NewsItem, bool>> filter = f => f.PageId == currentPageId;
            IsNewsItem = false;
            PageNumber = 1;
            var pathInfoParts = GetPathInfoParts();
            if (pathInfoParts != null)
            {
                if (pathInfoParts.Length > 4)
                {
                    IsNewsItem = true;
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
                if (pathInfoParts.Length == 2)
                {
                    int pageNamberValue;
                    if (!int.TryParse(pathInfoParts[1], out pageNamberValue))
                    {
                        PageNumber = 1;
                    }
                    else
                    {
                        PageNumber = pageNamberValue;
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

    }
}
