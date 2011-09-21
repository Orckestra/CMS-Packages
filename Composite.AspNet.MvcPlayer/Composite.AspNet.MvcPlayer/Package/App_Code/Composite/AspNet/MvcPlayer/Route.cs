using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Composite.Core.Application;

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
			RegisterRoutes(RouteTable.Routes);
			RegisterRoutes(MvcPlayerRouteTable.Routes);
		}

		public static void OnInitialized()
		{

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

	public static class MvcPlayerRouteTable
	{
		static MvcPlayerRouteTable()
		{
			Routes = new RouteCollection();
		}

		public static RouteCollection Routes { get; private set; }
	}
}