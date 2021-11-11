using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Composite.Core;
using Composite.Core.Application;
using Composite.Core.Routing;
using Composite.Core.WebClient.Renderings.Page;
using Microsoft.Extensions.DependencyInjection;
using static Orckestra.Web.BundlingAndMinification.CommonValues;

namespace Orckestra.Web.BundlingAndMinification
{
    [ApplicationStartup]
    public static class StatupHandler
    {
        public static void OnBeforeInitialize()
        {
            if (!HostingEnvironment.IsHosted) return;

            CreateCacheFolder();
        }

        private static void CreateCacheFolder()
        {
            var physicalPath = HostingEnvironment.MapPath(BundlesCacheFolder);
            if (!Directory.Exists(physicalPath))
            {
                Log.LogInformation(AppNameForLogs, $"The directory {physicalPath} does not exist, trying to create one.");
                try
                {
                    Directory.CreateDirectory(physicalPath);
                }
                catch (Exception ex)
                {
                    Log.LogError(AppNameForLogs, $"Cannot create folder {physicalPath} for caching bundles.");
                    Log.LogError(AppNameForLogs, ex);
                    if (!Directory.Exists(physicalPath)) throw;
                    return;
                }
            }
        }

        public static void OnInitialized()
        {
            if (!HostingEnvironment.IsHosted) return;

            Routes.OnBeforePageRouteAdded += routeCollection => routeCollection.Ignore("Bundles/{*pathinfo}");
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IPageContentFilter), typeof(PageContentFilter));
        }
    }
}