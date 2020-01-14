using Composite.Core;
using Composite.Core.Xml;
using Orckestra.Web.Css.CompileFoundation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Optimization;
using System.Xml.Linq;
using static Orckestra.Web.BundlingAndMinification.Helpers;

namespace Orckestra.Web.BundlingAndMinification
{
    /// <summary>
    /// Class to provide web page scripts and styles bundling and minification
    /// </summary>
    internal static class OptimizationManager
    {
        #region scripts
        internal static void OptimizeDocumentScripts(XhtmlDocument page)
        {
            GetScriptsToOptimize(page, out SortedSet<string> internalRefScripts);
            if (internalRefScripts.Any())
            {
                OptimizeInternalRefScripts(page, internalRefScripts);
            }
        }

        private static void GetScriptsToOptimize(XhtmlDocument page, out SortedSet<string> internalRefScripts)
        {
            List<XElement> scriptElements = page.Descendants(Namespaces.Xhtml + "script").Concat(page.Descendants("script")).ToList();
            ProcessElements(page.Body, scriptElements, "src", out internalRefScripts);
        }

        private static void OptimizeInternalRefScripts(XhtmlDocument page, SortedSet<string> internalRefScripts)
        {
            //Using MD5 hash to avoid collisions which will be pretty critical here and in other usage places
            string pathKey = string.Join(string.Empty, internalRefScripts.ToArray()).GetMD5Hash();
            string bundleVirtualPath = "~/Bundles/Scripts_" + pathKey;
            Bundle bundle = BundleTable.Bundles.GetBundleFor(bundleVirtualPath);
            if (bundle is null)
            {
                bundle = new ScriptBundle(bundleVirtualPath);
                foreach (string source in internalRefScripts)
                {
                    bundle.Include(source);
                }
                BundleTable.Bundles.Add(bundle);
            }
            string bundlerUrl = BundleTable.Bundles.ResolveBundleUrl(bundleVirtualPath);
            if (bundlerUrl is null || bundlerUrl.EndsWith($"{pathKey}?v="))
            {
                //empty bundle, nothing to add to the page, but still keep bundle in bundletable not to recreate it every time
                return;
            }
            //string.Empty to force closing tag creation
            XElement bundleXElement =
                new XElement(Namespaces.Xhtml + "script", string.Empty,
                new XAttribute("src", bundlerUrl),
                new XAttribute("type", "text/javascript"));
            page.Body.Add(bundleXElement);
        }
        #endregion scripts

        #region styles
        internal static void OptimizeDocumentStyles(XhtmlDocument page)
        {
            GetStylesToOptimize(page, out SortedSet<string> internalRefStyles);
            if (internalRefStyles.Any())
            {
                OptimizeInternalRefStyles(page, internalRefStyles);
            }
        }
        private static void GetStylesToOptimize(XhtmlDocument page, out SortedSet<string> internalRefStyles)
        {
            List<XElement> styleElements = (from obj in page.Descendants(Namespaces.Xhtml + "link")
                                            let rel = obj.Attribute("rel")?.Value
                                            let href = obj.Attribute("href")
                                            let path = href is null ? null : Path.GetExtension((string)href)
                                            where
                                            path != null && rel != null && rel == "stylesheet"
                                            select obj)
                                            .Concat(page.Descendants(Namespaces.Xhtml + "style")).ToList();
            ProcessElements(page.Head, styleElements, "href", out internalRefStyles);
        }

        private static void OptimizeInternalRefStyles(XhtmlDocument page, SortedSet<string> internalRefStyles)
        {
            string pathKey = string.Join(string.Empty, internalRefStyles.ToArray()).GetMD5Hash();
            string bundleVirtualPath = "~/Bundles/Styles_" + pathKey;
            Bundle bundle = BundleTable.Bundles.GetBundleFor(bundleVirtualPath);
            if (bundle is null)
            {
                bundle = new StyleBundle(bundleVirtualPath);
                List<ICssCompiler> cssCompilers = ServiceLocator.GetServices<ICssCompiler>().ToList();
                List<string> notSupportedExtentions = new List<string>();
                foreach (string source in internalRefStyles)
                {
                    string extention = Path.GetExtension(source);
                    if (extention == ".css")
                    {
                        bundle.Include(source, new CssRewriteUrlTransform());
                    }
                    else if (notSupportedExtentions.Contains(extention))
                    {
                        continue;
                    }
                    else
                    {
                        ICssCompiler compiler = cssCompilers.FirstOrDefault(x => x.SupportsExtension(extention));
                        if (compiler is null)
                        {
                            notSupportedExtentions.Add(extention);
                        }
                        else
                        {
                            bundle.Include(compiler.CompileCss(source), new CssRewriteUrlTransform());
                        }
                    }
                }
                if (notSupportedExtentions.Any())
                {
                    Log.LogError(nameof(OptimizationManager), $"There are no available css compilers for " +
                        $"{string.Join(";", notSupportedExtentions.OrderBy(x => x))} extention(s), these styles will not work");
                }
                BundleTable.Bundles.Add(bundle);
            }
            string bundlerUrl = BundleTable.Bundles.ResolveBundleUrl(bundleVirtualPath);
            if (bundlerUrl is null || bundlerUrl.EndsWith($"{pathKey}?v="))
            {
                return;
            }
            XElement bundleXElement = new XElement(Namespaces.Xhtml + "link", string.Empty,
                                        new XAttribute("href", bundlerUrl),
                                        new XAttribute("type", "text/css"),
                                        new XAttribute("rel", "stylesheet"));
            page.Head.Add(bundleXElement);
        }
        #endregion styles

        private static void ProcessElements(
            XElement insertArea,
            List<XElement> collection,
            string urlAttributeName,
            out SortedSet<string> internalRefElements)
        {
            List<string> ids = new List<string>();
            //to control dublicated inline elements
            HashSet<string> npHashes = new HashSet<string>();
            //sorted coll. not to create new bundles in case styles or scripts on page will be the same but in diff. orders 
            internalRefElements = new SortedSet<string>();

            foreach (XElement element in collection)
            {
                XAttribute noBundleMinifyAttr = element.Attribute("c1-not-bundleminify");
                if (noBundleMinifyAttr != null)
                {
                    noBundleMinifyAttr.Remove();
                    //It seems to be better to check-use attribute values, but can also skip this part
                    if (noBundleMinifyAttr.Value == "true")
                    {
                        continue;
                    }
                }

                string id = (string)element.Attribute("id");
                string urlVal = (string)element.Attribute(urlAttributeName);
                element.Remove();

                if (ids.Any(f => f == id) || internalRefElements.Any(f => f == urlVal))
                {
                    continue;
                }
                else if (string.IsNullOrEmpty(urlVal))
                {
                    /* Adding inline scripts and styles as is, in appropriate places, but potentially 
                    they also can be minimized with Microsoft Minifier and then cashed by keys*/
                    string hash = element.ToString().GetMD5Hash();
                    if (!npHashes.Contains(hash))
                    {
                        insertArea.Add(element);
                        npHashes.Add(hash);
                    }
                    continue;
                }

                if (id != null)
                {
                    ids.Add(id);
                }
                string virtualPath = GetVirtualPath(urlVal);
                if (!string.IsNullOrEmpty(virtualPath))
                {
                    internalRefElements.Add(virtualPath);
                }
                else
                {
                    insertArea.Add(element);
                }
            }
        }
    }
}