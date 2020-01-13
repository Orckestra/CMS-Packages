using Composite.C1Console.Security;
using Composite.Core;
using Composite.Core.WebClient;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Data.Types;
using System;
using System.Web;
using System.Web.Optimization;
using System.Web.UI;
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
            if (string.IsNullOrEmpty(document?.Root?.Document?.ToString()))
            {
                return;
            }

            //removing empty subset if exist
            XDocumentType doctype = document.Document.DocumentType;
            if (doctype?.InternalSubset?.Trim() == string.Empty)
            {
                doctype.InternalSubset = null;
            }

            HttpContext httpContext = HttpContext.Current;
            if ((!_bundleAndMinifyScripts && !_bundleAndMinifyStyles) ||
                httpContext.IsDebuggingEnabled ||
                !(httpContext.Handler is Page) ||
                UserValidationFacade.IsLoggedIn() ||
                IsAdminConsoleRequest(httpContext))
            {
                return;
            }

            XhtmlDocument optimizedPage = GetOptimizedDocument(document);
            if (optimizedPage?.Root != null)
            {
                document.Root.ReplaceWith(optimizedPage.Root);
            }
        }

        private static bool IsAdminConsoleRequest(HttpContext httpContext)
        {
            string relativeUrl = httpContext.Request.Path;
            return
                string.Equals(relativeUrl, UrlUtils.AdminRootPath, StringComparison.OrdinalIgnoreCase) ||
                relativeUrl.StartsWith(UrlUtils.AdminRootPath + "/", StringComparison.OrdinalIgnoreCase);
        }

        private XhtmlDocument GetOptimizedDocument(XhtmlDocument document)
        {
            int progress = 0; //counter, are there any changes at all
            Bundle bundle = default;
            /* Attempts to optimize scripts and styles are divided. 
             * In case some problems with scripts - at least to process styles, and visa versa */
            if (_bundleAndMinifyScripts)
            {
                try
                {
                    /* Processing page and outing bundle object, in case something went wrong 
                     * to delete certain bundle, not to clear the whole table */
                    document = OptimizationManager.GetPageWithOptimizedScripts(document, out bundle);
                    progress++;
                }
                catch (Exception ex)
                {
                    /* Risk of logs trashing by repeatable errors, maybe a good idea to take
                     * this into account on a logger level */
                    Log.LogError(nameof(OptimizationManager), ex);
                    if (bundle != null)
                    {
                        BundleTable.Bundles.Remove(bundle);
                    }
                }
            }

            if (_bundleAndMinifyStyles)
            {
                try
                {
                    document = OptimizationManager.GetPageWithOptimizedStyles(document, out bundle);
                    progress++;
                }
                catch (Exception ex)
                {
                    Log.LogError(nameof(OptimizationManager), ex);
                    if (bundle != null)
                    {
                        BundleTable.Bundles.Remove(bundle);
                    }
                }
            }
            return progress.Equals(0) ? null : document;
        }
    }
}
