using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;
using System.Xml.Linq;
using Composite.Core;
using Composite.Core.Xml;
using Orckestra.Web.Css.CompileFoundation;
using static Orckestra.Web.BundlingAndMinification.Helpers;
using static Orckestra.Web.BundlingAndMinification.PageContentFilter;

namespace Orckestra.Web.BundlingAndMinification
{
    /// <summary>
    /// Class to provide scripts and styles optimization. 
    /// It provides a modification of the original document without allocating additional memory.
    /// Page elements removing (if has to) appears in the end of the <see cref="Invoke"/> call.
    /// </summary>
    internal class ActionContainer
    {
        private const string VirtualFolderTemp = "./App_Data/Composite/Cache/Css";
        private static readonly string _physicalFolderTemp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VirtualFolderTemp);

        private object _tempLocker = new object();

        static ActionContainer()
        {
            if (!Directory.Exists(_physicalFolderTemp))
            {
                Directory.CreateDirectory(_physicalFolderTemp);
            }
        }

        internal ActionContainer(XhtmlDocument document)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));

            //Removing empty subset if it exists since it causes problems in some browsers
            XDocumentType doctype = Document.Document.DocumentType;
            if (doctype?.InternalSubset?.Trim() == string.Empty)
            {
                doctype.InternalSubset = null;
            }
        }
        private XhtmlDocument Document { get; set; }
        private HashSet<string> StylesPaths { get; set; }
        private List<Action> StylesActions { get; set; }
        private HashSet<string> ScriptsPaths { get; set; }
        private List<Action> ScriptsActions { get; set; }

        internal void Invoke()
        {
            if (BundleMinifyScripts) InvokeScripts();
            if (BundleMinifyStyles) InvokeStyles();
        }

        private void InvokeScripts()
        {
            List<XElement> scriptElements = Document.Descendants(Namespaces.Xhtml + "script").Concat(Document.Descendants("script")).ToList();
            if (!scriptElements.Any()) { return; }

            ScriptsPaths = new HashSet<string>();
            ScriptsActions = new List<Action>();

            PreProcessElements(scriptElements, ActionType.Script);

            if (ScriptsPaths.Any())
            {
                Bundle bundle = null;
                try
                {
                    string pathKey = string.Join(string.Empty, ScriptsPaths).GetMD5Hash();
                    string bundleVirtualPath = "~/Bundles/Scripts_" + pathKey;
                    bundle = BundleTable.Bundles.GetBundleFor(bundleVirtualPath);
                    if (bundle == null)
                    {
                        bundle = new ScriptBundle(bundleVirtualPath);
                        bundle.Include(ScriptsPaths.ToArray());
                        BundleTable.Bundles.Add(bundle);
                    }
                    string bundlerUrl = BundleTable.Bundles.ResolveBundleUrl(bundleVirtualPath);
                    if (bundlerUrl != null && !bundlerUrl.EndsWith($"{pathKey}?v="))
                    {
                        XElement bundleXElement =
                            new XElement(Namespaces.Xhtml + "script", string.Empty,
                            new XAttribute("src", bundlerUrl),
                            new XAttribute("type", "text/javascript"));

                        Document.Body.Add(bundleXElement);
                    }
                }
                catch (Exception ex)
                {
                    if (bundle != null)
                    {
                        BundleTable.Bundles.Remove(bundle);
                    }
                    Log.LogWarning("Cannot bundle and minify scripts", ex);
                    return;
                }
            }
            //Removing elements from the original document if no exceptions appear before
            foreach (var el in ScriptsActions)
            {
                el.Invoke();
            }
        }

        private void InvokeStyles()
        {
            List<XElement> styleElements = (from obj in Document.Descendants(Namespaces.Xhtml + "link")
                                            let rel = obj.Attribute("rel")?.Value
                                            let href = obj.Attribute("href")
                                            let path = href is null ? null : Path.GetExtension((string)href)
                                            where path != null && rel == "stylesheet"
                                            select obj).Concat(Document.Descendants(Namespaces.Xhtml + "style")).ToList();

            if (!styleElements.Any()) { return; }

            StylesPaths = new HashSet<string>();
            StylesActions = new List<Action>();

            PreProcessElements(styleElements, ActionType.Style);

            if (StylesPaths.Any())
            {
                Bundle bundle = null;
                try
                {
                    string pathKey = string.Join(string.Empty, StylesPaths).GetMD5Hash();
                    string bundleVirtualPath = "~/Bundles/Styles_" + pathKey;
                    bundle = BundleTable.Bundles.GetBundleFor(bundleVirtualPath);

                    if (bundle == null)
                    {
                        bundle = new CustomStyleBundle(bundleVirtualPath);
                        List<ICssCompiler> cssCompilers = ServiceLocator.GetServices<ICssCompiler>().ToList();
                        HashSet<string> notSupportedExtentions = new HashSet<string>();
                        foreach (string source in StylesPaths)
                        {
                            string extention = Path.GetExtension(source);
                            if (extention.Equals(".css", StringComparison.OrdinalIgnoreCase))
                            {
                                var tempPath = PrepareTempFile(source);
                                bundle.Include(tempPath);
                            }
                            else if (notSupportedExtentions.Contains(extention))
                            {
                                continue;
                            }
                            else
                            {
                                ICssCompiler compiler = cssCompilers.Find(x => x.SupportsExtension(extention));
                                if (compiler == null)
                                {
                                    notSupportedExtentions.Add(extention);
                                }
                                else
                                {
                                    var compiledFilePath = compiler.CompileCss(source);
                                    var tempPath = PrepareTempFile(compiledFilePath);
                                    bundle.Include(tempPath);
                                }
                            }
                        }
                        if (notSupportedExtentions.Any())
                        {
                            string message = $"Styles with the following extensions have not available CSS decompilers: {string.Join(" ", notSupportedExtentions)}.";
                            Log.LogError(nameof(ActionContainer), message);
                        }
                        BundleTable.Bundles.Add(bundle);
                    }

                    string bundlerUrl = BundleTable.Bundles.ResolveBundleUrl(bundleVirtualPath);
                    if (bundlerUrl != null && !bundlerUrl.EndsWith($"{pathKey}?v="))
                    {
                        XElement bundleXElement =
                            new XElement(Namespaces.Xhtml + "link", string.Empty,
                            new XAttribute("href", bundlerUrl),
                            new XAttribute("type", "text/css"),
                            new XAttribute("rel", "stylesheet"));

                        Document.Head.Add(bundleXElement);
                    }
                }
                catch (Exception ex)
                {
                    if (bundle != null)
                    {
                        BundleTable.Bundles.Remove(bundle);
                    }
                    Log.LogWarning("Cannot bundle and minify styles", ex);
                    return;
                }
            }

            foreach (var el in StylesActions)
            {
                el.Invoke();
            }
        }

        private void PreProcessElements(List<XElement> collection, ActionType actionType)
        {
            string attributeName;
            HashSet<string> paths;
            List<Action> actions;

            if (actionType == ActionType.Script)
            {
                attributeName = "src";
                paths = ScriptsPaths;
                actions = ScriptsActions;
            }
            else if (actionType == ActionType.Style)
            {
                attributeName = "href";
                paths = StylesPaths;
                actions = StylesActions;
            }
            else
            {
                throw new ArgumentException($"Value of {nameof(actionType)} cannot be {actionType.ToString()}", nameof(actionType));
            }

            HashSet<string> elementIds = new HashSet<string>();
            HashSet<string> inlineElementHashes = new HashSet<string>();

            foreach (XElement element in collection)
            {
                XAttribute notBundleMinifyAttr = element.Attribute("c1-not-bundleminify");
                if (notBundleMinifyAttr != null)
                {
                    //Scheduling removing c1-not-bundleminify attribute
                    actions.Add(() => notBundleMinifyAttr.Remove());
                    if (notBundleMinifyAttr.Value.Equals("true", StringComparison.OrdinalIgnoreCase)) { continue; }
                }

                string elementId = element.Attribute("id")?.Value;
                string urlVal = element.Attribute(attributeName)?.Value;

                if (elementIds.Contains(elementId) || paths.Contains(urlVal))
                {
                    //Duplicated reference script, scheduling removing
                    actions.Add(() => element.Remove());
                    continue;
                }
                else if (string.IsNullOrEmpty(urlVal))
                {
                    string hash = element.ToString().GetMD5Hash();
                    if (inlineElementHashes.Contains(hash))
                    {
                        //Duplicated inline script, scheduling removing
                        actions.Add(() => element.Remove());
                        continue;
                    }
                    //TODO: minify-cache inline page elements values
                    inlineElementHashes.Add(hash);
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
                    paths.Add(virtualPath);
                }
            }
        }

        private string PrepareTempFile(string virtualPathOrigin)
        {
            string physicalPathOrigin = HostingEnvironment.MapPath(virtualPathOrigin);
            FileInfo physicalFileInfoOrigin = new FileInfo(physicalPathOrigin);

            string fileNameTemp = string.Concat(virtualPathOrigin.GetMD5Hash(), ".css");

            string physicalPathTemp = Path.Combine(_physicalFolderTemp, fileNameTemp);
            FileInfo physicalFileInfoTemp = null;

            string virtualPathTemp = $"~/{VirtualFolderTemp}/{fileNameTemp}"; 

            if (File.Exists(physicalPathTemp))
            {
                physicalFileInfoTemp = new FileInfo(physicalPathTemp);
            }

            if (physicalFileInfoTemp != null 
                && physicalFileInfoTemp.Exists 
                && physicalFileInfoOrigin.LastWriteTimeUtc == physicalFileInfoTemp.LastWriteTimeUtc) return virtualPathTemp;

            lock (_tempLocker)
            {
                if (physicalFileInfoTemp != null 
                    && physicalFileInfoTemp.Exists 
                    && physicalFileInfoOrigin.LastWriteTimeUtc == physicalFileInfoTemp.LastWriteTimeUtc) return virtualPathTemp;
                 
                var pattern = "url\\s*\\((?!\\s*['\"]?\\s*(?:data|http|\\/|.\\s*\\/)\\s*['\"]?)\\s*['\"]?\\s*(.+?)\\s*['\"]?\\s*\\)";

                string input = File.ReadAllText(physicalPathOrigin);

                string virtualDirectoryOrigin = Path.GetDirectoryName(virtualPathOrigin);

                input = Regex.Replace(input, 
                    pattern, 
                    m=> m.Value.Replace(m.Groups[1].Value, 
                    VirtualPathUtility.ToAbsolute(Path.Combine(virtualDirectoryOrigin, m.Groups[1].Value))));

                File.WriteAllText(physicalPathTemp, input);
                File.SetLastWriteTimeUtc(physicalPathTemp, physicalFileInfoOrigin.LastWriteTimeUtc);
            }
            return virtualPathTemp;
        }

        internal enum ActionType
        {
            Style,
            Script,
        }
    }
}