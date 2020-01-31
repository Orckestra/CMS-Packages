using Orckestra.Web.Typescript.Classes;
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

        private void BeginReguest(HttpContext _) => TasksPool.CheckSourcesChanges();
    }
}