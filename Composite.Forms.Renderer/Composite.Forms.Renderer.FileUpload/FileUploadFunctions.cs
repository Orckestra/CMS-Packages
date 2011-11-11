using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Xml.Linq;
using System.Xml.Xsl;
using Composite.Core.Logging;
using Composite.Data;
using Composite.Forms.Renderer;
using Composite.Functions;
using System.Text;

namespace Composite.Forms.Renderer.FileUpload
{
	public class FileUploadFunctions
	{
		[FunctionParameterDescription("ResponseText", "ResponseText", "ResponseText", "")]
		[FunctionParameterDescription("From", "From", "From")]
		[FunctionParameterDescription("To", "To", "To")]
		[FunctionParameterDescription("Cc", "Cc", "Cc", "")]
		[FunctionParameterDescription("Subject", "Subject", "Subject")]
		public static string SendEmail(string ResponseText, string From, string To, string Cc, string Subject)
		{
			var newDataField = typeof(FormsRenderer).GetField("newData", BindingFlags.NonPublic | BindingFlags.Static);
			IData newData = (IData)newDataField.GetValue(null);

			var page = HttpContext.Current.Handler as Page;
			var attachments = new List<Attachment>();
			if (page != null)
			{
				foreach (string fileName in page.Request.Files)
				{
					HttpPostedFile file = page.Request.Files[fileName];
					var filename = file.FileName;
					foreach(var pair in new Dictionary<char,char>{{'æ','e'}, {'ø','o'}, {'å','a'}, {'Æ','E'},{'Ø','O'},{'Å','A'}})
					{
						filename = filename.Replace(pair.Key, pair.Value);
					};
					attachments.Add(new Attachment(file.InputStream, filename, file.ContentType)
					{ 
						NameEncoding = Encoding.ASCII
					});
				}
			}

			XElement inputXml = FormsRenderer.GetXElement(newData);
			XDocument mailBody = new XDocument();

			XslCompiledTransform xslTransform = new XslCompiledTransform();
			xslTransform.Load(FormsRenderer.FormsRendererLocalPath + "Xslt/MailBody.xslt");

			using (var writer = mailBody.CreateWriter())
			{
				xslTransform.Transform(inputXml.CreateReader(), writer);
			}

			MailMessage msgMail = new MailMessage();
			try
			{
				msgMail.From = new MailAddress(From);
			}
			catch (Exception e)
			{
				LoggingService.LogError(string.Format("Mail sending(From: '{0}')", From), e.Message);
				return string.Empty;
			}
			try
			{
				msgMail.To.Add(To);
			}
			catch (Exception e)
			{
				LoggingService.LogError(string.Format("Mail sending(To: '{0}')", To), e.Message);
				return string.Empty;
			}
			if (!string.IsNullOrEmpty(Cc))
			{
				try
				{
					msgMail.CC.Add(Cc);
				}
				catch (Exception e)
				{
					LoggingService.LogError(string.Format("Mail sending(Cc: '{0}')", Cc), e.Message);
				}
			}

			foreach (var attachment in attachments)
			{
				try
				{
					msgMail.Attachments.Add(attachment);
				}
				catch (Exception e)
				{
					LoggingService.LogError(string.Format("Mail sending(Attachment: '{0}')", attachment.Name), e.Message);
				}
			}

			try
			{
				msgMail.Subject = Subject;
				msgMail.IsBodyHtml = true;
				msgMail.Body = mailBody.ToString();

				SmtpClient client = new SmtpClient();
				client.Send(msgMail);
			}
			catch (Exception e)
			{
				throw new InvalidOperationException("Unable to send mail. Please ensure that web.config has correct /configuration/system.net/mailSettings: " + e.Message);
			}
			return ResponseText;
		}

	}
}
