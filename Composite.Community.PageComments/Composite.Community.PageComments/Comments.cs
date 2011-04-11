using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Data.Types;
using Composite.Core.Logging;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Functions;
using Composite.Core.Types;

namespace Composite.Community.PageComments
{
	public class Comments
	{
		[FunctionParameterDescription("Name", "Name of the person that commented", "Internal parameter", "")]
		[FunctionParameterDescription("Email", "Email of the person that commented", "Internal parameter", "")]
		[FunctionParameterDescription("CommentTitle", "Title to comment", "Internal parameter", "")]
		[FunctionParameterDescription("CommentText", "Comment text", "Internal parameter", "")]
		[FunctionParameterDescription("Captcha", "Captcha input", "Antibot protection", "")]
		public static IEnumerable<XElement> SaveComment(string name, string email, string commentTitle, string commentText, string Captcha)
		{
			var currentPageId = PageRenderer.CurrentPageId;

			yield return new XElement("SubmittedData",
							new XAttribute("Fieldname", "Name"),
							new XAttribute("Value", name));
			yield return new XElement("SubmittedData",
							new XAttribute("Fieldname", "Email"),
							new XAttribute("Value", email));
			yield return new XElement("SubmittedData",
							new XAttribute("Fieldname", "CommentTitle"),
							new XAttribute("Value", commentTitle));
			yield return new XElement("SubmittedData",
							new XAttribute("Fieldname", "CommentText"),
							new XAttribute("Value", commentText));
			yield return new XElement("SubmittedData",
							new XAttribute("Fieldname", "Captcha"),
							new XAttribute("Value", Captcha));

			// Input error ?
			if (Captcha != "true" || string.IsNullOrEmpty(name) || !Validate(RegExpLib.Email, email, true) || string.IsNullOrEmpty(commentTitle) || string.IsNullOrEmpty(commentText))
			{
				var errorText = new Hashtable();

				if (Captcha != "true")
				{
					errorText.Add("Captcha", "Incorrect Captcha value");
				}
				if (string.IsNullOrEmpty(name))
				{
					errorText.Add("Name", "Incorrect Name value");
				}
				if (!Validate(RegExpLib.Email, email, true))
				{
					errorText.Add("Email", "Incorrect E-mail value");
				}
				if (string.IsNullOrEmpty(commentTitle))
				{
					errorText.Add("Title", "Incorrect Title value");
				}
				if (string.IsNullOrEmpty(commentText))
				{
					errorText.Add("Comment", "Incorrect Comment value");
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
				var currentPage = PageManager.GetPageById(currentPageId);

				if (!currentPage.GetDefinedFolderTypes().Contains(typeof(Item)))
				{
					currentPage.AddFolderDefinition(typeof(Item).GetImmutableTypeId());
				}

				//add data
				var commentItem = DataFacade.BuildNew<Item>();

				commentItem.Name = name;
				commentItem.Email = email;
				commentItem.Title = commentTitle;
				commentItem.Date = DateTime.Now;
				commentItem.Comment = commentText;

				PageFolderFacade.AssignFolderDataSpecificValues(commentItem, currentPage);
				DataFacade.AddNew(commentItem);

				// Redirect to view newly added comment
				string pageUrl;
				var found = PageStructureInfo.TryGetPageUrl(currentPageId, out pageUrl);

				if (found)
				{
					PrepareMail(currentPageId, pageUrl, name, email, commentTitle, commentText, DateTime.Now.ToString());
					HttpContext.Current.Response.Redirect(pageUrl, false);
				}
			}
		}

		private static void PrepareMail(Guid pageId, string pageUrl, string commentName, string commentEmail, string commentTitle, string commentText, string commentDate)
		{
			var moderatorEmails = DataFacade.GetData<Settings>().Where(s => s.PageId == pageId).Select(s => s.Email).FirstOrDefault();

			if (string.IsNullOrEmpty(moderatorEmails)) return;
			var mailTo = moderatorEmails;
			var mailFrom = moderatorEmails;

			var websiteTitle = (from x in PageStructureInfo.PageListInDocumentOrder() where x.Key == PageStructureInfo.GetAssociatedPageIds(PageRenderer.CurrentPageId, SitemapScope.Level1).First() select x.Value).First();

			var hostname = string.Empty;
			try
			{
				hostname = (from x in DataFacade.GetData<IPageHostNameBinding>() where x.PageId == PageStructureInfo.GetAssociatedPageIds(PageRenderer.CurrentPageId, SitemapScope.Level1).First() select x.HostName).First();
			}
			catch (Exception)
			{
				hostname = Dns.GetHostName();
			}

			if (!hostname.StartsWith("http"))
			{
				hostname = "http://" + hostname;
			}

			var mailSubject = string.Format("New Comment: {0}", websiteTitle);
			var mailBody = string.Format("<h1>The following comments have been added:</h1>{0}{0}<b>Page:</b> {1}{0}<a target='_blank' href='{8}{7}'>{8}{7}</a>{0}{0}<b>Date and time:</b> {2}{0}<b>Name:</b> {3}{0}<b>E-mail:</b> {4}{0}{0}<b>Comment:</b> {0}<i>{5}</i>{0}{6}",
					GetNewline(), PageRenderer.CurrentPage.Title, commentDate, commentName, commentEmail, commentTitle, commentText, pageUrl, hostname).Replace("\r", "<br/>");

			SendMail(mailTo, mailFrom, mailSubject, mailBody);
		}

		public static string GetNewline()
		{
			return Environment.NewLine;
		}

		public static bool Validate(string regularExpression, object value, bool isRequired)
		{
			if (value == null)
			{
				return isRequired ? false : true;
			}
			{
				return String.IsNullOrEmpty(value.ToString()) ? false : Regex.IsMatch(value.ToString(), regularExpression);
			}
		}

		public static void SendMail(string to, string from, string subject, string body)
		{
			var mail = new MailMessage();
			mail.To.Add(to);
			mail.From = new MailAddress(from);
			mail.Subject = subject;
			mail.Body = body;
			mail.IsBodyHtml = true;
			mail.BodyEncoding = Encoding.Default;
			mail.SubjectEncoding = Encoding.Default;
			var smtpMail = new SmtpClient();
			smtpMail.Send(mail);
		}
	}

	public static class RegExpLib
	{
		public const string Email = @"^[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+(?:[A-Za-z]{2}|com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|coop|pro)\b$";
	}
}