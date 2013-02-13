using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Linq;
using Composite.Core.WebClient.Captcha;

namespace Composite.Community.TellAFriend
{
	[WebService(Namespace = "http://www.composite.net/ns/management")]
	[ScriptService]
	[SoapDocumentService(RoutingStyle = SoapServiceRoutingStyle.RequestElement)]
	public class TellAFriend : WebService
	{
		[WebMethod]
		[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
		public XmlNode Send(string fromName, string fromEmail, string toName, string toEmail, string description, string captcha, string captchaEncryptedValue, bool useCaptcha, string website, string url, string culture)
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);
			System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culture);
			var send = TellAFriendFacade.Send(fromName, fromEmail, toName, toEmail, description, captcha, captchaEncryptedValue, useCaptcha, website, url);
			var result = new XElement("TellAFriend", send);
			if (Captcha.IsValid(captchaEncryptedValue, captcha))
			{
				if (captcha != url)
				{
					Captcha.RegisterUsage(captchaEncryptedValue);
					var newEncryptedValue = Captcha.CreateEncryptedValue();
					//Refactoring Captcha.GetImageUrl
					var captchaServiceUrl = Composite.Core.WebClient.UrlUtils.PublicRootPath + "/Renderers/Captcha.ashx";
					var captchaUrl = new Composite.Core.UrlBuilder(captchaServiceUrl);
					captchaUrl["value"] = newEncryptedValue;
					result.SetAttributeValue("captchaImage", captchaUrl.ToString());
					result.SetAttributeValue("captchaEncryptedValue", newEncryptedValue);
				}
			}
			return GetXmlNode(result);
		}

		static XmlNode GetXmlNode(XElement element)
		{
			using (XmlReader xmlReader = element.CreateReader())
			{
				var xmlDoc = new XmlDocument();
				xmlDoc.Load(xmlReader);
				return xmlDoc;
			}
		}
	}
}
