using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Composite.Data;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Functions;

namespace Composite.Community.Blog
{
	public class CommentsFacade
	{
		[FunctionParameterDescription("name", "Name of the person that commented", "Internal parameter", "")]
		[FunctionParameterDescription("email", "Email of the person that commented", "Internal parameter", "")]
		[FunctionParameterDescription("commentTitle", "Title to comment", "Internal parameter", "")]
		[FunctionParameterDescription("commentText", "Comment text", "Internal parameter", "")]
		[FunctionParameterDescription("captcha", "Captcha input", "Antibot protection", "")]
		[FunctionParameterDescription("blogEntryGuid", "Blog Entry Guid", "Blog Entry Guid", "")]
		public static IEnumerable<XElement> SaveComment(string name, string email, string commentTitle, string commentText, string captcha, string blogEntryGuid)
		{
			var currentPageId = PageRenderer.CurrentPageId;
			var blogEntryId = new Guid(blogEntryGuid);
			var blog = (from b in DataFacade.GetData<Entries>() join v in DataFacade.GetData<Authors>() on b.Author equals v.Id into n from a in n.DefaultIfEmpty() where b.Id == blogEntryId select new { blogTitle = b.Title, blogDate = b.Date, authorsEmail = a.Email, NotifyOnNewComments = b.NotifyOnNewComments, AllowNewComments = b.AllowNewComments, DisplayComments = b.DisplayComments }).First();

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
							new XAttribute("Value", captcha));

			// Input error ?
			if (captcha != "true" || string.IsNullOrEmpty(name) || !BlogFacade.Validate(RegExpLib.Email, email, true) || string.IsNullOrEmpty(commentTitle) || string.IsNullOrEmpty(commentText))
			{
				var errorText = new Hashtable();

				if (captcha != "true")
				{
					errorText.Add("Captcha", "Incorrect Captcha value");
				}

				if(string.IsNullOrEmpty(name))
				{
					errorText.Add("Name", "Incorrect Name value");
				}

				if(!BlogFacade.Validate(RegExpLib.Email, email, true))
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
				if(blog.AllowNewComments)
				{
					var commentItem = DataFacade.BuildNew<Comments>();
					commentItem.Id = Guid.NewGuid();
					commentItem.BlogEntry = blogEntryId;
					commentItem.Date = DateTime.Now;
					commentItem.Name = name;
					commentItem.Email = email;
					commentItem.Title = commentTitle;
					commentItem.Comment = commentText;
					DataFacade.AddNew(commentItem);

					// Redirect to view newly added comment
					string pageUrl;
					bool found = PageStructureInfo.TryGetPageUrl(currentPageId, out pageUrl);

					if (found)
					{
						pageUrl = pageUrl + BlogFacade.GetBlogUrl(blog.blogDate, blog.blogTitle);

						if (blog.NotifyOnNewComments)
						{
							if (!string.IsNullOrEmpty(blog.authorsEmail))
							{
								PrepareMail(blog.blogTitle, blog.authorsEmail, pageUrl, name, email, commentTitle, commentText, DateTime.Now.ToString());
							}
						}
						HttpContext.Current.Response.Redirect(pageUrl + "#newcomment", false);
					}
				}
			}
		}

		private static void PrepareMail(string blogTitle, string authorsEmail, string pageUrl, string commentName, string commentEmail, string commentTitle, string commentText, string commentDate)
		{
			commentText = commentText.Replace("\r", "<br/>");
			pageUrl = BlogFacade.GetFullPath(pageUrl);
			var mailSubject = string.Format("New Comment: {0}", commentTitle);
			var mailBody = string.Format("<b>Date and time:</b> {2}<br/><b>Name:</b> {3}<br/><b>E-mail:</b> {4}<br/><b>Comment title:</b> {5}<br/><b>Comment:</b><br/>{6}<br/><br/><b>Blog:</b> {0}<br/><b>Blog Entry Title:</b> {7}<br/><b>Url:</b> <a target='_blank' href='{1}'>{1}</a>",
					PageRenderer.CurrentPage.Title, pageUrl, commentDate, commentName, commentEmail, commentTitle, commentText, blogTitle);

			BlogFacade.SendMail(authorsEmail, authorsEmail, mailSubject, mailBody);
		}
	}
}
