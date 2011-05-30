using System;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using Composite.Data;
using Composite.Data.Types;
using Composite.Core.WebClient.Renderings.Page;

namespace Composite.Community.Blog
{
	public class BlogRssFeed : IHttpHandler
	{
		public const string CacheRSSKeyTemplate = "BlogRssFeed-{0}-{1}";

		public void ProcessRequest(HttpContext context)
		{
			Guid pageId = Guid.Empty;

			if (context.Request["bid"] != null && Guid.TryParse(context.Request["bid"], out pageId))
			{
				var cultureName = context.Request["cultureName"];
				string cachedRssKey = string.Format(CacheRSSKeyTemplate, pageId, cultureName);
				if (context.Cache[cachedRssKey] == null)
				{
					if (string.IsNullOrEmpty(cultureName))
					{
						cultureName = string.Empty;
					}

					var cultureInfo = new CultureInfo(cultureName);

					context.Response.ContentType = "text/xml";
					using (new DataScope(DataScopeIdentifier.Public))
					{
						using (new DataScope(cultureInfo))
						{
							string pageUrl;
							PageStructureInfo.TryGetPageUrl(pageId, out pageUrl);
							if (!string.IsNullOrEmpty(pageUrl))
							{
								pageUrl = BlogFacade.GetFullPath(pageUrl);
								string pageTitle = DataFacade.GetData<IPage>().Where(p => p.Id == pageId).Select(p => p.Title).Single();
								var feed = new SyndicationFeed(pageTitle, "", new Uri(pageUrl));
								var blogItems =
									DataFacade.GetData<Entries>().Where(b => b.PageId == pageId).Select(
										b => new { b.Id, b.Title, b.Date, b.Teaser, b.Tags }).OrderByDescending(b => b.Date).ToList();

								var items = (from blog in blogItems
											 let blogUrl = pageUrl + BlogFacade.GetBlogUrl(blog.Date, blog.Title)
											 select new SyndicationItem(blog.Title, blog.Teaser, new Uri(blogUrl), blog.Id.ToString(), blog.Date))
									.ToList();

								feed.Items = items;
								context.Cache[cachedRssKey] = feed;
							}
						}
					}
				}

				var syndicationFeed = (SyndicationFeed)context.Cache[cachedRssKey];

				using (XmlWriter writer = XmlWriter.Create(context.Response.OutputStream))
				{
					syndicationFeed.SaveAsRss20(writer);
				}
			}
			else
			{
				context.Response.Write("The required query paramerer 'bid' (blog page GUID) is missing.");
			}
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}
