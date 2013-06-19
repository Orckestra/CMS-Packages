using System;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using Composite.Plugins.Functions.XslExtensionsProviders.ConfigBasedXslExtensionsProvider;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace Composite.Community.OpenID
{
    [ConfigurationElementType(typeof (ConfigBasedXslExtensionInfo))]
    public class OpenIDExtensions
    {
        public string GetLocalized(string resourceName, string key)
        {
            return Utils.GetLocalized(resourceName, key);
        }

        public static XPathNavigator GetLocalizedXml(string resourceName, string key)
        {
            return XElement.Parse("<root>" + Utils.GetLocalized(resourceName, key) + "</root>").CreateNavigator();
        }

        public string GetReturnUrl()
        {
            return HttpUtility.UrlEncode(Utils.GetReturnUrl());
        }

        public string SignIn(string userDetailsPageId)
        {
            Guid pageId;
            if (Guid.TryParse(userDetailsPageId, out pageId))
            {
                return OpenIDFacade.SignIn(pageId);
            }
            else
            {
                return OpenIDFacade.SignIn(Guid.Empty);
            }
        }

        public void SignOut()
        {
            OpenIDFacade.SignOut();
        }

        public string GetCurrentUserDisplayName()
        {
            return OpenIDFacade.GetCurrentUserDisplayName();
        }

        public string GetCurrentUserGuid()
        {
            return OpenIDFacade.GetCurrentUserGuid().ToString();
        }

        public XPathNavigator UserDetailsHelper()
        {
            XElement result = HttpContext.Current.Request.Form["submit"] == "true"
                                  ? OpenIDFacade.SaveCurrentUserDetails()
                                  : OpenIDFacade.GetCurrentUserDetails();
            return result.CreateNavigator();
        }

        public bool IsAuthenticated()
        {
            return OpenIDFacade.IsAuthenticated();
        }

        public void Redirect(string pageId)
        {
            Guid pageGuid;
            if (Guid.TryParse(pageId, out pageGuid))
            {
                Utils.Redirect(pageGuid);
            }
        }
    }
}