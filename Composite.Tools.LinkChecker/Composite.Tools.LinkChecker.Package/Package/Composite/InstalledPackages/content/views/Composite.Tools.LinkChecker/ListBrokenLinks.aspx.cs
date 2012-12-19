using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Composite;
using Composite.C1Console.Security;
using Composite.C1Console.Users;
using Composite.Core;
using Composite.Core.Collections.Generic;
using Composite.Core.Parallelization;
using Composite.Core.ResourceSystem;
using Composite.Core.WebClient;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Data.Types;
using Composite.Core.Routing;

/// <summary>
/// Linkchecker. Searches for broken links within the composite system
/// Based on contribution by JamBo - nu.Faqtz.com
/// </summary>
public partial class ListBrokenLinks : System.Web.UI.Page
{
    private const int PageRenderingTimeout = 5000;
    private const int UrlCheckingTimeout = 5000;
    private static readonly string XsltFileName = "ListBrokenLinks.xslt";

    private readonly Hashtable<string, BrokenLinkType?> _brokenLinks = new Hashtable<string, BrokenLinkType?>();
    private readonly XName PageElementName = "Page";

    private static readonly string LogTitle = "LinkChecker";

    private readonly Hashtable<string, object> _hostnameSync = new Hashtable<string, object>(); 

    protected void Page_Load(object sender, EventArgs e)
    {
        using (new DataScope(DataScopeIdentifier.Administrated, UserSettings.ActiveLocaleCultureInfo))
        {
            XElement infoDocumentRoot = new XElement("ActionItems");

            bool noBrokenLinks;

            int tickCount = Environment.TickCount;
            using (new DataScope(PublicationScope.Published))
            {
                noBrokenLinks = BuildBrokenLinksReport(infoDocumentRoot);
            }

            Log.LogInformation(LogTitle, "Time spent: " + (Environment.TickCount - tickCount) + " ms");

            if (noBrokenLinks)
            {
                emptyLabelPlaceHolder.Visible = true;
                return;
            }
            
            XDocument newTree = TransformMarkup(infoDocumentRoot);
                
            visualOutput.Controls.Add(new LiteralControl(newTree.ToString()));
        }
    }

    private enum PageRenderingResult
    {
        Failed = 0,
        Successful = 1,
        Redirect = 2,
        NotFound = 3
    }

    private PageRenderingResult RenderPage(string url, out string responseBody, out string errorMessage)
    {
        if(!url.StartsWith("http"))
        {
            url = UrlUtils.Combine(ServerUrl, url);
        }

        // Marking the link as valid, so it won't be shown multiple places and there won't be any additional requests
        lock(_brokenLinks)
        {
            _brokenLinks[url] = BrokenLinkType.None;
        }

        try
        {
            var request = WebRequest.Create(url) as HttpWebRequest;

            request.UserAgent = @"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5";
            request.Timeout = PageRenderingTimeout;
            request.AllowAutoRedirect = false; // Some pages may contain redirects to other pages/different websites
            request.Method = "GET";


            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            int statusCode = (int)response.StatusCode;

            if (statusCode == 200)
            {
                using (var responseStream = response.GetResponseStream())
                {
                    responseBody = new StreamReader(responseStream).ReadToEnd();
                }
                errorMessage = null;
                return PageRenderingResult.Successful;
            }

            if(statusCode == 301 || statusCode == 302)
            {
                responseBody = null;
                errorMessage = null;
                return PageRenderingResult.Redirect;
            }

            errorMessage = string.Format(GetResourceString("BrokenLinkReport.HttpStatus"), statusCode);
        }
        catch(WebException ex)
        {
            var webResponse = ex.Response as HttpWebResponse;
            if (webResponse != null && webResponse.StatusCode != HttpStatusCode.OK)
            {
                if(webResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    lock(_brokenLinks)
                    {
                        _brokenLinks[url] = BrokenLinkType.Relative;
                    }

                    errorMessage = responseBody = null;
                    return PageRenderingResult.NotFound;
                }
                errorMessage = string.Format(GetResourceString("BrokenLinkReport.HttpStatus"), (int)webResponse.StatusCode + " " + webResponse.StatusCode);
            }
            else
            {
                errorMessage = ex.ToString();
            }
        }

        responseBody = null;
        return PageRenderingResult.Failed;
    }


    private class LinkToCheck
    {
        public XElement LinkNode;
        public XElement ReportPageNode;
        public string PageServerUrl;
    }

    private bool BuildBrokenLinksReport(XElement infoDocumentRoot)
    {
        bool noInvalidLinksFound = true;
        
        // Get all pages present in the console
        List<IPage> actionRequiredPages = DataFacade.GetData<IPage>().ToList();

        // Check security for each page (does the user have access - no need to bother the user with pages they do not have access to)
        UserToken userToken = UserValidationFacade.GetUserToken();
        var userPermissions = PermissionTypeFacade.GetUserPermissionDefinitions(userToken.Username).ToList();
        var userGroupPermissions = PermissionTypeFacade.GetUserGroupPermissionDefinitions(userToken.Username).ToList();

        // Loop all pages and remove the ones the user has no access to
        actionRequiredPages = actionRequiredPages.Where(page =>
            PermissionTypeFacade.GetCurrentPermissionTypes(userToken, page.GetDataEntityToken(), userPermissions, userGroupPermissions)
                                .Contains(PermissionType.Read)).ToList();

        HashSet<Guid> pageIdsWithAccessTo = new HashSet<Guid>(actionRequiredPages.Select(p => p.Id));


        var allSitemapElements = PageStructureInfo.GetSiteMap().DescendantsAndSelf();

        var relevantElements = allSitemapElements.Where(f => pageIdsWithAccessTo.Contains(new Guid(f.Attribute("Id").Value)));
        var minimalTree = relevantElements.AncestorsAndSelf().Where(f => f.Name.LocalName == "Page").Distinct().ToList();

        Hashtable<Guid, XElement> reportElements = new Hashtable<Guid, XElement>();

        var linksToCheck = new List<LinkToCheck>();

        foreach(XElement pageElement in minimalTree)
        {
            Guid pageId = new Guid(pageElement.Attribute("Id").Value);
            
            IPage page = PageManager.GetPageById(pageId);
            Verify.IsNotNull(page, "Failed to get the page");

            string publicationStatus;

            using(new DataScope(PublicationScope.Unpublished))
            {
                var unpublishedPage = PageManager.GetPageById(pageId);
                publicationStatus = unpublishedPage != null ? unpublishedPage.PublicationStatus : "published";
            }

            string pageTitle = pageElement.Attribute("MenuTitle") != null
                                ? pageElement.Attribute("MenuTitle").Value
                                : pageElement.Attribute("Title").Value;
 
            XElement resultPageElement = new XElement(PageElementName,
                                               new XAttribute("Id", pageId),
                                               new XAttribute("Title", pageTitle),
                                               new XAttribute("Status", publicationStatus));

            lock (reportElements)
            {
                reportElements[pageId] = resultPageElement;
            }

            string htmlDocument, errorCode;

            string url = pageElement.Attribute("URL").Value;
            string pageServerUrl = null;

            if(url.StartsWith("http"))
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
            catch(Exception)
            {
                resultPageElement.Add(GetRenderingErrorNode(GetResourceString("BrokenLinkReport.NotValidXhml")));
                continue;
            }

            linksToCheck.AddRange(document.Descendants(Namespaces.Xhtml + "a")
                                          .Select(a => new LinkToCheck {LinkNode = a, ReportPageNode = resultPageElement, PageServerUrl = pageServerUrl}));
        }

        linksToCheck = linksToCheck.OrderBy(o => Guid.NewGuid()).ToList(); // Shuffling links

        ParallelFacade.ForEach(linksToCheck, linkToCheck =>
        {
            XElement a = linkToCheck.LinkNode;

            if (a.Attribute("href") == null)
            {
                return;
            }


            string urlStr = a.Attribute("href").Value;

            var href = HttpUtility.UrlDecode(urlStr).Trim();
            if (NotHttpLink(href))
            {
                return;
            }

            string previousnode = ToPreviewString (a.PreviousNode);
            string nextnode = ToPreviewString(a.NextNode);

            if (previousnode.Length > 50)
            {
                previousnode = "..." + previousnode.Substring(previousnode.Length - 40);
            }
            if (nextnode.Length > 50)
            {
                nextnode = nextnode.Substring(nextnode.Length - 40) + "...";
            }

            BrokenLinkType brokenLinkType;

            if (HttpLinkIsValid(href, linkToCheck.PageServerUrl, out brokenLinkType))
            {
                return;
            }

            string errorText = Describe(brokenLinkType);

            XElement invalidContent = new XElement("invalidContent",
                                                    new XAttribute("previousNode", previousnode),
                                                    new XAttribute("originalText", a.Value),
                                                    new XAttribute("originalLink", href),
                                                    new XAttribute("nextNode", nextnode),
                                                    new XAttribute("errorType", errorText));

            lock (linkToCheck.ReportPageNode)
            {
                linkToCheck.ReportPageNode.Add(invalidContent);
            }
            noInvalidLinksFound = false;
        });

        BuildReportTreeRec(infoDocumentRoot, Guid.Empty, reportElements);

        return noInvalidLinksFound;
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

        if(node is XElement)
        {
            var sbResult = new StringBuilder();

            foreach(var childNode in (node as XElement).Nodes())
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

        foreach(Guid childNodeId in childNodes)
        {
            XElement element = allNodes[childNodeId];
            if(element == null) continue;

            reportNode.Add(element);

            BuildReportTreeRec(element, childNodeId, allNodes);
        }
    }

    private bool HttpLinkIsValid(string url, string serverUrl, out BrokenLinkType brokenLinkType)
    {
        BrokenLinkType? cachedResult = _brokenLinks[url];
        if (cachedResult != null)
        {
            brokenLinkType = cachedResult.Value;
            return brokenLinkType == BrokenLinkType.None;
        }

        // Trying to parse as a page url first
        PageUrlData pageUrlData = null;
        try
        {
            pageUrlData = PageUrls.ParseUrl(url);
        }
        catch (UriFormatException)
        {
        }

        if (pageUrlData != null)
        {
            Guid linkedPageId = pageUrlData.PageId;

            IPage page;

            using(new DataScope(pageUrlData.PublicationScope, pageUrlData.LocalizationScope))
            {
                page = PageManager.GetPageById(linkedPageId);
            }

            if (page == null)
            {
                if (pageUrlData.PublicationScope == PublicationScope.Published)
                {
                    using (new DataScope(PublicationScope.Unpublished, pageUrlData.LocalizationScope))
                    {
                        if(PageManager.GetPageById(linkedPageId) != null)
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
            if(string.IsNullOrEmpty(pageUrlData.PathInfo))
            {
                brokenLinkType = BrokenLinkType.None;
                return true;
            }

            // If there's pathInfo -> making a request to check whether the link is actuall broken
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

    private bool SaveLinkCheckResult(string url, BrokenLinkType brokenLinkType)
    {
        lock (_brokenLinks)
        {
            _brokenLinks[url] = brokenLinkType;
        }
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

    protected static string GetResourceString(string stringId)
    {
        return StringResourceSystemFacade.GetString("Composite.Tools.LinkChecker", stringId);
    }

    private string Describe(BrokenLinkType brokenLinkType)
    {
        switch (brokenLinkType)
        {
            case BrokenLinkType.External:
                return GetResourceString("BrokenLink.External");
            case BrokenLinkType.MediaLibrary:
                return GetResourceString("BrokenLink.MediaLibrary");
            case BrokenLinkType.Page:
                return GetResourceString("BrokenLink.Page");
            case BrokenLinkType.PageNotPublished:
                return GetResourceString("BrokenLink.PageNotPublished");
            case BrokenLinkType.Relative:
                return GetResourceString("BrokenLink.Relative");
        }

        throw new InvalidOperationException("not supported link type " + brokenLinkType.ToString());
    }

    private XDocument TransformMarkup(XElement inputRoot)
    {
        XDocument newTree = new XDocument();
        
        using (XmlWriter writer = newTree.CreateWriter())
        {
            var xslTransformer = new XslCompiledTransform();
            xslTransformer.LoadFromPath(this.MapPath(XsltFileName));
            xslTransformer.Transform(inputRoot.CreateReader(), writer);
        }
        
        return newTree;
    }

    /// <summary>
    /// This method will check a url to see that it does not return server or protocol errors
    /// </summary>
    /// <param name="url">The path to check</param>
    /// <returns></returns>
    private BrokenLinkType ValidateByRequest(string url, string serverUrl, BrokenLinkType brokenLinkType)
    {
        if(url.StartsWith("/"))
        {
            url = UrlUtils.Combine(serverUrl ?? ServerUrl, url);
        }

        string hostname;
        try
        {
            hostname = new Uri(url).Host;
        }
        catch(UriFormatException)
        {
            return brokenLinkType;
        }

        // Allowing only one request to the same hostname at a time. Along with cache also ensures that the same url won't be requested to twice
        object hostNameSyncRoot = GetHostNameSyncObject(hostname);

        lock(hostNameSyncRoot)
        {
            var cachedResult = _brokenLinks[url];
            if (cachedResult != null) return cachedResult.Value;

            bool success = MakeHeadRequest(url);

            var result = success ? BrokenLinkType.None : brokenLinkType;
            SaveLinkCheckResult(url, result);

            return result;
        }
    }


    private bool MakeHeadRequest(string url)
    {
        Log.LogVerbose(LogTitle, url);

        try
        {
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;

            request.UserAgent = @"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5";
            request.Timeout = UrlCheckingTimeout;

            // Get only the header information -- no need to download any content
            request.Method = "HEAD";
            // Get the response
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            // Get the status code
            int statusCode = (int)response.StatusCode;
            // Good requests
            if (statusCode >= 100 && statusCode < 400)
            {
                return true;
            }

            // Server Errors
            if (statusCode >= 500 && statusCode <= 510)
            {
                Debug.Write(String.Format("The remote server has thrown an internal error. Url is not valid: {0}", url));
                return false;
            }
        }
        catch (WebException ex)
        {
            // 400 errors
            if (ex.Status == WebExceptionStatus.ProtocolError)
            {
                return false;
            }

            Debug.Write(String.Format("Unhandled status [{0}] returned for url: {1}", ex.Status, url));
        }
        catch (Exception ex)
        {
            Debug.Write(String.Format("Could not test url {0}; throws : {1}", url, ex));
        }

        return false;
    }

    private object GetHostNameSyncObject(string hostname)
    {
        hostname = hostname.ToLowerInvariant();

        object result = _hostnameSync[hostname];
        if(result == null)
        {
            lock (_hostnameSync)
            {
                result = _hostnameSync[hostname];
                if (result == null)
                {
                    _hostnameSync[hostname] = result = new object();
                }
            }
        }
        return result;
    }

    private string _serverUrl;
    private string ServerUrl
    {
        get
        {
            if (_serverUrl == null)
            {
                _serverUrl = new UrlBuilder(Context.Request.Url.ToString()).ServerUrl;
            }
            return _serverUrl;
        }
    }
}
