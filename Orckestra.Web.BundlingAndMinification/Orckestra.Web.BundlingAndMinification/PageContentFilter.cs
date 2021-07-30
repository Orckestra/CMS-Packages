using System;
using System.Web;
using Composite.C1Console.Security;
using Composite.Core.WebClient;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Data.Types;
using static Orckestra.Web.BundlingAndMinification.CommonValues;


namespace Orckestra.Web.BundlingAndMinification
{
    public class PageContentFilter : IPageContentFilter
    {

        //To be executed in the end, after all another filter but before CdnPublisher
        public int Order => 999;

        public void Filter(XhtmlDocument document, IPage page)
        {
            HttpContext httpContext = HttpContext.Current;

            //if (
            //    (!BundleMinifyScripts && !BundleMinifyStyles)
            //    || IsAdminConsoleRequest(httpContext)
            //    || (httpContext.Request["c1mode"] != "perf"
            //    &&
            //    (httpContext.IsDebuggingEnabled       
            //    || UserValidationFacade.IsLoggedIn()))
            //)
            //{
            //    return;
            //}

            DocumentProcessor processor = new DocumentProcessor(document);
            processor.Execute();
        }

        private static bool IsAdminConsoleRequest(HttpContext httpContext)
        {
            string relativeUrl = httpContext.Request.Path;
            return string.Equals(relativeUrl, UrlUtils.AdminRootPath, StringComparison.OrdinalIgnoreCase) ||
                relativeUrl.StartsWith(UrlUtils.AdminRootPath + "/", StringComparison.OrdinalIgnoreCase);
        }
    }
}