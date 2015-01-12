using Composite.Core.Application;
using Composite.Core.Routing;

namespace Composite.Web.BundlingAndMinification
{
    public static class StartupHandler
    {
        [ApplicationStartup]
        public static class StatupHandler
        {
            public static void OnBeforeInitialize() { }

            public static void OnInitialized()
            {
                Routes.OnBeforePageRouteAdded += routeCollection => routeCollection.Ignore("Bundles/{*pathinfo}");
            }
        }
    }
}
