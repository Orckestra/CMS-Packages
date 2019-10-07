using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using Composite.Core.Application;

namespace Composite.AspNet.WebAPI
{
    [ApplicationStartup]
    public static class StartupHandler
    {
        public static void OnBeforeInitialize()
        {
        }

        public static void OnInitialized()
        {
            GlobalConfiguration.Configure(WebApiRegister);
            GlobalConfiguration.Configuration.Formatters.Insert(0, new CustomIDataXmlFormatter());
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("type", "json", new MediaTypeHeaderValue("application/json")));
        }

        public static void WebApiRegister(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            JsonIDataSerialization.WrapJsonContentResolver(config);
        }
    }
}
