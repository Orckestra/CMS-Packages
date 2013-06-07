using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Composite.Core.WebClient.Services.WysiwygEditor;
using Composite.Data;
using Composite.Data.ProcessControlled.ProcessControllers.GenericPublishProcessController;
using Composite.Data.Types;
using CookComputing.XmlRpc;

namespace Composite.Community.Blog.MetaWeblog
{
    internal class MetaWeblogBlog : MetaWeblogData
    {
        private readonly Expression<Func<Entries, bool>> predicate;

        public MetaWeblogBlog(string username, string password, string blogid)
            : base(username, password)
        {
            PageId = ParsePageId(blogid);
            predicate = d => d.PageId == PageId && d.Author == Author.Id;
        }

        protected Guid PageId { get; private set; }

        public static Guid ParsePageId(string blogid)
        {
            Guid pageId;
            if (Guid.TryParse(blogid, out pageId))
            {
                return pageId;
            }
            throw new XmlRpcFaultException(0, "Invalid blog id");
        }

        public Guid AddPost(string title, string description, string publicationStatus)
        {
            IPage page = PageManager.GetPageById(PageId, true);
            if (page != null)
            {
                var entry = DataFacade.BuildNew<Entries>();
                entry.Date = DateTime.Now;
                entry.Title = title;
                entry.Content = RelativeMediaUrl(MarkupTransformationServices.TidyHtml(description).Output.Root);
                entry.PublicationStatus = publicationStatus;
                entry.Tags = string.Empty;
                entry.Teaser = string.Empty;
                entry.Author = Author.Id;
                PageFolderFacade.AssignFolderDataSpecificValues(entry, page);
                entry = DataFacade.AddNew(entry);

                if (publicationStatus == GenericPublishProcessController.Published)
                {
                    entry.PublicationStatus = publicationStatus;
                    DataFacade.Update(entry);
                }
                return entry.Id;
            }
            return Guid.Empty;
        }

        public IEnumerable<Entries> GetRecentPosts()
        {
            return DataFacade.GetData(predicate).OrderByDescending(d => d.Date);
        }
    }
}