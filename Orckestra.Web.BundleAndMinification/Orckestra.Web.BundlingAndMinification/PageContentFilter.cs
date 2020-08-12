using System;
using System.Web;
using Composite.C1Console.Security;
using Composite.Core.WebClient;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Data.Types;
using static System.Configuration.ConfigurationManager;

namespace Orckestra.Web.BundlingAndMinification
{
    public class PageContentFilter : IPageContentFilter
    {
        private const string AppSettingsPath = "Orckestra.Web.BundlingAndMinification";
        internal static readonly bool BundleMinifyScripts;
        internal static readonly bool BundleMinifyStyles;

        static PageContentFilter()
        {
            BundleMinifyScripts = AppSettings[$"{AppSettingsPath}.BundleAndMinifyScripts"].Equals("true", StringComparison.OrdinalIgnoreCase);
            BundleMinifyStyles = AppSettings[$"{AppSettingsPath}.BundleAndMinifyStyles"].Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        //To be executed in the end, after all another filter
        public int Order => int.MaxValue;

        public void Filter(XhtmlDocument document, IPage page)
        {
            HttpContext httpContext = HttpContext.Current;

            if ((!BundleMinifyScripts && !BundleMinifyStyles)
                || IsAdminConsoleRequest(httpContext)
                || (httpContext.Request["c1mode"] != "perf" &&
                (httpContext.IsDebuggingEnabled       
                || UserValidationFacade.IsLoggedIn())))
            {
                return;
            }

            ActionContainer actionContainer = new ActionContainer(document);
            actionContainer.Invoke();
        }

        private static bool IsAdminConsoleRequest(HttpContext httpContext)
        {
            string relativeUrl = httpContext.Request.Path;
            return string.Equals(relativeUrl, UrlUtils.AdminRootPath, StringComparison.OrdinalIgnoreCase) ||
                relativeUrl.StartsWith(UrlUtils.AdminRootPath + "/", StringComparison.OrdinalIgnoreCase);
        }
    }
}