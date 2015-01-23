using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Composite.C1Console.Security;
using Composite.Core;
using Composite.Core.Collections.Generic;
using Composite.Core.Parallelization;
using Composite.Core.Routing;
using Composite.Core.WebClient;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Tools.LinkChecker
{
    public class BrokenLinksReport
    {
        // private static readonly string LogTitle = typeof(BrokenLinksReport).FullName;

        private readonly XName PageElementName = "Page";


        private readonly string _serverUrl;

        private readonly ConcurrentDictionary<string, BrokenLinkType> _brokenLinks = new ConcurrentDictionary<string, BrokenLinkType>();
        private readonly ConcurrentDictionary<string, object> _hostnameSync = new ConcurrentDictionary<string, object>();

        private readonly HashSet<string> _bindedHostnames;

        public BrokenLinksReport(HttpContext context)
        {
            _serverUrl = new UrlBuilder(context.Request.Url.ToString()).ServerUrl;

            _bindedHostnames = new HashSet<string>(DataFacade.GetData<IHostnameBinding>().AsEnumerable().Select(h => h.Hostname.ToLowerInvariant()));
        }

        public bool BuildBrokenLinksReport(XElement infoDocumentRoot)
        {
            using (new DataScope(PublicationScope.Published))
            {
                bool noInvalidLinksFound = true;

                // Get all pages present in the console
                List<IPage> actionRequiredPages = DataFacade.GetData<IPage>().ToList();

                // Check security for each page (does the user have access - no need to bother the user with pages they do not have access to)
                UserToken userToken = UserValidationFacade.GetUserToken();
                var userPermissions = PermissionTypeFacade.GetUserPermissionDefinitions(userToken.Username).ToList();
                var userGroupPermissions =
                    PermissionTypeFacade.GetUserGroupPermissionDefinitions(userToken.Username).ToList();

                // Loop all pages and remove the ones the user has no access to
                actionRequiredPages = actionRequiredPages.Where(page =>
                    PermissionTypeFacade.GetCurrentPermissionTypes(userToken, page.GetDataEntityToken(), userPermissions,
                        userGroupPermissions)
                        .Contains(PermissionType.Read)).ToList();

                var pageIdsWithAccessTo = new HashSet<Guid>(actionRequiredPages.Select(p => p.Id));


                var allSitemapElements = PageStructureInfo.GetSiteMap().DescendantsAndSelf();

                var relevantElements =
                    allSitemapElements.Where(f => pageIdsWithAccessTo.Contains(new Guid(f.Attribute("Id").Value)));
                var minimalTree =
                    relevantElements.AncestorsAndSelf().Where(f => f.Name.LocalName == "Page").Distinct().ToList();

                var reportElements = new Hashtable<Guid, XElement>();

                var linksToCheck = new List<LinkToCheck>();

                // Rendering all the C1 pages and collecting links
                foreach (XElement pageElement in minimalTree)
                {
                    Guid pageId = new Guid(pageElement.Attribute("Id").Value);

                    IPage page = PageManager.GetPageById(pageId);
                    Verify.IsNotNull(page, "Failed to get the page");

                    string pageTitle = pageElement.Attribute("MenuTitle") != null
                        ? pageElement.Attribute("MenuTitle").Value
                        : pageElement.Attribute("Title").Value;

                    var resultPageElement = new XElement(PageElementName,
                        new XAttribute("Id", pageId),
                        new XAttribute("Title", pageTitle));

                    reportElements[pageId] = resultPageElement;
                    

                    string htmlDocument, errorCode;

                    string url = pageElement.Attribute("URL").Value;
                    string pageServerUrl = null;

                    if (url.StartsWith("http"))
                    {
                        pageServerUrl = new UrlBuilder(url).ServerUrl;
                        if (pageServerUrl == string.Empty) pageServerUrl = url; /* Bug in versions < C1 4.0 beta 2 */
                    }

                    PageRenderingResult result = RenderPage(url, out htmlDocument, out errorCode);
                    if (result == PageRenderingResult.Failed)
                    {
                        resultPageElement.Add(GetRenderingErrorNode(errorCode));
                        continue;
                    }

                    if (result == PageRenderingResult.Redirect || result == PageRenderingResult.NotFound)
                    {
                        continue;
                    }

                    XDocument document;
                    try
                    {
                        document = XDocument.Parse(htmlDocument);
                    }
                    catch (Exception)
                    {
                        resultPageElement.Add(GetRenderingErrorNode(Localization.BrokenLinkReport_NotValidXhml));
                        continue;
                    }

                    linksToCheck.AddRange(CollectLinksToCheck(document, resultPageElement, pageServerUrl));
                }

                linksToCheck = linksToCheck.OrderBy(o => Guid.NewGuid()).ToList(); // Shuffling links

                ParallelFacade.ForEach(linksToCheck, linkToCheck =>
                {
                    BrokenLinkType brokenLinkType;

                    if (HttpLinkIsValid(linkToCheck.Href, linkToCheck.PageServerUrl, out brokenLinkType))
                    {
                        return;
                    }

                    var brokenLinkDescriptionElement = DescribeBrokenLink(linkToCheck.LinkNode, linkToCheck.Href, brokenLinkType);

                    lock (linkToCheck.ReportPageNode)
                    {
                        linkToCheck.ReportPageNode.Add(brokenLinkDescriptionElement);
                    }
                    noInvalidLinksFound = false;
                });

                BuildReportTreeRec(infoDocumentRoot, Guid.Empty, reportElements);

                return noInvalidLinksFound;
            }
        }

        private XElement DescribeBrokenLink(XElement a, string link, BrokenLinkType brokenLinkType)
        {
            string previousnode = ToPreviewString(a.PreviousNode);
            string nextnode = ToPreviewString(a.NextNode);

            if (previousnode.Length > 50)
            {
                previousnode = "..." + previousnode.Substring(previousnode.Length - 40);
            }
            if (nextnode.Length > 50)
            {
                nextnode = nextnode.Substring(nextnode.Length - 40) + "...";
            }

            string errorText = Describe(brokenLinkType);

            return new XElement("invalidContent",
                new XAttribute("previousNode", previousnode),
                new XAttribute("originalText", a.Value),
                new XAttribute("originalLink", link),
                new XAttribute("nextNode", nextnode),
                new XAttribute("errorType", errorText));
        }

        private IEnumerable<LinkToCheck> CollectLinksToCheck(XDocument document, XElement reportElement, string pageServerUrl)
        {
            var result = new List<LinkToCheck>();

            foreach (var a in document.Descendants(Namespaces.Xhtml + "a"))
            {
                var hrefAttr = a.Attribute("href");
                if (hrefAttr == null)
                {
                    continue;
                }

                string urlStr = a.Attribute("href").Value;

                var href = HttpUtility.UrlDecode(urlStr).Trim();
                if (NotHttpLink(href))
                {
                    continue;
                }

                var item = new LinkToCheck
                {
                    Href = href,
                    LinkNode = a,
                    ReportPageNode = reportElement,
                    PageServerUrl = pageServerUrl
                };

                result.Add(item);
            }

            return result;
        } 

        private class LinkToCheck
        {
            public string Href;
            public XElement LinkNode;
            public XElement ReportPageNode;
            public string PageServerUrl;
        }



        private bool NotHttpLink(string link)
        {
            if (string.IsNullOrEmpty(link) || link.StartsWith("#")) return true;

            if (link.StartsWith("/")
                || link.StartsWith("http://")
                || link.StartsWith("https://"))
            {
                return false;
            }

            string[] parts = link.Split('/');
            return parts[0].Contains(":");
        }

        private XElement GetRenderingErrorNode(string message)
        {
            return new XElement("renderingError", new XAttribute("message", message));
        }

        private string ToPreviewString(XNode node)
        {
            if (node == null) return string.Empty;

            if (node is XElement)
            {
                var sbResult = new StringBuilder();

                foreach (var childNode in (node as XElement).Nodes())
                {
                    sbResult.Append(ToPreviewString(childNode));
                }

                return sbResult.ToString();

            }
            return node.ToString();
        }

        private void BuildReportTreeRec(XElement reportNode, Guid currentPageId, Hashtable<Guid, XElement> allNodes)
        {
            var childNodes = PageManager.GetChildrenIDs(currentPageId);

            foreach (Guid childNodeId in childNodes)
            {
                XElement element = allNodes[childNodeId];
                if (element == null) continue;

                reportNode.Add(element);

                BuildReportTreeRec(element, childNodeId, allNodes);
            }
        }

        private bool HttpLinkIsValid(string url, string serverUrl, out BrokenLinkType brokenLinkType)
        {
            BrokenLinkType cachedResult;

            if (_brokenLinks.TryGetValue(url, out cachedResult))
            {
                brokenLinkType = cachedResult;
                return brokenLinkType == BrokenLinkType.None;
            }

            // Trying to parse as a page url first
            PageUrlData pageUrlData = null;
            try
            {
                if (IsKnownHostname(url)) // Workaround "if" for early vesrions of 4.0 beta
                {
                    pageUrlData = PageUrls.ParseUrl(url);
                }
            }
            catch (UriFormatException)
            {
            }

            if (pageUrlData != null)
            {
                Guid linkedPageId = pageUrlData.PageId;

                IPage page;

                using (new DataScope(pageUrlData.PublicationScope, pageUrlData.LocalizationScope))
                {
                    page = PageManager.GetPageById(linkedPageId);
                }

                if (page == null)
                {
                    if (pageUrlData.PublicationScope == PublicationScope.Published)
                    {
                        using (new DataScope(PublicationScope.Unpublished, pageUrlData.LocalizationScope))
                        {
                            if (PageManager.GetPageById(linkedPageId) != null)
                            {
                                brokenLinkType = BrokenLinkType.PageNotPublished;
                                return SaveLinkCheckResult(url, brokenLinkType);
                            }
                        }
                    }

                    brokenLinkType = BrokenLinkType.Page;
                    return SaveLinkCheckResult(url, brokenLinkType);
                }

                // If no PathInfo - page link is already valid
                if (string.IsNullOrEmpty(pageUrlData.PathInfo))
                {
                    brokenLinkType = BrokenLinkType.None;
                    return true;
                }

                // If there's pathInfo -> making a request to check whether the link is actually broken
                brokenLinkType = ValidateByRequest(url, serverUrl, BrokenLinkType.Page);
                return brokenLinkType == BrokenLinkType.None;
            }

            MediaUrlData mediaUrlData = MediaUrls.ParseUrl(url);

            if (mediaUrlData != null)
            {
                Guid mediaId = mediaUrlData.MediaId;
                string mediastore = mediaUrlData.MediaStore;

                bool mediaExist = DataFacade.GetData<IMediaFile>().Any(f => f.StoreId == mediastore && f.Id == mediaId);

                brokenLinkType = mediaExist ? BrokenLinkType.None : BrokenLinkType.MediaLibrary;
                return SaveLinkCheckResult(url, brokenLinkType);
            }

            bool urlIsInternal = url.StartsWith("/");

            brokenLinkType = ValidateByRequest(url, serverUrl, urlIsInternal ? BrokenLinkType.Relative : BrokenLinkType.External);
            return brokenLinkType == BrokenLinkType.None;
        }

        private bool IsKnownHostname(string href)
        {
            if (!href.StartsWith("http://") && !href.StartsWith("https://"))
            {
                return true;
            }

            string absoluteUrl = href;

            // Converting links 
            // "http://localhost" to "http://localhost/"
            // "http://localhost?..." to "http://localhost/?..."
            if ((absoluteUrl.Count(c => c == '/') == 2) && absoluteUrl.Contains("//"))
            {
                int questionMarkIndex = absoluteUrl.IndexOf('?');
                if (questionMarkIndex > 0)
                {
                    absoluteUrl = absoluteUrl.Insert(questionMarkIndex, "/");
                }
                else
                {
                    absoluteUrl += "/";
                }
            }

            Uri uri = new Uri(absoluteUrl);

            string hostname = uri.DnsSafeHost;

            var context = HttpContext.Current;
            if (context != null && context.Request.Url.DnsSafeHost == hostname)
            {
                return true;
            }

            return _bindedHostnames.Contains(hostname);
        }

        private string Describe(BrokenLinkType brokenLinkType)
        {
            switch (brokenLinkType)
            {
                case BrokenLinkType.External:
                    return Localization.BrokenLink_External;
                case BrokenLinkType.MediaLibrary:
                    return Localization.BrokenLink_MediaLibrary;
                case BrokenLinkType.Page:
                    return Localization.BrokenLink_Page;
                case BrokenLinkType.PageNotPublished:
                    return Localization.BrokenLink_PageNotPublished;
                case BrokenLinkType.Relative:
                    return Localization.BrokenLink_Relative;
            }

            throw new InvalidOperationException("not supported link type " + brokenLinkType.ToString());
        }

        /// <summary>
        /// This method will check a url to see that it does not return server or protocol errors
        /// </summary>
        /// <param name="url">The path to check</param>
        /// <returns></returns>
        private BrokenLinkType ValidateByRequest(string url, string serverUrl, BrokenLinkType brokenLinkType)
        {
            if (url.StartsWith("/"))
            {
                url = UrlUtils.Combine(serverUrl ?? _serverUrl, url);
            }

            string hostname;
            try
            {
                hostname = new Uri(url).Host;
            }
            catch (UriFormatException)
            {
                return brokenLinkType;
            }

            // Allowing only one request to the same hostname at a time. Along with cache also ensures that the same url won't be requested to twice
            object hostNameSyncRoot = GetHostNameSyncObject(hostname);

            lock (hostNameSyncRoot)
            {
                BrokenLinkType cachedResult;
                if (_brokenLinks.TryGetValue(url, out cachedResult)) return cachedResult;

                bool success = HttpRequestHelper.MakeHeadRequest(url);

                var result = success ? BrokenLinkType.None : brokenLinkType;
                SaveLinkCheckResult(url, result);

                return result;
            }
        }




        private object GetHostNameSyncObject(string hostname)
        {
            hostname = hostname.ToLowerInvariant();

            return _hostnameSync.GetOrAdd(hostname, h => new object());
        }

        private bool SaveLinkCheckResult(string url, BrokenLinkType brokenLinkType)
        {
            _brokenLinks.TryAdd(url, brokenLinkType);
            
            return brokenLinkType == BrokenLinkType.None;
        }

        private enum BrokenLinkType
        {
            None = 0,
            Page = 1,
            PageNotPublished = 2,
            External = 3,
            Relative = 4,
            MediaLibrary = 5
        }

        

        private PageRenderingResult RenderPage(string url, out string responseBody, out string errorMessage)
        {
            if (!url.StartsWith("http"))
            {
                url = UrlUtils.Combine(_serverUrl, url);
            }

            // Marking the link as valid, so it won't be shown multiple places and there won't be any additional requests
            _brokenLinks.TryAdd(url, BrokenLinkType.None);

            var result = HttpRequestHelper.RenderPage(url, out responseBody, out errorMessage);

            if (result == PageRenderingResult.NotFound)
            {
                _brokenLinks.TryAdd(url, BrokenLinkType.Relative);
            }

            return result;
        }
    }
}
