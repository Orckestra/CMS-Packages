using Composite.Core;
using Composite.Core.Xml;
using Orckestra.Web.Css.CompileFoundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Optimization;
using System.Xml.Linq;
using static Orckestra.Web.BundlingAndMinification.Helpers;

namespace Orckestra.Web.BundlingAndMinification
{
    /// <summary>
    /// Class to provide web page scripts and styles bundling and minification, and comments removing
    /// </summary>
    internal static class OptimizationManager
    {
        /// <summary>
        /// To compress page bytes according to selected compression method
        /// </summary>
        /// <param name="bytes">Bytes to process</param>
        /// <param name="compressionMethod">Method to be used for compression</param>
        /// <returns>Compressed bytes according to selected compression method</returns>
        internal static byte[] GetCompressedBytes(byte[] bytes, DecompressionMethods compressionMethod)
        {
            if (compressionMethod == DecompressionMethods.GZip)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (GZipStream gzipStream = new GZipStream(ms, CompressionMode.Compress))
                    {
                        gzipStream.Write(bytes, 0, bytes.Length);
                    }
                    return ms.ToArray();
                }
            }
            else if (compressionMethod == DecompressionMethods.Deflate)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (DeflateStream deflateStream = new DeflateStream(ms, CompressionMode.Compress))
                    {
                        deflateStream.Write(bytes, 0, bytes.Length);
                    }
                    return ms.ToArray();
                }
            }
            return bytes;
        }

        /// <summary>
        /// To return optimized page with bundled and minificated onsite file styles and scripts
        /// </summary>
        /// <param name="bytes">Original page bytes</param>
        /// <param name="encoding">Encoding used by server to return a page</param>
        /// <param name="bundleMinifyScripts">To process scripts or not</param>
        /// <param name="bundleMinifyStyles">To process styles or not</param>
        /// /// <param name="removeComments">To remove page comments or not</param>
        /// <returns>Bytes of optimized page. In case something went wrong, original bytes will be returned.</returns>
        internal static byte[] GetOptimizedBytes(
            byte[] bytes,
            Encoding encoding,
            bool bundleMinifyScripts,
            bool bundleMinifyStyles,
            bool removeComments)
        {
            XhtmlDocument page;
            try
            {
                page = XhtmlDocument.Parse(encoding.GetString(bytes));
                /* Fixing XDocument parcing error, removing empty subset declaration, 
                 * because it causes problems with styles on a page */
                XDocumentType doctype = page.Document.DocumentType;
                if (doctype.InternalSubset?.Trim() == string.Empty)
                {
                    doctype.InternalSubset = null;
                }
            }
            catch (Exception ex)
            {
                /* Risk of logs trashing by repeatable errors, maybe a good idea to take
                 * this into account on a logger level */
                Log.LogError(nameof(OptimizationManager), ex);
                return bytes;
            }

            Bundle bundle = default;

            /* Attempts to optimize scripts and styles are divided. 
             * In case some problems with scripts - at least to process styles, and visa versa */
            if (bundleMinifyScripts)
            {
                try
                {
                    /* Processing page and outing bundle object, in case something went wrong 
                     * to delete certain bundle, not to clear the whole table */
                    page = GetPageWithOptimizedScripts(page, out bundle);
                }
                catch (Exception ex)
                {
                    Log.LogError(nameof(OptimizationManager), ex);
                    if (bundle != null)
                    {
                        BundleTable.Bundles.Remove(bundle);
                    }
                }
            }

            if (bundleMinifyStyles)
            {
                try
                {
                    page = GetPageWithOptimizedStyles(page, out bundle);
                }
                catch (Exception ex)
                {
                    Log.LogError(nameof(OptimizationManager), ex);
                    if (bundle != null)
                    {
                        BundleTable.Bundles.Remove(bundle);
                    }
                }
            }

            if (removeComments)
            {
                page.DescendantNodes().OfType<XComment>().Remove();
            }

            return encoding.GetBytes(page.ToString());
        }

        #region scripts
        private static XhtmlDocument GetPageWithOptimizedScripts(XhtmlDocument page, out Bundle bundle)
        {
            XhtmlDocument newPage = new XhtmlDocument(page);

            GetScriptsToOptimize(newPage, out SortedSet<string> internalRefScripts);

            bundle = default;
            if (internalRefScripts.Any())
            {
                OptimizeInternalRefScripts(newPage, internalRefScripts, out bundle);
            }
            return newPage;
        }

        private static void GetScriptsToOptimize(XhtmlDocument page, out SortedSet<string> internalRefScripts)
        {
            List<XElement> scriptElements = page.Descendants(Namespaces.Xhtml + "script").Concat(page.Descendants("script")).ToList();
            ProcessElements(page.Body, scriptElements, "src", out internalRefScripts);
        }

        private static void OptimizeInternalRefScripts(XhtmlDocument page, SortedSet<string> internalRefScripts, out Bundle bundle)
        {
            //Using MD5 hash to avoid collisions which will be pretty critical here and in other usage places
            string pathKey = string.Join(string.Empty, internalRefScripts.ToArray()).GetMD5Hash();
            string bundleVirtualPath = "~/Bundles/Scripts_" + pathKey;
            bundle = BundleTable.Bundles.GetBundleFor(bundleVirtualPath);
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
        private static XhtmlDocument GetPageWithOptimizedStyles(XhtmlDocument page, out Bundle bundle)
        {
            XhtmlDocument newPage = new XhtmlDocument(page);

            GetStylesToOptimize(newPage, out SortedSet<string> internalRefStyles);

            bundle = default;
            if (internalRefStyles.Any())
            {
                OptimizeInternalRefStyles(newPage, internalRefStyles, out bundle);
            }
            return newPage;
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

        private static void OptimizeInternalRefStyles(XhtmlDocument page, SortedSet<string> internalRefStyles, out Bundle bundle)
        {
            string pathKey = string.Join(string.Empty, internalRefStyles.ToArray()).GetMD5Hash();
            string bundleVirtualPath = "~/Bundles/Styles_" + pathKey;
            bundle = BundleTable.Bundles.GetBundleFor(bundleVirtualPath);
            if (bundle is null)
            {
                bundle = new StyleBundle(bundleVirtualPath);
                List<ICssCompiler> cssCompilers = ServiceLocator.GetServices<ICssCompiler>().ToList();

                List<string> notSupportedExtentions = new List<string>();
                foreach (string source in internalRefStyles)
                {
                    string extention = Path.GetExtension(source);
                    if (notSupportedExtentions.Contains(extention))
                    {
                        continue;
                    }
                    else if (extention == ".css")
                    {
                        bundle.Include(source, new CssRewriteUrlTransform());
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
                    Log.LogError(nameof(OptimizationManager),
                        $"There are no available css compilers for " +
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
            //not to create new bundles in case styles or scripts on page will be the same but in diff. orders 
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