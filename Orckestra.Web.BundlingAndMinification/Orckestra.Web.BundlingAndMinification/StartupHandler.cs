using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Hosting;
using Composite.Core;
using Composite.Core.Application;
using Composite.Core.Routing;
using Composite.Core.WebClient.Renderings.Page;
using Microsoft.Extensions.DependencyInjection;
using static Orckestra.Web.BundlingAndMinification.CommonValues;
using static Orckestra.Web.BundlingAndMinification.Helpers;

namespace Orckestra.Web.BundlingAndMinification
{
    [ApplicationStartup]
    public static class StatupHandler
    {
        public static void OnBeforeInitialize()
        {
            LoadCachedBundles();
        }

        public static void OnInitialized()
        {
            Routes.OnBeforePageRouteAdded += routeCollection => routeCollection.Ignore("Bundles/{*pathinfo}");
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IPageContentFilter), typeof(PageContentFilter));
        }

        private static void LoadCachedBundles()
        {
            var physicalPath = HostingEnvironment.MapPath(BundlesCacheFolder);
            if (!Directory.Exists(physicalPath))
            {
                Log.LogInformation(AppNameForLogs, $"The directory {physicalPath} does not exist, trying to create one.");
                try
                {
                    Directory.CreateDirectory(physicalPath);
                }
                catch(Exception ex)
                {
                    //second validation if directory exist
                    Log.LogError(AppNameForLogs, $"Cannot create folder {physicalPath} for caching bundles.");
                    Log.LogError(AppNameForLogs, ex);
                    return;
                }
            }
            if (BundleMinifyScripts)
            {
                ProcessCache(ActionType.Script, physicalPath);
            }
            if (BundleMinifyStyles && !PackageStateManager.IsCriticalState())
            {
                ProcessCache(ActionType.Style, physicalPath);
            }
        }

        private static void ProcessCache(ActionType actionType, string physicalPath)
        {
            string[] bundleCacheFiles;
            string searchMasc = actionType == ActionType.Script ? $"*{CacheFileTypeScripts}" : $"*{CacheFileTypeStyles}";
            try
            {
                bundleCacheFiles = Directory.GetFiles(physicalPath, searchMasc);
            }
            catch (Exception ex)
            {
                Log.LogError(AppNameForLogs, $"Cannot get {actionType} cached bundles from folder {physicalPath}.");
                Log.LogError(AppNameForLogs, ex);
                PackageStateManager.SetCriticalState();
                return;
            }

            foreach (var bundleFile in bundleCacheFiles)
            {
                HashSet<string> paths = null;
                try
                {
                    var md5Hash = Path.GetFileName(bundleFile);
                    var underlyingFileRelativePaths = File.ReadAllLines(bundleFile);
                    paths = new HashSet<string>(underlyingFileRelativePaths.ToList());
                }
                catch (Exception ex)
                {
                    Log.LogError(AppNameForLogs, $"Cannot read and process {bundleFile} file.");
                    Log.LogError(AppNameForLogs, ex);
                    PackageStateManager.SetCriticalState();
                    return;
                }
                DocumentProcessor.CreateBundle(actionType, paths, out _);
            }
        }
    }
}