using System.Linq;
using System.Xml.Linq;
using System.Web;

using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Core.Routing.Pages;


namespace Composite.Media.WordDocumentViewer.Renderers
{
    public class SplitRenderer
    {
        private static string GetPathInfo()
        {
            string result = C1PageRoute.GetPathInfo() ?? string.Empty;

            if (result != string.Empty)
            {
                C1PageRoute.RegisterPathInfoUsage();
            }

            return result;
        }

        private static string[] PathInfoParts;
        private static string CurrentTocName = string.Empty;


        public static XDocument Render(XDocument document, DocumentStyles documentStyles)
        {
            var tocName = HttpContext.Current.Request["Toc"];
            if (tocName == null)
            {
                var pathInfo = GetPathInfo();
                if (pathInfo.StartsWith("/"))
                    pathInfo = pathInfo.Substring(1);
                PathInfoParts = pathInfo.Split('/');
                // get last PathInfo Part
                CurrentTocName = PathInfoParts[PathInfoParts.Length - 1];

            }

            var body = document.Root.Element(Namespaces.Xhtml + "body");

            body = AddNumeration(body, documentStyles);

            var tocs = body.Elements().Where(documentStyles.IsLevel).ToArray();

            var toc = tocs.FirstOrDefault(e => documentStyles.GetLevelToc(e) == CurrentTocName);

            if (toc == null)
            {
                toc = tocs.FirstOrDefault();
                CurrentTocName = toc?.AttributeValue("toc");
            }


            if (toc != null)
            {
                var tocText = toc.NodesAfterSelf().TakeWhile(node => !documentStyles.IsLevel(node)).ToList();

                tocName = documentStyles.GetLevelToc(toc);


                var innerLinks = tocText.Cast<XElement>().Descendants(Namespaces.Xhtml + "a").Where(a => a.Attribute("href") != null && a.Attribute("href").Value.StartsWith("#")).ToArray();
                foreach (var innerLink in innerLinks)
                {
                    var anchorId = innerLink.AttributeValue("href").Substring(1);
                    var anchor = body.Descendants(Namespaces.Xhtml + "a")
                                     .FirstOrDefault(a => a.AttributeValue("name") == anchorId);
                    if (anchor != null)
                    {
                        var anchorToc = anchor.Parent.Parent; // need check is valid
                        if (documentStyles.IsLevel(anchorToc))
                        {
                            innerLink.SetAttributeValue("href", GetUrl(anchorToc.AttributeValue("toc")) + innerLink.AttributeValue("href"));
                        }
                        else
                        {
                            anchorToc = anchorToc.NodesBeforeSelf().Reverse().FirstOrDefault(documentStyles.IsLevel) as XElement;
                            if (anchorToc != null)
                            {
                                innerLink.SetAttributeValue("href", GetUrl(anchorToc.AttributeValue("toc")) + innerLink.AttributeValue("href"));
                            }
                        }
                    }
                }


                var prevLink = toc.NodesBeforeSelf().Reverse().FirstOrDefault(documentStyles.IsLevel) as XElement;
                var nextLink = toc.NodesAfterSelf().FirstOrDefault(documentStyles.IsLevel) as XElement;

                var navigation = new XElement(Namespaces.Xhtml + "div",
                    new XAttribute("class", "Navigation"),
                    GetLink(prevLink),
                    GetLink(null),
                    GetLink(nextLink)
                );
                navigation = null;

                body.ReplaceNodes(
                    new XElement(Namespaces.Xhtml + "ul",
                        new XAttribute("id", "WordDocumentViewerToc"),
                        tocs.Select(e => new XElement(
                            Namespaces.Xhtml + "li",
                            documentStyles.GetLevelToc(e) == tocName ? new XAttribute("class", "selected") : null,
                            GetLink(e)))
                    ),
                    navigation,
                    toc, tocText,
                    navigation);
            }

            return document;
        }


        private static XElement GetLink(XElement toc, string className)
        {
            var link = GetLink(toc);
            link.SetAttributeValue("class", className);
            return link;
        }

        private static XElement GetLink(XElement toc)
        {
            if (toc == null)
                return new XElement(Namespaces.Xhtml + "a",
                    new XAttribute("href", GetUrl(string.Empty)),
                    new XAttribute("class", "Contents"),
                    "Contents"
                );

            var tocName = toc.AttributeValue("toc");
            return new XElement(Namespaces.Xhtml + "a",
                new XAttribute("href", GetUrl(tocName)),
                new XText(toc.Value)
            );
        }

        private static string GetUrl(string toc)
        {
            var pageId = PageRenderer.CurrentPageId;

            if (string.IsNullOrEmpty(toc))
                return $"~/Renderers/Page.aspx?pageId={pageId}";
            var lastPart = PathInfoParts[PathInfoParts.Length - 1];
            var path = lastPart == CurrentTocName ? GetPathInfo().Replace("/" + lastPart, string.Empty) : GetPathInfo();
            return $"~/Renderers/Page.aspx{path}/{HttpUtility.UrlEncode(toc)}?pageId={pageId}";
        }


        private static XElement AddNumeration(XElement body, DocumentStyles documentStyles)
        {

            var levels = body.Elements().Where(documentStyles.IsLevel).ToArray();
            foreach (var level in levels)
            {
                var newLevel = new XElement(Namespaces.Xhtml + "h4",
                    new XAttribute("level", documentStyles.GetLevel(level)),
                    //new XText(documentStyles.GetNumeration(level)),
                    //new XText(" "),
                    level.Nodes()
                );
                newLevel.SetAttributeValue("toc", documentStyles.GetLevelToc(newLevel));
                level.ReplaceWith(newLevel);
            }

            return body;
        }

/*		public struct Link
		{
			public Link(string title, string id)
			{
				Title = title;
				Id = id;
			}

			public readonly string Title;
			public readonly string Id;
		}*/

    }
}
