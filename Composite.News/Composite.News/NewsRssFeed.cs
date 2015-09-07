using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using Composite.Core.Linq;
using Composite.Core.Routing;
using Composite.Data;
using Composite.Data.Types;


namespace Composite.News
{
	public class NewsRssFeed : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/xml";
			CultureInfo culture = DataLocalizationFacade.DefaultLocalizationCulture;

		    string pathInfo = context.Request.PathInfo;
            if (pathInfo.Length > 1)
			{
                culture = new CultureInfo(pathInfo.Substring(1));
			}

			using (var con = new DataConnection(culture))
			{
				var feed = new SyndicationFeed
				{
				    Title = new TextSyndicationContent("News List")
				};
			    var items = new List<SyndicationItem>();

			    IQueryable<NewsItem> allNews = con.Get<NewsItem>();

				string hostname = context.Request.Url.Host;
			    var hostnameBinding = con.Get<IHostnameBinding>().FirstOrDefault(h => h.Hostname == hostname);
				if (hostnameBinding != null)
				{
					Guid homepageId = hostnameBinding.HomePageId;
					var sm = new SitemapNavigator(con);
					var pagesFilter = new HashSet<Guid>(
                        sm.GetPageNodeById(homepageId)
                          .GetPageNodes(SitemapScope.DescendantsAndCurrent)
                          .Select(p => p.Id));

                    allNews = allNews.Evaluate().Where(n => pagesFilter.Contains(n.PageId)).ToList().AsQueryable();
				}

			    IEnumerable<NewsItem> latestNews = allNews.OrderByDescending(d => d.Date).Take(10);

				foreach (var news in latestNews.Evaluate())
				{
				    var page = PageManager.GetPageById(news.PageId);
				    if (page == null)
				    {
				        continue;
				    }

				    var pageUrlData = new PageUrlData(page) {PathInfo = NewsFacade.GetPathInfo(news.TitleUrl, news.Date)};
                    string pageUrl = PageUrls.BuildUrl(pageUrlData);
				    if (pageUrl == null)
				    {
				        continue;
				    }

					var item = new SyndicationItem(
						news.Title,
						news.Teaser,
						context.GetPath(pageUrl),
						news.Id.ToString(),
						news.Date
						);
					item.Categories.Add( new SyndicationCategory(news.PageId.ToString()) );
					items.Add(item);
				}
				feed.Items = items;
				var writer = new XmlTextWriter(context.Response.Output);
				feed.SaveAsRss20(writer);
			}
		}

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

	}

	internal static class NewsRssFeedExtensions
	{
		public static Uri GetPath(this HttpContext context, string path)
		{
			return new Uri(context.Request.Url, path);
		}
	}
}
