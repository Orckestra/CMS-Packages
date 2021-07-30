using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;
using Composite.Core;
using static Orckestra.Web.BundlingAndMinification.CommonValues;

namespace Orckestra.Web.BundlingAndMinification
{
    public class BundleMinifyHttpModule : IHttpModule
    {
        private static ConcurrentBag<string> _bundlesRequests = new ConcurrentBag<string>();
        private const string _bundlePathRegex = "^/Bundles/(Scripts|Styles)_([a-zA-Z0-9]{32})?v=(.+?)$";
        public void Dispose() { }

        public void Init(HttpApplication application)
        {
            application.BeginRequest += (a, b) => context_BeginRequest(application.Context);
        }

        private void context_BeginRequest(HttpContext context)
        {
            if (!BundleMinifyStyles && !BundleMinifyStyles) return;
            var requestPath = context.Request.Path;

            var match = Regex.Match(requestPath, _bundlePathRegex);
            if (!match.Success) return;

            var hash = match.Groups[2]?.Value;
            if (string.IsNullOrWhiteSpace(hash) || _bundlesRequests.Contains(hash)) return;

            ActionType actionType = match.Groups[1]?.Value == "Scripts" ? ActionType.Script : ActionType.Style;
            if ((actionType == ActionType.Script && !BundleMinifyScripts) 
                    || (actionType == ActionType.Style && !BundleMinifyStyles)) return;

            Bundle bundleObj = BundleTable.Bundles.GetBundleFor(requestPath);
            if (bundleObj !=null)
            {
                _bundlesRequests.Add(hash);
                return;
            }

            var physicalPath = HostingEnvironment.MapPath(BundlesCacheFolder);
            ProcessCache(actionType, physicalPath);
            _bundlesRequests.Add(hash);
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
                    return;
                }
                DocumentProcessor.CreateBundle(actionType, paths, out _);
            }
        }
    }
}
