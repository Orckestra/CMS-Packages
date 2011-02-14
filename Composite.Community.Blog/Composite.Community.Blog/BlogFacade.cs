using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using Composite.Data;
using Composite.Core.Logging;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.WebClient;
using Composite.Core.Xml;
using Composite.Core;

namespace Composite.Community.Blog
{
	public class BlogFacade
	{
		public static IEnumerable<XElement> GetTagCloudXml(double minFontSize, double maxFontSize)
		{
			Guid currentPageId = PageRenderer.CurrentPageId;
			var blog = DataFacade.GetData<Entries>().Where(b => b.PageId == currentPageId).Select(b => b.Tags).ToList();
			var dcTags = new Dictionary<string, int>();

			foreach (var tag in blog.Where(b => !string.IsNullOrEmpty(b)).SelectMany(b => b.Split(','), (b, s) => s.Trim()))
			{
				if (dcTags.ContainsKey(tag))
				{
					dcTags[tag] = dcTags[tag] + 1;
				}
				else
				{
					dcTags.Add(tag, 1);
				}
			}

			return from d in dcTags
				   let minOccurs = dcTags.Values.Min()
				   let maxOccurs = dcTags.Values.Max()
				   let occurencesOfCurrentTag = d.Value
				   let weight = (Math.Log(occurencesOfCurrentTag) - Math.Log(minOccurs)) / (Math.Log(maxOccurs) - Math.Log(minOccurs))
				   let fontSize = minFontSize + Math.Round((maxFontSize - minFontSize) * weight)
				   select new XElement("Tags",
									   new XAttribute("Tag", d.Key),
									   new XAttribute("FontSize", fontSize),
									   new XAttribute("Rel", d.Value)
					);
		}

		public static IEnumerable<XElement> GetArchiveXml()
		{
			Guid currentPageId = PageRenderer.CurrentPageId;
			return DataFacade.GetData<Entries>().Where(c => c.PageId == currentPageId).GroupBy(c => new { c.Date.Year, c.Date.Month }).Select(
					b =>
					new XElement("BlogEntries",
									new XAttribute("Date", new DateTime(b.Key.Year, b.Key.Month, 1)),
									new XAttribute("Count", b.Select(x => x.Date.Year).Count())
								)
				);
		}

		public static Expression<Func<Entries, bool>> GetBlogFilterFromUrl()
		{
			Guid currentPageId = PageRenderer.CurrentPageId;
			Expression<Func<Entries, bool>> filter = f => f.PageId == currentPageId;

			var pathInfoParts = GetPathInfoParts();
			if (pathInfoParts != null)
			{
				int year;
				//TODO: this is very primitive and not ideal way of checking if it's year or tag: "2010" or "ASP.NET"
				if (int.TryParse(pathInfoParts[1], out year) && (pathInfoParts[1].Length == 4))
				{
					filter = f => f.Date.Year == year && f.PageId == currentPageId;
				}
				else
				{
					string tag = pathInfoParts[1];
					// filter below replaced becuase of LINQ2SQL problems
					//filter = f => f.Tags.Split(',').Any(t => t == tag) && f.PageId == currentPageId;
					filter = f => ((f.Tags.Contains("," + tag + ",")) || f.Tags.StartsWith(tag + ",") || (f.Tags.EndsWith("," + tag)) || f.Tags.Equals(tag)) && f.PageId == currentPageId;
				}

				if (pathInfoParts.Length > 2)
				{
					int month = Int32.Parse(pathInfoParts[2]);

					if (pathInfoParts.Length > 3)
					{
						int day = Int32.Parse(pathInfoParts[3]);

						if (pathInfoParts.Length > 4)
						{
							DateTime blogDate = new DateTime(year, month, day);

							filter = f => f.Date.Date == blogDate && pathInfoParts[4] == f.TitleUrl && f.PageId == currentPageId;
						}
						else
						{
							filter = f => f.Date.Year == year && f.Date.Month == month && f.Date.Day == day && f.PageId == currentPageId;
						}
					}
					else
					{
						filter = f => f.Date.Year == year && f.Date.Month == month && f.PageId == currentPageId;
					}
				}
			}

			return filter;
		}

		public static Expression<Func<Comments, bool>> GetCommentsFilterFromUrl()
		{
			Guid currentPageId = PageRenderer.CurrentPageId;
			Expression<Func<Comments, bool>> filter = f => true;

			var pathInfoParts = GetPathInfoParts();
			if (pathInfoParts != null)
			{
				if (pathInfoParts.Length > 4)
				{
					int year = Int32.Parse(pathInfoParts[1]);
					int month = Int32.Parse(pathInfoParts[2]);
					int day = Int32.Parse(pathInfoParts[3]);
					DateTime blogDate = new DateTime(year, month, day);

					Guid blogId =
						(DataFacade.GetData<Entries>().Where(
							b => b.PageId == currentPageId && b.Date.Date == blogDate && pathInfoParts[4] == b.TitleUrl).Select(
								b => b.Id)).First();

					filter = f => f.BlogEntry == blogId;
				}
			}

			return filter;
		}

		public static IEnumerable<XElement> GetCommentsCount()
		{
			return DataFacade.GetData<Comments>().GroupBy(c => c.BlogEntry).Select(
					b =>
					new XElement("Comment",
									new XAttribute("Id", b.Key),
									new XAttribute("Count", b.Select(x => x.BlogEntry).Count())
								)
				);

		}

		public static string GetUrlFromTitle(string title)
		{
			const string autoRemoveChars = @",./\?#!""@+'`´*():;$%&=¦§";
			var generated = new StringBuilder();

			foreach (var c in title.Where(c => autoRemoveChars.IndexOf(c) == -1))
			{
				generated.Append(c);
			}

			var url = generated.ToString().Replace(" ", "-");

			return url;
		}

		public static string GetBlogUrl(DateTime date, string title)
		{
			return string.Format("/{0}/{1}", CustomDateFormat(date, "yyyy/MM/dd"), GetUrlFromTitle(title));
		}

		public static XsltExtensionDefinition<BlogXsltExtensionsFunction> XsltExtensions()
		{
			return new XsltExtensionDefinition<BlogXsltExtensionsFunction>
			{
				EntensionObject = new BlogXsltExtensionsFunction(),
				ExtensionNamespace = "#BlogXsltExtensionsFunction"
			};
		}

		public static string[] GetPathInfoParts()
		{
			// Expecting '/yyyy/mm/dd/title' OR /tag
			var pathInfo = new UrlBuilder(HttpContext.Current.Request.RawUrl).PathInfo;

			return pathInfo != null && pathInfo.Contains("/") ? pathInfo.Split('/') : null;
		}

		public static string GetFullPath(string path)
		{
			var request = HttpContext.Current.Request;
			return (new Uri(request.Url, System.IO.Path.Combine(request.ApplicationPath, path))).OriginalString;
		}

		public static string CustomDateFormat(DateTime date, string dateFormat)
		{
			return date.ToString(dateFormat, CultureInfo.GetCultureInfo("en-US").DateTimeFormat);
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

		public static string GetCurrentCultureName()
		{
			return CultureInfo.CurrentCulture.Name.ToLower();
		}

		public static string GetCurrentPath()
		{
			var pathInfoParts = GetPathInfoParts();
			string path = string.Empty;
			if (pathInfoParts != null)
			{
				int year;

				if (int.TryParse(pathInfoParts[1], out year))
				{
					path = string.Format("/{0}", pathInfoParts[1]);

					if (pathInfoParts.Length > 2)
					{
						int month;
						if (int.TryParse(pathInfoParts[2], out month))
						{
							path = string.Format("{0}/{1}", path, pathInfoParts[2]);
						}
					}
				}
				else
				{
					path = string.Format("/{0}", pathInfoParts[1]);
				}
			}

			return path;
		}

		public static void SetTitleUrl(object sender, DataEventArgs dataEventArgs)
		{
			Entries entry = (Entries)dataEventArgs.Data;
			entry.TitleUrl = GetUrlFromTitle(entry.Title);
		}

		public static string Encode(string text)
		{
			text = HttpContext.Current.Server.UrlEncode(text);
			return text.Replace("+","%20");
		}
	}
}
