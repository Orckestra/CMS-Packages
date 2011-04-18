using System.Web.Routing;
using System.Web.Mvc;
using Composite.Core.Application;

namespace Composite.AspNet.MvcPlayer
{
    /// <summary>
    /// Initializer for Routes
    /// </summary>
    [ApplicationStartup]
    public class Route
    {
        public static void OnBeforeInitialize()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        public static void OnInitialized()
        {
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("Composite/{*pathInfo}");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                    "Default",
                    "{controller}/{action}/{id}",
                    new { action = "Index", id = "" }
            );
        }
    }
}