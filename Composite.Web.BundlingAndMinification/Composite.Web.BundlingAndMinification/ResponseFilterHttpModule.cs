using Composite.C1Console.Security;
using Composite.Core.WebClient;
using System;
using System.Web;
using System.Web.UI;

namespace Orckestra.Web.BundlingAndMinification
{
    internal class ResponseFilterHttpModule : IHttpModule
    {
        public void Init(HttpApplication context) => context.PostMapRequestHandler += AttachFilter;

        private static void AttachFilter(object sender, EventArgs e)
        {
            HttpContext httpContext = HttpContext.Current;

            bool bundleAndMinifyScripts = System.Configuration.ConfigurationManager
                .AppSettings["Orckestra.Web.BundlingAndMinification.BundleAndMinifyScripts"]
                .Equals("true", StringComparison.OrdinalIgnoreCase);

            bool bundleAndMinifyStyles = System.Configuration.ConfigurationManager
                .AppSettings["Orckestra.Web.BundlingAndMinification.BundleAndMinifyStyles"]
                .Equals("true", StringComparison.OrdinalIgnoreCase);

            bool removeComments = System.Configuration.ConfigurationManager.
                AppSettings["Orckestra.Web.BundlingAndMinification.RemoveComments"]
                .Equals("true", StringComparison.OrdinalIgnoreCase);

            if ((!bundleAndMinifyScripts && !bundleAndMinifyStyles) ||
                httpContext.IsDebuggingEnabled ||
                !(httpContext.Handler is Page) ||
                UserValidationFacade.IsLoggedIn() ||
                IsAdminConsoleRequest(httpContext))
            {
                return;
            }

            httpContext.Response.Filter = new ResponseFilterStream(
                httpContext.Response.Filter,
                httpContext.Response.ContentEncoding,
                bundleAndMinifyScripts,
                bundleAndMinifyStyles,
                removeComments);
        }

        private static bool IsAdminConsoleRequest(HttpContext httpContext)
        {
            string relativeUrl = httpContext.Request.Path;
            return
                string.Equals(relativeUrl, UrlUtils.AdminRootPath, StringComparison.OrdinalIgnoreCase) ||
                relativeUrl.StartsWith(UrlUtils.AdminRootPath + "/", StringComparison.OrdinalIgnoreCase);
        }

        public void Dispose() { }
    }
}