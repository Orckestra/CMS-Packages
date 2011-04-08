using System;
using System.Net.Mail;

namespace Composite.Community.TellAFriend
{
	public class Email
	{
		public static void Send(MailAddress from, MailAddress to, MailAddress bcc, MailAddress cc, string subject, string body, bool isBodyHtml, MailPriority priority)
		{
			var mailMessage = new MailMessage();

			mailMessage.From = from;
			mailMessage.To.Add(to);

			if (bcc != null)
				mailMessage.Bcc.Add(bcc);

			if (cc != null)
				mailMessage.CC.Add(cc);

			mailMessage.Subject = subject;
			mailMessage.Body = body;
			mailMessage.IsBodyHtml = isBodyHtml;
			mailMessage.Priority = priority;

			var mSmtpClient = new SmtpClient();
			mSmtpClient.Send(mailMessage);
		}
	}
}
