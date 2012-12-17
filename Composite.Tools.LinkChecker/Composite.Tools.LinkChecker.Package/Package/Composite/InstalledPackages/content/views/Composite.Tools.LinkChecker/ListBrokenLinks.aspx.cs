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

    protected void Page_Load(object sender, EventArgs e)
    {
        using (new DataScope(DataScopeIdentifier.Administrated, UserSettings.ActiveLocaleCultureInfo))
        {
            XElement infoDocumentRoot = new XElement("ActionItems");

            bool noBrokenLinks;

            using (new DataScope(PublicationScope.Published))
            {
                noBrokenLinks = BuildBrokenLinksReport(infoDocumentRoot);
            }

            if (noBrokenLinks)
            {
                emptyLabelPlaceHolder.Visible = true;
                return;
            }
            
            XDocument newTree = TransformMarkup(infoDocumentRoot);
                
            visualOutput.Controls.Add(new LiteralControl(newTree.ToString()));
        }
    }

    private bool RenderPage(string url, out string responseBody, out string errorMessage)
    {
        if(!url.StartsWith("http"))
        {
            url = ServerUrl + url;
        }

        try
        {
            var request = WebRequest.Create(url) as HttpWebRequest;

            request.UserAgent = @"Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.1.5) Gecko/20091102 Firefox/3.5.5";
            request.Timeout = PageRenderingTimeout;

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

                lock(_brokenLinks)
                {
                    _brokenLinks[url] = BrokenLinkType.None;
                }
                return true;
            }

            errorMessage = string.Format(GetResourceString("BrokenLinkReport.HttpStatus"), statusCode);
        }
        catch(WebException ex)
        {
            var webResponse = ex.Response as HttpWebResponse;
            if (webResponse != null && webResponse.StatusCode != HttpStatusCode.OK)
            {
                errorMessage = string.Format(GetResourceString("BrokenLinkReport.HttpStatus"), (int)webResponse.StatusCode + " " + webResponse.StatusCode);
            }
            else
            {
                errorMessage = ex.ToString();
            }
        }

        // Marking the link as valid, so it won't be shown multiple places and there won't be any additional requests
        lock (_brokenLinks)
        {
            _brokenLinks[url] = BrokenLinkType.None;
        }

        responseBody = null;
        return false;
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

            XElement resultPageElement = new XElement(PageElementName,
                                               new XAttribute("Id", pageId),
                                               new XAttribute("Title", pageElement.Attribute("Title").Value),
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

            if (!RenderPage(url, out htmlDocument, out errorCode))
            {
                resultPageElement.Add(GetErrorNode(errorCode));
                continue;
            }

            XDocument document;
            try
            {
                document = XDocument.Parse(htmlDocument);
            }
            catch(Exception)
            {
                resultPageElement.Add(GetErrorNode(GetResourceString("BrokenLinkReport.NotValidXhml")));
                continue;
            }

            ParallelFacade.ForEach(document.Descendants(Namespaces.Xhtml + "a"), a =>
            {
                if (a.Attribute("href") == null)
                {
                    return;
                }


                string urlStr = a.Attribute("href").Value;

                var href = HttpUtility.UrlDecode(urlStr);
                if (href.StartsWith("javascript:", StringComparison.InvariantCultureIgnoreCase))
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
                if (LinkIsValid(href, pageServerUrl, out brokenLinkType))
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

                lock (resultPageElement)
                {
                    resultPageElement.Add(invalidContent);
                }
                noInvalidLinksFound = false;
            }
            );
        }

        BuildReportTreeRec(infoDocumentRoot, Guid.Empty, reportElements);

        return noInvalidLinksFound;
    }

    private XElement GetErrorNode(string errorText)
    {
        return new XElement("invalidContent", new XAttribute("errorType", errorText));
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

    private bool LinkIsValid(string url, string serverUrl, out BrokenLinkType brokenLinkType)
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
                            return false;
                        }
                    }
                }

                brokenLinkType = BrokenLinkType.Page;
                return false;
            }

            // If no PathInfo - page link is already valid
            if(string.IsNullOrEmpty(pageUrlData.PathInfo))
            {
                brokenLinkType = BrokenLinkType.None;
                return true;
            }

            // If there's pathInfo -> making a request to check whether the link is actuall broken
            bool valid = ValidateByRequest(url, serverUrl);

            brokenLinkType = valid ? BrokenLinkType.None : BrokenLinkType.Page;
            return valid;
        }
        
        MediaUrlData mediaUrlData = MediaUrls.ParseUrl(url);
        
        if (mediaUrlData != null)
        {
            Guid mediaId = mediaUrlData.MediaId;
            string mediastore = mediaUrlData.MediaStore;
            
            bool mediaExist = DataFacade.GetData<IMediaFile>().Any(f => f.StoreId == mediastore && f.Id == mediaId);

            brokenLinkType = mediaExist ? BrokenLinkType.None : BrokenLinkType.MediaLibrary;
            return mediaExist;
        }

        bool urlIsInternal = url.StartsWith("/");
        bool isValid = ValidateByRequest(url, serverUrl);

        brokenLinkType = isValid
                             ? BrokenLinkType.None
                             : (urlIsInternal ? BrokenLinkType.Relative : BrokenLinkType.External);

        lock(_brokenLinks)
        {
            _brokenLinks[url] = brokenLinkType;
        }
        return isValid;
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

    private static string GetResourceString(string stringId)
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
    private bool ValidateByRequest(string url, string serverUrl)
    {
        if(url.StartsWith("/"))
        {
            url = (serverUrl ?? ServerUrl) + url;
        }

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
