using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using Composite.Data;
using Composite.Data.Types;

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
                bool isGlobal = Convert.ToBoolean(context.Request["IsGlobal"]);
                string cultureName = context.Request["cultureName"];
                if (string.IsNullOrEmpty(cultureName))
                {
                    cultureName = DataLocalizationFacade.DefaultLocalizationCulture.Name;
                }
                string cachedRssKey = string.Format(CacheRSSKeyTemplate, pageId, cultureName);
                if (context.Cache[cachedRssKey] == null)
                {
                    var cultureInfo = new CultureInfo(cultureName);
                    context.Response.ContentType = "text/xml";

                    using (var conn = new DataConnection(cultureInfo))
                    {
                        string pageUrl = BlogFacade.GetPageUrlById(pageId);
                        if (!string.IsNullOrEmpty(pageUrl))
                        {
                            pageUrl = BlogFacade.GetFullPath(pageUrl);
                            string pageTitle =
                                conn.Get<IPage>().Where(p => p.Id == pageId).Select(p => p.Title).Single();
                            var feed = new SyndicationFeed(pageTitle, "", new Uri(pageUrl));
                            var blogItems =
                                conn.Get<Entries>()
                                    .Where(b => isGlobal ? b.PageId != null : b.PageId == pageId)
                                    .Select(b => new {b.Id, b.Title, b.Date, b.Teaser, b.Tags, b.PageId})
                                    .OrderByDescending(b => b.Date)
                                    .ToList();

                            List<SyndicationItem> items = (from blog in blogItems
                                                           let blogUrl =
                                                               BlogFacade.GetBlogUrl(blog.Date, blog.Title, blog.PageId,
                                                                                     pageUrl)
                                                           select
                                                               new SyndicationItem(blog.Title, blog.Teaser,
                                                                                   new Uri(blogUrl), blog.Id.ToString(),
                                                                                   blog.Date)).ToList();

                            feed.Items = items;
                            context.Cache[cachedRssKey] = feed;
                        }
                    }
                }

                var syndicationFeed = (SyndicationFeed) context.Cache[cachedRssKey];

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
            get { return true; }
        }
    }
}