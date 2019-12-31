using System.Web.Http;
using System.Web.Mvc;

namespace Composite.Web.Js.TypeScript.Tester
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            StartupHandler.OnBeforeInitialize();
            
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            
            StartupHandler.OnInitialized();
        }
    }
}
