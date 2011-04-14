using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Xml.Linq;
using Composite.Core.Logging;
using Composite.Core.WebClient.Services.WysiwygEditor;
using Composite.Core.Xml;

namespace Composite.Forms.FormSubmitHandler
{
	public class Handler
	{
		private static readonly XNamespace ns = Namespaces.Xhtml;
		private static readonly string _providerName = "Composite.Forms.FormSubmitHandler";
		public static string ProviderName
		{
			get
			{
				return _providerName;
			}
		}

		
		private static Func<XElement, IEnumerable<XElement>> SelectInputs = el => el.Descendants().Where(e => e.Name == ns + "input" || e.Name == ns + "select" || e.Name == ns + "textarea").ToArray();

		public static IEnumerable<XElement> GetForm(string FormId, string HTMLForm, string Response, bool ShowFormPost, string EmailFrom, string EmailRecipients, string ReceiptEmailField, string ReceiptEmailIntro, string ReceiptEmailSubject)
		{
			XElement HTMLFormXml = GetXmlMarkup(HTMLForm);
			XElement ResponseXml = GetXmlMarkup(Response);
			var Request = HttpContext.Current.Request;

			if (Request.Form[FormId] == null)
			{
				Helper.ReplaceFormTagByDivTag(HTMLFormXml);
				Helper.SetNameForSubmitButtons(FormId, HTMLFormXml);
				yield return HTMLFormXml;
			}
			else
			{
				var result = new XElement("Result");
				foreach (var input in SelectInputs(HTMLFormXml))
				{
					var name = input.AttributeValue("name");
					if (name != null)
					{
						var values = Request.Form.GetValues(name) ?? new string[0];

						
						name = Helper.ToValidIndefiniter(name);
						result.SetAttributeValue(name, string.Join(",", values));

						switch (input.Name.LocalName)
						{
							case "input":
								var type = (input.AttributeValue("type")??string.Empty).ToLower();
								switch (type)
								{
									case "checkbox":
									case "radio":
										var checkboxValue = input.AttributeValue("value");
										input.SetAttributeValue("checked", values.Where(d => d == checkboxValue).FirstOrDefault());
										input.SetAttributeValue("disabled", "disabled");
										break;
									case "hidden":
									case "submit":
									case "image":
										input.Remove();
										break;
									default:
										input.ReplaceWith(new XText(values.FirstOrDefault() ?? string.Empty));
										break;

								}
								break;
							case "select":
								var options = input.Elements(ns + "option").Where(o => values.Contains(o.AttributeValue("value")) || (o.Attribute("value") == null) && values.Contains(o.Value.Trim()));
								input.ReplaceWith(options
									.Select(o =>
										new List<XNode> { new XText(o.Value), new XElement("br") }
										)
									);
								break;
							default:
								input.ReplaceWith(new XText(values.FirstOrDefault() ?? string.Empty));
								break;
						}
					}
					else
						input.Remove();
				}

				// remove from email "marked" nodes <span class="hidefromemail">xxx</span>
				HTMLFormXml.Descendants().Where(n => n.Attribute("class") != null && n.Attribute("class").Value == "hidefromemail").Remove();

				StoreHelper.Save(FormId, result);

				yield return ResponseXml;

				if (ShowFormPost)
					yield return HTMLFormXml;


				if (!string.IsNullOrEmpty(ReceiptEmailIntro.Trim()))
				{
					XElement ReceiptEmailIntroXml = GetXmlMarkup(ReceiptEmailIntro);
					HTMLFormXml.Element(ns + "body").AddFirst(
						new XElement(ns + "p", ReceiptEmailIntroXml)
						);
				}

				if (!string.IsNullOrEmpty(EmailRecipients.Trim()) && !string.IsNullOrEmpty(EmailFrom.Trim()))
				{
					SendEmail(EmailFrom, EmailRecipients, ReceiptEmailSubject, HTMLFormXml);
				}

				if (!string.IsNullOrEmpty(ReceiptEmailField.Trim()) && !string.IsNullOrEmpty(EmailFrom.Trim()))
				{
					var recipients = result.AttributeValue(ReceiptEmailField.Trim());
					if (!string.IsNullOrEmpty(recipients))
					{
						SendEmail(EmailFrom, recipients, ReceiptEmailSubject, HTMLFormXml);
					}
				}
			}
		}

		private static void SendEmail(string From, string To, string Subject, XElement MailBody)
		{
			MailMessage msgMail = new MailMessage();
			try
			{
				msgMail.From = new MailAddress(From);
			}
			catch (Exception e)
			{
				LoggingService.LogError(string.Format("Mail sending(From: '{0}')", From), e.Message);
				return;
			}

			try
			{
				msgMail.To.Add(To);
			}
			catch (Exception e)
			{
				LoggingService.LogError(string.Format("Mail sending(To: '{0}')", To), e.Message);
				return;
			}
			try
			{
				msgMail.Subject = Subject;
				msgMail.IsBodyHtml = true;
				msgMail.Body = MailBody.ToString();
				SmtpClient client = new SmtpClient();
				client.Send(msgMail);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("Unable to send mail. Please ensure that web.config has correct /configuration/system.net/mailSettings: " + e.Message);
			}
		}

		private static XElement GetXmlMarkup(string markup)
		{
			var stringMarkup = File.ReadAllText(HttpContext.Current.Server.MapPath(markup));
			return MarkupTransformationServices.TidyHtml(stringMarkup).Output.Root;
			/*
			old version
			 * if (markup is XElement)
			{
				markup = (markup as XElement).ToString();
			}
			if (markup is string)
			{
				var stringMarkup = markup as string;
				try
				{
					//Fix &#xA; bug
					stringMarkup = stringMarkup.Replace("&#xA;", "\r\n");

					return MarkupTransformationServices.TidyHtml(stringMarkup).Output.Root;
				}
				catch
				{
				}
			}
			else
			{
				throw new InvalidOperationException("Function support only string and XElement formMarkup");
			}
			return new XElement(ns + "html",
				new XElement(ns + "body")
			);*/
		}
	}
}