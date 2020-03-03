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
            application.BeginRequest += (a, b) => BeginRequest(application.Context);
        }

        private void BeginRequest(HttpContext httpContext)
        {
            string url = httpContext.Request.Path;
            //skipping admin console requests
            if (string.Equals(url, UrlUtils.AdminRootPath, StringComparison.OrdinalIgnoreCase)
                || url.StartsWith(UrlUtils.AdminRootPath + "/", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            TasksPool.CheckSourcesChanges();
        }
    }
}