using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Data;
using Composite.Data.Types;
using System.Linq.Expressions;

namespace Composite.News
{
	public class NewsRssFeed : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/xml";
			CultureInfo culture = DataLocalizationFacade.DefaultLocalizationCulture;
			if (context.Request.PathInfo.Length > 0)
			{
				culture = new CultureInfo(context.Request.PathInfo.Substring(1));
			}
			using (DataConnection con = new DataConnection(culture))
			{
				SyndicationFeed feed = new SyndicationFeed();
				feed.Title = new TextSyndicationContent("News List");
				List<SyndicationItem> items = new List<SyndicationItem>();
				IEnumerable<NewsItem> latestNews;

				string hostname = context.Request.Url.Host;
				Guid homepageId = Guid.Empty;
				var hostnameBinding = con.Get<IHostnameBinding>().Where(h => h.Hostname == hostname).FirstOrDefault();
				if (hostnameBinding != null)
				{
					homepageId = hostnameBinding.HomePageId;
					SitemapNavigator sm = new SitemapNavigator(con);
					var pagesFilter = sm.GetPageNodeById(homepageId).GetPageNodes(SitemapScope.DescendantsAndCurrent).Select(p => p.Id);
					latestNews = con.Get<NewsItem>().Where(n => pagesFilter.Contains(n.PageId)).OrderByDescending(d => d.Date).Take(10);
				}
				else
					latestNews = con.Get<NewsItem>().OrderByDescending(d => d.Date).Take(10);

				foreach (var news in latestNews)
				{
					string pageUrl;
					if (PageStructureInfo.TryGetPageUrl(news.PageId, out pageUrl))
					{
						SyndicationItem item = new SyndicationItem(
							news.Title,
							news.Teaser,
							context.GetPath(pageUrl + NewsFacade.GetPathInfo(news.TitleUrl, news.Date)),
							news.Id.ToString(),
							news.Date
							);
						item.Categories.Add(
							new SyndicationCategory(news.PageId.ToString())
							);
						items.Add(item);
					}
				}
				feed.Items = items;
				XmlTextWriter writer = new XmlTextWriter(context.Response.Output);
				feed.SaveAsRss20(writer);
			}
		}

		public bool IsReusable
		{
			get
			{
				return false;
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
