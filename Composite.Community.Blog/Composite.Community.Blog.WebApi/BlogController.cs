using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;
using System.Web.Http;
using Composite.Community.Blog.Models;
using Composite.Community.Blog.Parameters;
using Composite.Core.Extensions;
using Composite.Core.Routing;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Community.Blog.Api
{
    public class BlogController : ApiController
    {
        [HttpGet]
        [ActionName("items")]
        public BlogListModel GetBlogEntries([FromUri] GetBlogEntriesParameter param)
        {

            var culture = DataLocalizationFacade.DefaultLocalizationCulture;
            if (!string.IsNullOrEmpty(param.CultureCode))
            {
                culture = CultureInfo.GetCultureInfo(param.CultureCode);
            }

            using (var con = new DataConnection(PublicationScope.Published, culture))
            {
                var model = new BlogListModel();

                var filter = GetBlogFilter(param);
                var entries = con.Get<Entries>().Where(filter).OrderByDescending(e => e.Date).ToList();

                var entriesCount = entries.Count();

                model.Title = GetBlogListTitle(param.Tags, entriesCount);

                model.PageCount = param.PageSize == 0
                    ? entriesCount
                    : (entriesCount + param.PageSize - 1)/param.PageSize;
                model.PageNumber = param.PageNumber;

                var entriesForPage = param.PageSize == 0
                    ? entries
                    : entries.Skip(param.PageSize*(param.PageNumber - 1)).Take(param.PageSize).ToList();

                model.Items = new List<BlogListItemModel>();

                var pageUrl = GetFullBlogPageUrl(param.BlogPageId);

                foreach (var entry in entriesForPage)
                {
                    model.Items.Add(CreateBlogItemModel(entry, pageUrl));
                }

                return model;
            }

        }

        private BlogListItemModel CreateBlogItemModel(Entries entry, string blogPageUrl)
        {
            using (var con = new DataConnection())
            {
                var image = con.Get<IMediaFile>().FirstOrDefault(m => m.KeyPath == entry.Image);
                var model = new BlogListItemModel
                {
                    Title = entry.Title,
                    Url = BlogFacade.GetBlogUrl(entry.Date, entry.Title, entry.PageId, blogPageUrl),
                    Tags = entry.Tags,
                    TagsByType = BlogFacade.GetTagsByType(entry.Tags),
                    ImageUrl = image != null ? BlogFacade.GetFullPath(MediaUrls.BuildUrl(image)) : string.Empty
                };

                return model;
            }
        }

        private static string GetBlogListTitle(string tags, int totalItems)
        {
            var rm = new ResourceManager("Resources.Blog", Assembly.Load("App_GlobalResources"));

            var title = rm.GetString("List_AllBlogsTitle");
            if (!string.IsNullOrEmpty(tags))
            {
                var separator = rm.GetString("List_SelectedTagsTitle_Separator");
                title = BlogFacade.Decode(tags.Replace(",", separator));
                if (totalItems == 0)
                {
                    title = rm.GetString("List_SelectedTagsTitle_NoFound").FormatWith(title);
                }
            }
            return title;
        }

        private Expression<Func<Entries, bool>> GetBlogFilter(GetBlogEntriesParameter param)
        {
            Expression<Func<Entries, bool>> filter;

            if (!string.IsNullOrEmpty(param.Tags))
            {
                var tagsList = param.Tags.Split(',').Select(i => BlogFacade.Decode(i)).ToList();
                filter =
                    f =>
                        Enumerable.Intersect(f.Tags.Split(','), tagsList).Count() == tagsList.Count &&
                        (param.BlogPageId == Guid.Empty || f.PageId == param.BlogPageId);
            }
            else
            {
                filter = f => param.BlogPageId == Guid.Empty || f.PageId == param.BlogPageId;
            }

            return filter;
        }

        private string GetFullBlogPageUrl(Guid blogPageId)
        {
            var pageUrl = string.Empty;
            if (blogPageId != Guid.Empty)
            {
                pageUrl = BlogFacade.GetPageUrlById(blogPageId);
                if (!string.IsNullOrEmpty(pageUrl))
                {
                    pageUrl = BlogFacade.GetFullPath(pageUrl);
                }
            }
            return pageUrl;
        }
    }
}
