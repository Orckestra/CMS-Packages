using System;
using System.Collections.Specialized;
using System.Web;
using Composite.Core;
using Composite.Core.WebClient.Renderings.Page;

namespace Composite.Community.OpenID
{
    public class Utils
    {
        public static string GetLocalized(string resourceName, string key)
        {
            object ro = HttpContext.GetGlobalResourceObject(resourceName, key);
            return ro == null ? string.Empty : ro.ToString();
        }

        public static string GetReturnUrl()
        {
            return HttpContext.Current.Request.QueryString["returnUrl"] ?? HttpContext.Current.Request.RawUrl;
        }

        /// <summary>
        ///     Redirect to page with query string parameter returnUrl as current page
        /// </summary>
        /// <param name="pageId"></param>
        public static bool Redirect(Guid pageId)
        {
            return Redirect(pageId, GetReturnUrl());
        }

        /// <summary>
        ///     Redirect to page with query string parameter returnUrl
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="returnUrl"></param>
        public static bool Redirect(Guid pageId, string returnUrl)
        {
            string pageUrl;
            PageStructureInfo.TryGetPageUrl(pageId, out pageUrl);
            if (!string.IsNullOrEmpty(pageUrl))
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    var urlBuilder = new UrlBuilder(pageUrl);
                    var queryParameters = new NameValueCollection();
                    queryParameters.Add("returnUrl", returnUrl);
                    urlBuilder.AddQueryParameters(queryParameters);
                    pageUrl = urlBuilder.ToString();
                }
                HttpContext.Current.Response.Redirect(pageUrl, false);
                return true;
            }
            return false;
        }

        public static bool RedirectToReturnUrl()
        {
            string returnUrl = HttpContext.Current.Request.QueryString["ReturnUrl"];
            if (!String.IsNullOrEmpty(returnUrl))
            {
                HttpContext.Current.Response.Redirect(returnUrl, false);
                return true;
            }
            return false;
        }
    }
}