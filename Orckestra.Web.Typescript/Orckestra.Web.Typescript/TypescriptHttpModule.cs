using Composite.Core.WebClient;
using Orckestra.Web.Typescript.Classes;
using System;
using System.Web;

namespace Orckestra.Web.Typescript
{
    public class TypescriptHttpModule : IHttpModule
    {
        public static bool IsDebugMode = HttpContext.Current.IsDebuggingEnabled;

        public void Dispose() { }
        public void Init(HttpApplication application)
        {
            if (!Helper.PackageEnabled)
            {
                return;
            }
            application.BeginRequest += (a, b) => BeginReguest(application.Context);
        }

        private void BeginReguest(HttpContext httpContext)
        {
            string relativeUrl = httpContext.Request.Path;
            //skipping admin console requests
            if (string.Equals(relativeUrl, UrlUtils.AdminRootPath, StringComparison.OrdinalIgnoreCase) ||
                relativeUrl.StartsWith(UrlUtils.AdminRootPath + "/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            TasksPool.CheckSourcesChanges();
        }
    }
}