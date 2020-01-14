using Composite.C1Console.Security;
using Composite.Core.WebClient;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Data.Types;
using System;
using System.Web;
using System.Xml.Linq;
using static System.Configuration.ConfigurationManager;

namespace Orckestra.Web.BundlingAndMinification
{
    public class PageContentFilter : IPageContentFilter
    {
        private static bool _bundleAndMinifyScripts = AppSettings["Orckestra.Web.BundlingAndMinification.BundleAndMinifyScripts"]
            .Equals("true", StringComparison.OrdinalIgnoreCase);

        private static bool _bundleAndMinifyStyles = AppSettings["Orckestra.Web.BundlingAndMinification.BundleAndMinifyStyles"]
            .Equals("true", StringComparison.OrdinalIgnoreCase);

        //to be executed in the end, after all another filters
        public int Order => int.MaxValue;

        public void Filter(XhtmlDocument document, IPage page)
        {
            HttpContext httpContext = HttpContext.Current;
            if ((!_bundleAndMinifyScripts && !_bundleAndMinifyStyles) ||
                httpContext.IsDebuggingEnabled ||
                IsAdminConsoleRequest(httpContext) ||
                UserValidationFacade.IsLoggedIn()
                )
            {
                return;
            }

            //removing empty subset if exist
            XDocumentType doctype = document.Document.DocumentType;
            if (doctype?.InternalSubset?.Trim() == string.Empty)
            {
                doctype.InternalSubset = null;
            }

            OptimizeDocument(document);
        }

        private static bool IsAdminConsoleRequest(HttpContext httpContext)
        {
            string relativeUrl = httpContext.Request.Path;
            return
                string.Equals(relativeUrl, UrlUtils.AdminRootPath, StringComparison.OrdinalIgnoreCase) ||
                relativeUrl.StartsWith(UrlUtils.AdminRootPath + "/", StringComparison.OrdinalIgnoreCase);
        }

        private void OptimizeDocument(XhtmlDocument document)
        {
            if (_bundleAndMinifyScripts)
            {
                OptimizationManager.OptimizeDocumentScripts(document);
            }
            if (_bundleAndMinifyStyles)
            {
                OptimizationManager.OptimizeDocumentStyles(document);
            }
        }
    }
}
