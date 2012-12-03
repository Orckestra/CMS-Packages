using System.Web.Routing;
using System.Web.Mvc;
using Composite.Core;
using Composite.Core.Application;
using Composite.Core.Routing;

namespace Composite.AspNet.MvcPlayer
{
	/// <summary>
	/// Initializer for Routes
	/// </summary>
	[ApplicationStartup]
	public class MvcPlayerRoutes
	{

		public static void OnBeforeInitialize()
		{
            Routes.OnAfterPageRouteAdded += RegisterRoutes;
            RegisterRoutes(MvcPlayerRouteTable.Routes);
		}

		public static void OnInitialized()
		{
		}

		private static void RegisterRoutes(RouteCollection routes)
		{
            if (routes["Default"] != null)
            {
                Log.LogWarning(typeof (MvcPlayerRoutes).Name, "The 'Default' route has already been defined.");
                return;
            }

		    routes.IgnoreRoute("Composite/{*pathInfo}");
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {action = "Index", id = ""}
                );
		}

	}

	public static class MvcPlayerRouteTable
	{
		static MvcPlayerRouteTable()
		{
			Routes = new RouteCollection();
		}

		public static RouteCollection Routes { get; private set; }
	}
}