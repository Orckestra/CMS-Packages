using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using Composite.Core;
using Composite.Core.Routing;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;

namespace Composite.Plugins.Functions.FunctionProviders.MvcFunctions
{
    internal static class ActionLinkHelper
    {
        private static readonly XName HrefAttributeName = "href";
        private static readonly XName ActionAttributeName = "action";

        public static bool IsActionLink(string href)
        {
            return href.StartsWith("/?");
        }

        public static void ConvertActionLinks(XhtmlDocument document, RequestContext requestContext, RouteCollection routeCollection)
        {
            var hrefAttributes = document.Descendants()
                .Where(e => e.Name.LocalName == "a")
                .Select(a => a.Attribute(HrefAttributeName));

            var actionAttributes = document.Descendants()
                .Where(e => e.Name.LocalName == "form")
                .Select(a => a.Attribute(ActionAttributeName));

            var linkAttributes = hrefAttributes.Concat(actionAttributes)
                                 .Where(a => a != null && IsActionLink((string) a));

            foreach (var linkAttr in linkAttributes)
            {
                string url = ConvertActionLink((string)linkAttr, requestContext, routeCollection);
                if (url != null && !url.StartsWith("/?"))
                {
                    linkAttr.Value = url;
                }
            }
        }

        public static string ConvertActionLink(string link, RequestContext requestContext, RouteCollection routeCollection)
        {
            var urlBuilder = new UrlBuilder(link);
            var parameters = urlBuilder.GetQueryParameters();
            string actionName = parameters["action"];
            string controllerName = parameters["controller"];
            if (string.IsNullOrEmpty(actionName) || string.IsNullOrEmpty(controllerName))
            {
                return null;
            }

            parameters.Remove("action");
            parameters.Remove("controller");

            var routeValueDictionary = new RouteValueDictionary();
            foreach (var key in parameters.AllKeys)
            {
                routeValueDictionary.Add(key, parameters[key]);
            }

            return UrlHelper.GenerateUrl(null, actionName, controllerName, routeValueDictionary, routeCollection, requestContext, false);   
        }

        public static string ControllerLinkToC1PageLink(string link, string baseControllerUrl)
        {
            string pageUrl = GetCurrentC1PageUrl();

            var replacementRule = GetReplacementRule(pageUrl, baseControllerUrl);

            return ReplacePrefix(link, replacementRule);
        }

        public static void ControllerLinksToC1PageLinks(XhtmlDocument document, string baseControllerUrl)
        {
            // Converting links like 
            // "~/<controller name>/action" to "<page url>/action
            //
            // Also for function with parameters: f.e.:
            // Page url "/Home/BookStore/Fantasy",
            // and controller "books" with a predefined parameter "fantasy", it should replace
            // "/books[/...]"  with "/Home/BookStore[/....]" which require the url part comparison below:

            string pageUrl = GetCurrentC1PageUrl();

            var replacementRule = GetReplacementRule(pageUrl, baseControllerUrl);

            // Replacing action links
            foreach (var anchor in document.Descendants().Where(e => e.Name.LocalName == "a"))
            {
                var hrefAttr = anchor.Attribute(HrefAttributeName);
                if (hrefAttr == null) continue;

                hrefAttr.Value = ReplacePrefix(hrefAttr.Value, replacementRule);
            }
        }

        private static string ReplacePrefix(string link, ReplacementRule replacementRule)
        {
            return link
                .ReplacePrefix(replacementRule.OldPrefix, replacementRule.NewPrefix)
                .ReplacePrefix(replacementRule.OldPrefixWithTilde, replacementRule.NewPrefix);
        }

        private static string ReplacePrefix(this string link, string from, string to)
        {
            if (string.Equals(link, from, StringComparison.OrdinalIgnoreCase))
            {
                return to;
            }

            if (link.StartsWith(from, StringComparison.OrdinalIgnoreCase))
            {
                char nextSymbol = link[from.Length];
                if (nextSymbol == '/' || nextSymbol == '?' || nextSymbol == '#')
                {
                    return to + link.Substring(from.Length);
                }
            }

            return link;
        }

        private static string GetCurrentC1PageUrl()
        {
            return PageUrls.BuildUrl(PageRenderer.CurrentPage) 
                ?? PageUrls.BuildUrl(PageRenderer.CurrentPage, UrlKind.Internal);
        }

        private static ReplacementRule GetReplacementRule(string pageUrl, string routePrefix)
        {
            string[] routeUrlParts = routePrefix.Split('/');
            if (routeUrlParts.Length > 2)
            {
                string[] pageUrlParts = pageUrl.Split('/');

                int commonParts = 0;
                while (commonParts < Math.Min(pageUrlParts.Length, routeUrlParts.Length - 2)
                       && pageUrlParts[pageUrlParts.Length - 1 - commonParts] == routeUrlParts[routeUrlParts.Length - 1 - commonParts])
                {
                    commonParts++;
                }

                if (commonParts > 0)
                {
                    pageUrl = string.Join("/", pageUrlParts.Take(pageUrlParts.Length - commonParts));
                    routePrefix = string.Join("/", routeUrlParts.Take(routeUrlParts.Length - commonParts));
                }
            }

            return new ReplacementRule
            {
                OldPrefixWithTilde = routePrefix,
                OldPrefix = routePrefix.Substring(1),
                NewPrefix = pageUrl
            };
        }

        private class ReplacementRule
        {
            public string OldPrefix;
            public string OldPrefixWithTilde;
            public string NewPrefix;
        }
    }
}
