using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;
using Composite.Core;
using Composite.Core.WebClient;
using static Orckestra.Web.BundlingAndMinification.CommonValues;

namespace Orckestra.Web.BundlingAndMinification
{
    public class BundlingAndMinificationHttpModule : IHttpModule
    {
        private static readonly Regex _bundlePathRegex = 
            new Regex("^" + Regex.Escape(UrlUtils.PublicRootPath) + "/Bundles/(Scripts|Styles)_([a-zA-Z0-9]{32})?v=(.+?)$", RegexOptions.Compiled);
        public void Dispose() { }

        public void Init(HttpApplication application)
        {
            application.BeginRequest += (a, b) => context_BeginRequest(application.Context);
        }

        private void context_BeginRequest(HttpContext context)
        {
            if (!BundleMinifyStyles && !BundleMinifyStyles) return;
            var requestPath = context.Request.Path;

            var match = _bundlePathRegex.Match(requestPath);
            if (!match.Success) return;

            var hash = match.Groups[2]?.Value;
            if (string.IsNullOrWhiteSpace(hash)) return;

            ActionType actionType = match.Groups[1]?.Value == "Scripts" ? ActionType.Script : ActionType.Style;
            if ((actionType == ActionType.Script && !BundleMinifyScripts) 
                    || (actionType == ActionType.Style && !BundleMinifyStyles)) return;

            Bundle bundleObj = BundleTable.Bundles.GetBundleFor(requestPath);
            if (bundleObj != null) return;

            var bundleCacheFolderPath = HostingEnvironment.MapPath(BundlesCacheFolder);
            ProcessCache(actionType, bundleCacheFolderPath, hash);
        }

        private static void ProcessCache(ActionType actionType, string bundleCacheFolderPath, string hash)
        {
            string fileExtention = actionType == ActionType.Script ? CacheFileTypeScripts : CacheFileTypeStyles;
            string filePath;
            try
            {
                filePath = Path.Combine(bundleCacheFolderPath, $"{hash}{fileExtention}");
            }
            catch(Exception ex)
            {
                Log.LogError(AppNameForLogs, ex);
                return;
            }

            if (!File.Exists(filePath)) return;

            try
            {
                var underlyingFileRelativePaths = File.ReadAllLines(filePath);
                HashSet<string> paths = new HashSet<string>(underlyingFileRelativePaths.ToList());
                DocumentProcessor.CreateBundle(actionType, paths, out _);
            }
            catch (Exception ex)
            {
                Log.LogError(AppNameForLogs, $"Cannot read and process {filePath} file.");
                Log.LogError(AppNameForLogs, ex);
            }
            
        }
    }
}
