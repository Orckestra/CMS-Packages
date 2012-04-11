using System;
using System.Linq;
using System.Web;
using System.Net.Mail;
using Composite.Core.Logging;
using System.Text;
using Composite.Data;
using Composite.Core;
using Composite.Core.Xml;

namespace Composite.Tools.LegacyUrlHandler.BrokenLinks
{
	public class Functions
	{
		public Functions()
		{
		}
		
		public static XhtmlDocument SaveBrokenLink()
		{
			var request = HttpContext.Current.Request;

			// ignore if referer is empty
			if (request.UrlReferrer == null
				|| request.RawUrl.StartsWith("/Composite/content/flow/FlowUi.aspx", StringComparison.InvariantCultureIgnoreCase)
				|| request.UrlReferrer.ToString().IndexOf("/Composite/content/views/browser/browser.aspx", StringComparison.InvariantCultureIgnoreCase) > 0)
			{
				return null;
			}
			var url = "http://" + request.Url.Host + request.RawUrl;
			var referer = (request.UrlReferrer == null) ? "" : request.UrlReferrer.ToString();
			var userAgent = request.UserAgent ?? string.Empty;
			var ipAddress = GetIPAddress(request);

			if (!UserAgentIsTrusted(userAgent)
				|| !RefererIsTrusted(referer)
				|| IPAddressIsInBlackList(ipAddress))
			{
				return null;
			}

			using (var conn = new DataConnection())
			{
				var item = conn.Get<BrokenLink>().FirstOrDefault(el => el.BadURL == url);
				if (item == null)
				{
					var newItem = conn.CreateNew<BrokenLink>();
					newItem.Id = Guid.NewGuid();
					newItem.BadURL = url;
					newItem.Referer = referer;
					newItem.UserAgent = userAgent;
					newItem.IP = ipAddress;
					newItem.Date = DateTime.Now;
					conn.Add<BrokenLink>(newItem);
				}
				else
				{
					var now = DateTime.Now;
					var itemDate = item.Date.Value.Date;
					if (itemDate.Year == now.Year && itemDate.Month == now.Month && itemDate.Day == now.Day)
						return null;
					item.Date = DateTime.Now;
					item.Referer = referer;
					item.UserAgent = userAgent;
					item.IP = ipAddress;
					item.IsNotified = false;
					conn.Update<BrokenLink>(item);
				}
			}
			return null;
		}

		public static void Send404MailReport()
		{
			//var request = HttpContext.Current.Request;
			using (var conn = new DataConnection())
			{
				var brokenLinks =
					conn.Get<BrokenLink>().Where(link => link.IsNotified == false).ToList();
				var brokenLinksIPs = brokenLinks.Select(bl => bl.IP).Distinct().ToList();
				if (!brokenLinksIPs.Any()) return;
				var emailBody = new StringBuilder();
				var siteHostName = new System.Uri(brokenLinks.First().BadURL).Host;
				var emailSubject = string.Format("Broken links report at {0}", siteHostName);
				var emailRecipients = Config.RecipientEmails.Aggregate((i, j) => i + ";" + j);
				emailBody.AppendFormat(@"<h2>{0}</h2> <h3>BAD URLs grouped by IP:</h3>", emailSubject);

				brokenLinksIPs.ForEach(ip =>
				{
					emailBody.AppendFormat("<ul><li><b>{0}</b>", ip);
					emailBody.Append("<ol>");
					brokenLinks.Where(el => el.IP == ip).ToList().ForEach(link =>
																			{
																				emailBody.Append("<li>");
																				emailBody.AppendFormat("BAD URL: {0}<br/>", link.BadURL);
																				emailBody.AppendFormat("REFERER: {0}<br/>", link.Referer);
																				emailBody.AppendFormat("USER AGENT: {0}<br/>", link.UserAgent);
																				emailBody.AppendFormat("<br/><a href='http://{0}/FixBrokenLink.aspx?badURL={1}'>FIX THIS URL >></a>", siteHostName, link.BadURL);
																				emailBody.Append("<br/></li>");
																				link.IsNotified = true;
																			});
					emailBody.Append("</ol>");
					emailBody.Append("</li></ul>");
				});
				emailBody.AppendFormat(@"<p style='font-size: 12px; color: silver;'>Email was sent to: {0}</p>", emailRecipients);
				SendMail(emailRecipients, Config.FromEmail, emailSubject, emailBody.ToString());
				conn.Update<BrokenLink>(brokenLinks);
				Log.LogInformation("Composite.Tools.LegacyUrlHandler", "Broken URLs report was sent to " + emailRecipients);
			}
		}

		private static string GetIPAddress(HttpRequest request)
		{
			// Look for a proxy address first
			string ipaddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

			// If there is no proxy, get the standard remote address
			if (String.IsNullOrWhiteSpace(ipaddress))
				ipaddress = request.ServerVariables["REMOTE_ADDR"];

			return ipaddress ?? "(undefined)";
		}

		private static bool RefererIsTrusted(string referer)
		{
			var result = true;
			Config.RefererBlackList.ForEach(blocked =>
			{
				result = result && !referer.Contains(blocked);
			}
			);
			return result;
		}

		private static bool UserAgentIsTrusted(string userAgent)
		{
			var result = true;
			Config.UserAgentBlackList.ForEach(blocked =>
			{
				result = result && !userAgent.Contains(blocked);
			}
			);
			return result;
		}

		private static bool IPAddressIsInBlackList(string ipAddress)
		{
			return Config.IPBlackList.Contains(ipAddress);
		}

		public static void SendMail(string to, string from, string subject, string body)
		{
			try
			{
				var mail = new MailMessage();
				mail.To.Add(to);
				mail.From = new MailAddress(from);
				mail.Subject = subject;
				mail.Body = body;
				mail.IsBodyHtml = true;
				mail.BodyEncoding = Encoding.UTF8;
				mail.SubjectEncoding = Encoding.UTF8;
				var smtpMail = new SmtpClient();
				smtpMail.Send(mail);
			}
			catch (Exception ex)
			{
				LoggingService.LogInformation("Composite.Tools.LegacyUrlHandler.SendMail", "Error while sending Email. Check mail settings in web.config file.");
				LoggingService.LogError("Composite.Tools.LegacyUrlHandler.SendMail", ex);
			}
		}
	}
}