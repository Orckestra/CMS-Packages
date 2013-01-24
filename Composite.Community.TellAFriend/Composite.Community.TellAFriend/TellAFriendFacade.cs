using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using System.Web;
using System.Xml.Linq;
using Composite.Community.TellAFriend.Localization;
using Composite.Community.TellAFriend.Validators;
using Composite.Core.WebClient.Captcha;
using Composite.Core.Xml;

namespace Composite.Community.TellAFriend
{
	public class TellAFriendFacade
	{
		public static IEnumerable<XElement> Send(string fromName, string fromEmail, string toName, string toEmail, string description, string captcha, string captchaEncryptedValue, bool useCaptcha, string website, string url)
		{
			var catpchaIsValid = !useCaptcha || Captcha.IsValid(captcha, captchaEncryptedValue);

			yield return new XElement("SubmittedData",
				new XAttribute("Fieldname", "fromName"),
				new XAttribute("Value", fromName));
			yield return new XElement("SubmittedData",
							new XAttribute("Fieldname", "fromEmail"),
							new XAttribute("Value", fromEmail));
			yield return new XElement("SubmittedData",
							new XAttribute("Fieldname", "toName"),
							new XAttribute("Value", toName));
			yield return new XElement("SubmittedData",
							new XAttribute("Fieldname", "toEmail"),
							new XAttribute("Value", toEmail));
			yield return new XElement("SubmittedData",
							new XAttribute("Fieldname", "description"),
							new XAttribute("Value", description));
			yield return new XElement("SubmittedData",
							new XAttribute("Fieldname", "captcha"),
							new XAttribute("Value", catpchaIsValid));

			// Input error ?
			if (string.IsNullOrEmpty(fromName) || !RegExp.Validate(RegExpLib.Email, fromEmail, true) || string.IsNullOrEmpty(toName) || !RegExp.Validate(RegExpLib.Email, toEmail, true) || !catpchaIsValid)
			{
				var errorText = new Hashtable();

				if (string.IsNullOrEmpty(fromName))
				{
					errorText.Add("fromName", Resource.GetLocalized("TellAFriend","fromNameError"));
				}

				if (!RegExp.Validate(RegExpLib.Email, fromEmail, true))
				{
					errorText.Add("fromEmail", Resource.GetLocalized("TellAFriend","fromEmailError"));
				}

				if (string.IsNullOrEmpty(toName))
				{
					errorText.Add("toName", Resource.GetLocalized("TellAFriend", "toNameError"));
				}

				if (!RegExp.Validate(RegExpLib.Email, toEmail, true))
				{
					errorText.Add("toEmail", Resource.GetLocalized("TellAFriend", "toEmailError"));
				}

				if (!catpchaIsValid)
				{
					errorText.Add("captcha", Resource.GetLocalized("TellAFriend", "captchaError"));
				}

				foreach (string key in errorText.Keys)
				{
					yield return new XElement("Error",
								 new XAttribute("Fieldname", key),
								 new XAttribute("ErrorDescription", errorText[key].ToString()));
				}
			}
			else
			{
				var from = new MailAddress(fromEmail, fromName);
				var to = new MailAddress(toEmail, toName);
				url = url.Replace("&TellAFriend=1", "").Replace("?TellAFriend=1", "");
				var subject = string.Format("{0} {1}", Resource.GetLocalized("TellAFriend", "emailSubject"), website);
				//var body = string.Format("{0}<br />{1}: {2}", description, Resource.GetLocalized("TellAFriend", "emailBody"),  url);
				var emailBody = Resource.GetLocalized("TellAFriend", "emailBody");
				var body = string.Format(emailBody, description.Replace("\n", "<br />"), url);

				Email.Send(from, to, from, null, subject, body, true, MailPriority.Normal);
			}
		}

		public static XsltExtensionDefinition<TellAFriendXsltExtensionsFunction> XsltExtensions()
		{
			return new XsltExtensionDefinition<TellAFriendXsltExtensionsFunction>
			{
				EntensionObject = new TellAFriendXsltExtensionsFunction(),
				ExtensionNamespace = "#TellAFriendXsltExtensionsFunction"
			};
		}
		
		public static string GetUrl()
		{
			var url = HttpContext.Current.Request.Url.ToString();
			return string.Format("{0}{1}TellAFriend=1", url, url.Contains("?") ? "&" : "?");
		}

		public static string GetWebsite()
		{
			return HttpContext.Current.Request.Url.DnsSafeHost;
		}

	}
}
