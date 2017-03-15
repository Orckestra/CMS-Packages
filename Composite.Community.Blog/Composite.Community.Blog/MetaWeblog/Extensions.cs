using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Community.Blog.MetaWeblog
{
    public static class Extensions
    {
        public static string GetPublicUrl(this IPage page)
        {
            return GetPagePublicUrl(page.Id);
        }

        public static string GetPublicUrl(this Entries entry)
        {
            return BlogFacade.GetBlogUrl(entry);
        }

        public static string GetPagePublicUrl(Guid pageId)
        {
            string pageUrl = null;
            if (PageStructureInfo.TryGetPageUrl(pageId, out pageUrl))
            {
                return new Uri(HttpContext.Current.Request.Url, pageUrl).OriginalString;
            }
            return pageUrl;
        }
    }

    internal class UpdateDataWrapper<T> : IDisposable where T : class, IData
    {
        public UpdateDataWrapper(Expression<Func<T, bool>> predicate)
        {
            Data = DataFacade.GetData(predicate).Single();
        }

        public T Data { get; set; }

        public void Dispose()
        {
            if (Data != null) DataFacade.Update(Data);
        }
    }
}