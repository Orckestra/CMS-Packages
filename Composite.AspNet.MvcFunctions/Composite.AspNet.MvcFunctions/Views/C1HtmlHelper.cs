using System.Web.Mvc;
using Composite.Core.Routing;
using Composite.Core.WebClient.Renderings.Page;

namespace Composite.AspNet.MvcFunctions.Views
{
    public class C1HtmlHelper
    {
        private readonly HtmlHelper _helper;

        public C1HtmlHelper(HtmlHelper helper)
        {
            _helper = helper;
        }

        public string PageUrl()
        {
            var currentPage = PageRenderer.CurrentPage;
            return PageUrls.BuildUrl(currentPage) ?? PageUrls.BuildUrl(currentPage, UrlKind.Internal);
        }
    }
}
