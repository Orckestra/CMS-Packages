using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Composite.Core.Routing.Pages;
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

        public static string GetPathInfo(NewsItem newsItem)
        {
            Verify.ArgumentNotNull(newsItem, "newsItem");

            return GetPathInfo(newsItem.TitleUrl, newsItem.Date);
        }


        public static string GetPathInfo(string titleUrl, DateTime dateTime)
        {
            return string.Format("/{0:yyyy}/{0:MM}/{0:dd}/{1}", dateTime, titleUrl);
        }


        public static Expression<Func<NewsItem, bool>> GetNewsFilterFromUrl(out bool pathInfoResolved)
        {
            var currentPageId = SitemapNavigator.CurrentPageId;
            IsNewsItem = false;
            PageNumber = 1;

            var pathInfoParts = GetPathInfoParts();
            pathInfoResolved = false;
            if (pathInfoParts != null)
            {
                if (pathInfoParts.Length == 5)
                {
                    IsNewsItem = true;
                    int year, month, day;
                    if (
                        int.TryParse(pathInfoParts[1], out year)
                        && int.TryParse(pathInfoParts[2], out month)
                        && int.TryParse(pathInfoParts[3], out day)
                        && DateTimeUtils.IsValidDate(year, month, day)
                        )
                    {
                        var titleUrl = pathInfoParts[4];
                        var date = new DateTime(year, month, day);
                        pathInfoResolved = true;
                        return f => f.Date.Date == date && f.TitleUrl == titleUrl && f.PageId == currentPageId;
                    }
                }

                if (pathInfoParts.Length == 2)
                {
                    int pageNumberValue;
                    if (int.TryParse(pathInfoParts[1], out pageNumberValue) && pageNumberValue > 0)
                    {
                        PageNumber = pageNumberValue;
                        pathInfoResolved = true;
                    }
                }
            }

            return f => f.PageId == currentPageId;
        }


        private static string[] GetPathInfoParts()
        {
            var pathInfo = C1PageRoute.GetPathInfo() ?? string.Empty;
            return pathInfo.Contains("/") ? pathInfo.Split('/') : null;
        }
    }
}
