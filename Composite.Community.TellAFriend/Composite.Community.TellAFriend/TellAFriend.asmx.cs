using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Linq;

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
			return GetXmlNode(new XElement("TellAFriend", send));
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
