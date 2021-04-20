using System;
using System.Globalization;
using System.Threading;
using System.Web;
using Composite.C1Console.Security;
using Composite.C1Console.Users;
using Composite.Core.Configuration;
using Composite.Core.WebClient;
using Composite.Core.WebClient.Presentation;

namespace Composite.Hotfixes
{
    public class Hotfix679HttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            if (!SystemSetupFacade.IsSystemFirstTimeInitialized)
            {
                return;
            }

            context.PostRequestHandlerExecute += context_PostRequestHandlerExecute;
        }


        void context_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;

            bool adminRootRequest = IsAdminConsoleRequest(context.Request.Path);

            if (adminRootRequest && context.Request.UserAgent.Contains("Chrome/")
                && context.Items.Contains("AdministrativeOutputTransformationHttpModule.TransformationList")
                )
            {

                OutputTransformationManager.RegisterTransformation(
                    context.Request.MapPath("~/Composite/InstalledPackages/hotfixes/769/769.xsl"), 5);

            }
        }


        public static bool IsAdminConsoleRequest(string requestPath)
        {
            return string.Compare(requestPath, UrlUtils.AdminRootPath, StringComparison.OrdinalIgnoreCase) == 0
                   || requestPath.StartsWith(UrlUtils.AdminRootPath + "/", StringComparison.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
        }
    }
}
