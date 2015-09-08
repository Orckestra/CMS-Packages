using System;
using System.Linq;
using Composite.Core.Routing;
using Composite.Data;

namespace Composite.News
{
    class NewsDataUrlMapper : IDataUrlMapper
    {
        public PageUrlData GetPageUrlData(IDataReference dataReference)
        {
            if (dataReference.ReferencedType != typeof(NewsItem))
            {
                return null;
            }

            var data = dataReference.Data;

            var newsItem = data as NewsItem;
            if (newsItem == null) return null;

            var page = PageManager.GetPageById(newsItem.PageId);
            if (page == null) return null;

            var newsPathInfo = NewsFacade.GetPathInfo(newsItem.TitleUrl, newsItem.Date);

            return new PageUrlData(page) { PathInfo = newsPathInfo };
        }

        public IDataReference GetData(PageUrlData pageUrlData)
        {
            string pathInfo = pageUrlData.PathInfo;
            if (pathInfo == null)
            {
                return null;
            }

            string[] pathInfoParts = pathInfo.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (pathInfoParts.Length != 4) return null;

            if (string.IsNullOrEmpty(pathInfo)) return null;

            int year, month, day;

            if (int.TryParse(pathInfoParts[0], out year) 
                && int.TryParse(pathInfoParts[1], out month) 
                && int.TryParse(pathInfoParts[2], out day)
                && DateTimeUtils.IsValidDate(year, month, day))
            {
                Guid pageId = pageUrlData.PageId;
                string titleUrl = pathInfoParts[3];
                var date = new DateTime(year, month, day);

                var data = DataFacade.GetData<NewsItem>()
                    .FirstOrDefault(n => n.PageId == pageId && n.Date.Date == date && n.TitleUrl == titleUrl);

                return data != null ? data.ToDataReference() : null;
            }

            return null;
        }
    }
}
