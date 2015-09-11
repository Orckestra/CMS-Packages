using System;
using System.Linq;
using Composite.Core.Routing;
using Composite.Data;

namespace Composite.Community.Blog
{
    class BlogEntryDataUrlMapper: IDataUrlMapper
    {
        public IDataReference GetData(PageUrlData pageUrlData)
        {
            string pathInfo = pageUrlData.PathInfo;
            if(string.IsNullOrEmpty(pathInfo)) return null;

            string[] pathInfoParts = pathInfo.Split('/');
            if (pathInfoParts.Length != 5) return null;

            var filter = BlogFacade.GetBlogFilter(pageUrlData.PageId, pathInfoParts);
            if (filter == null) return null;

            var entry = DataFacade.GetData<Entries>().Where(filter).FirstOrDefault();

            return entry != null ? entry.ToDataReference() : null;
        }

        public PageUrlData GetPageUrlData(IDataReference instance)
        {
            if (!instance.IsSet || instance.ReferencedType != typeof (Entries))
            {
                return null;
            }

            var entry = instance.Data as Entries;
            if (entry == null || entry.PageId == Guid.Empty)
            {
                return null;
            }

            using (new DataScope(entry.DataSourceId.LocaleScope))
            {
                var page = PageManager.GetPageById(entry.PageId);
                if (page == null)
                {
                    return null;
                }

                return new PageUrlData(page)
                {
                    PathInfo = BlogFacade.GetBlogPath(entry)
                };
            }
        }
    }
}
