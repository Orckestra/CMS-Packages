using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Optimization;
using System.Xml.Linq;
using Composite.Core;
using Composite.Core.Xml;
using Orckestra.Web.Css.CompileFoundation;
using static Orckestra.Web.BundlingAndMinification.Helpers;
using static Orckestra.Web.BundlingAndMinification.CommonValues;
using Orckestra.Web.BundlingAndMinification.Customizations;
using System.Web.Hosting;
using System.Web;
using System.Runtime.InteropServices;

namespace Orckestra.Web.BundlingAndMinification
{
    /// <summary>
    /// Class to provide scripts and styles optimization. 
    /// It provides a modification of the original document without allocating additional memory.
    /// Page elements removing (if has to) appears in the end of the <see cref="Execute"/> call.
    /// </summary>
    internal class DocumentProcessor
    {
        private static readonly object _styleCompilationLocker = new object();
        private static readonly object _cacheWrittingLocker = new object();

        private XhtmlDocument _document { get; set; }

        internal DocumentProcessor(XhtmlDocument document)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));

            ResetEmptySubset();
        }

        private void ResetEmptySubset()
        {
            //Removing empty subset if it exists since it causes problems in some browsers
            XDocumentType doctype = _document?.Document?.DocumentType;
            if (doctype?.InternalSubset?.Trim() == string.Empty)
            {
                doctype.InternalSubset = null;
            }
        }

        internal void Execute()
        {
            if (!PackageStateManager.IsCriticalState() && BundleMinifyScripts) Execute(ActionType.Script);
            if (!PackageStateManager.IsCriticalState() && BundleMinifyStyles ) Execute(ActionType.Style);
        }

        private void Execute(ActionType actionType)
        {
            PrepareDataToProcess(actionType, out List<Action> actionsToProvide, out HashSet<string> filePaths);

            CreateBundle(actionType, filePaths, out string bundleUrl);

            if (bundleUrl == null) return;

            string filePathsHash = string.Join(string.Empty, filePaths).GetMD5Hash();
            AddBundleToTheDocument(actionType, bundleUrl, filePathsHash);

            actionsToProvide.ForEach(x => x.Invoke());
        }

        private void PrepareDataToProcess(ActionType actionType, out List<Action> actions, out HashSet<string> pageFilePaths)
        {
            List<XElement> collection = GetXElementsFromDocument(actionType);
            string attributeName;
            pageFilePaths = new HashSet<string>();
            actions = new List<Action>();

            attributeName = actionType == ActionType.Script ? "src" : "href";

            HashSet<string> elementIds = new HashSet<string>();
            HashSet<string> inlineElementHashes = new HashSet<string>();

            foreach (XElement element in collection)
            {
                XAttribute notBundleMinifyAttr = element.Attribute("c1-not-bundleminify");
                if (notBundleMinifyAttr != null)
                {
                    //Scheduling removing c1-not-bundleminify attribute from the element
                    actions.Add(() => notBundleMinifyAttr.Remove());
                    if (notBundleMinifyAttr.Value.Equals("true", StringComparison.OrdinalIgnoreCase)) continue;
                }

                string elementId = element.Attribute("id")?.Value;
                string urlVal = element.Attribute(attributeName)?.Value;

                //Check if it is a duplicate by id or url
                if (elementIds.Contains(elementId) || pageFilePaths.Contains(urlVal))
                {
                    //Duplicated reference script, scheduling removing
                    actions.Add(() => element.Remove());
                    continue;
                }
                
                if (string.IsNullOrEmpty(urlVal))
                {
                    //This is an inline element, do nothing with it but check for dublicates
                    string hash = element.ToString().GetMD5Hash();
                    if (inlineElementHashes.Contains(hash))
                    {
                        //Duplicated element, scheduling removing
                        actions.Add(() => element.Remove());
                    }
                    else
                    {
                        inlineElementHashes.Add(hash);
                    }
                    continue;
                }

                if (elementId != null)
                {
                    elementIds.Add(elementId);
                }

                string virtualPath = GetVirtualPath(urlVal);
                if (!string.IsNullOrEmpty(virtualPath))
                {
                    //Original reference of an internal file script, scheduling removing since it will be in the bundle
                    actions.Add(() => element.Remove());
                    pageFilePaths.Add(virtualPath);
                }
            }
        }

        private List<XElement> GetXElementsFromDocument(ActionType actionType)
        {
            return actionType == ActionType.Script
                ? _document.Descendants(Namespaces.Xhtml + "script").Concat(_document.Descendants("script")).ToList()
                : (from obj in _document.Descendants(Namespaces.Xhtml + "link")
                   let rel = obj.Attribute("rel")?.Value
                   let href = obj.Attribute("href")
                   let path = href is null ? null : Path.GetExtension((string)href)
                   where path != null && rel == "stylesheet"
                   select obj).Concat(_document.Descendants(Namespaces.Xhtml + "style")).ToList();
        }

        internal static void CreateBundle(ActionType actionType, HashSet<string> pageFilePaths, out string bundleUrl)
        {
            bundleUrl = null;
            if (pageFilePaths == null || !pageFilePaths.Any()) return;

            try
            {
                if (actionType == ActionType.Script)
                {
                    CreateScriptsBundle(pageFilePaths, out bundleUrl);
                }
                else
                {
                    CreateStylesBundle(pageFilePaths, out bundleUrl);
                }
            }
            catch (Exception ex)
            {
                Log.LogError($"Cannot bundle and minify {actionType}", ex);
                PackageStateManager.SetCriticalState();
                return;
            }
        }

        private static void CreateScriptsBundle(HashSet<string> pageFilePaths, out string bundleUrl)
        {
            string hash = string.Join(string.Empty, pageFilePaths).GetMD5Hash();
            string bundleVirtualPath = $"~/Bundles/{BundlePathPartScripts}" + hash;
            Bundle bundleObj = BundleTable.Bundles.GetBundleFor(bundleVirtualPath);
            bundleUrl = null;
            bool initialRunning = HttpContext.Current?.Handler == null;

            if (bundleObj == null)
            {
                var pathsAreValidated = ValidateFilePaths(pageFilePaths);
                if (!pathsAreValidated)
                {
                    return;
                }
                bundleObj = new ScriptBundle(bundleVirtualPath);
                bundleObj.Include(pageFilePaths.ToArray());
                BundleTable.Bundles.Add(bundleObj);
                if (!initialRunning)
                {
                    SaveBundleToCache(ActionType.Script, pageFilePaths, hash);
                }
            }
            if (!initialRunning)
            {
                bundleUrl = BundleTable.Bundles.ResolveBundleUrl(bundleVirtualPath);
            }
        }

        private static void CreateStylesBundle(HashSet<string> pageFilePaths, out string bundleUrl)
        {
            string hash = string.Join(string.Empty, pageFilePaths).GetMD5Hash();
            string bundleVirtualPath = $"~/Bundles/{BundlePathPartStyles}" + hash;
            CustomStyleBundle bundleObj = (CustomStyleBundle)BundleTable.Bundles.GetBundleFor(bundleVirtualPath);
            bundleUrl = null;
            bool initialRunning = HttpContext.Current?.Handler == null;

            if (bundleObj == null)
            {
                var pathsAreValidated = ValidateFilePaths(pageFilePaths);
                if (!pathsAreValidated)
                {
                    return;
                }
                bundleObj = new CustomStyleBundle(bundleVirtualPath);
                
                HashSet<string> notSupportedExtentions = new HashSet<string>();
                List<ICssCompiler> _cssCompilers = ServiceLocator.GetServices<ICssCompiler>().ToList();
                foreach (string source in pageFilePaths)
                {
                    string extention = Path.GetExtension(source);

                    if (extention.Equals(".css", StringComparison.OrdinalIgnoreCase))
                    {
                        bundleObj.Include(source);
                        continue;
                    }
                    else if (notSupportedExtentions.Contains(extention))
                    {
                        continue;
                    }

                    ICssCompiler compiler = _cssCompilers.Find(x => x.SupportsExtension(extention));
                    if (compiler == null)
                    {
                        notSupportedExtentions.Add(extention);
                        continue;
                    }

                    string compiledStylePath = CompileStyle(source, compiler);

                    if (string.IsNullOrWhiteSpace(compiledStylePath))
                    {   
                        //Saas/less compilers use filesystem, so it could be multiinstance issue here.
                        Log.LogWarning(AppNameForLogs, $"Could not compile styles for the source {source}");
                        return;
                    }
                    bundleObj.Include(compiledStylePath);
                }

                if (notSupportedExtentions.Any())
                {
                    string message = $"Styles with the following extensions have not available CSS decompilers: " +
                                        $"{string.Join(" ", notSupportedExtentions)}.";
                    Log.LogError(AppNameForLogs, message);
                    PackageStateManager.SetCriticalState();
                    return;
                }
                BundleTable.Bundles.Add(bundleObj);
                if (!initialRunning)
                {
                    SaveBundleToCache(ActionType.Style, pageFilePaths, hash);
                }
            }
            if (!initialRunning)
            {
                bundleUrl = BundleTable.Bundles.ResolveBundleUrl(bundleVirtualPath);
            }
        }

        private static string CompileStyle(string source, ICssCompiler compiler)
        {
            lock (_styleCompilationLocker)
            {
                try
                {
                    return compiler.CompileCss(source);
                }
                catch (Exception ex)
                {
                    Log.LogError(AppNameForLogs, ex);
                    PackageStateManager.SetCriticalState();
                    return null;
                }
            }
        }

        private static void SaveBundleToCache(ActionType actionType, HashSet<string> pageFilePaths, string hash)
        {
            string fileType = actionType == ActionType.Script ? CacheFileTypeScripts : CacheFileTypeStyles;
            var physicalPath = HostingEnvironment.MapPath(BundlesCacheFolder);
            lock (_cacheWrittingLocker)
            {
                try
                {
                    var filePath = $"{physicalPath}/{hash}{fileType}";
                    File.WriteAllLines(filePath, pageFilePaths);
                }
                catch (Exception ex)
                {
                    Log.LogError(AppNameForLogs, ex);
                    PackageStateManager.SetCriticalState();
                }
            }
        }

        private void AddBundleToTheDocument(ActionType actionType, string bundleUrl, string hash)
        {
            if (_document == null || bundleUrl == null || hash == null || bundleUrl.EndsWith($"{hash}?v=")) return;

            if (actionType == ActionType.Script)
            {
                XElement bundleXElement =
                    new XElement(Namespaces.Xhtml + "script", string.Empty,
                    new XAttribute("src", bundleUrl),
                    new XAttribute("type", "text/javascript"));

                _document.Body.Add(bundleXElement);
            }
            else
            {
                XElement bundleXElement =
                    new XElement(Namespaces.Xhtml + "link", string.Empty,
                    new XAttribute("href", bundleUrl),
                    new XAttribute("type", "text/css"),
                    new XAttribute("rel", "stylesheet"));

                // Add before the first link or at the end
                var element = _document.Head.Element(Namespaces.Xhtml + "link");
                if (element != null)
                {
                    element.AddBeforeSelf(bundleXElement);
                }
                else
                {
                    _document.Head.Add(bundleXElement);
                }
            }
        }

        

    }
}
