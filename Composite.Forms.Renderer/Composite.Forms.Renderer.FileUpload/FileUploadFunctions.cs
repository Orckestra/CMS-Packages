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
using System.Linq;
using System.IO;

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
					var file = page.Request.Files[fileName];

					if (file.FileName == string.Empty) continue;

					var filename = NormalizeFilename(file.FileName);

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

		private static string NormalizeFilename(string filename)
		{
			foreach (var pair in new Dictionary<string, string> { { "æ", "ae" }, { "Æ", "AE" }})
			{
				filename = filename.Replace(pair.Key, pair.Value);
			};
			filename = string.Join(string.Empty, filename
						.Select(d => d.ToString().Normalize(NormalizationForm.FormKD))
						.Select(d => d.Length == 1 ? (d[0] < 128 ? d : "_") : string.Join(string.Empty, d.Where(c => c < 128))));
			filename = filename.Trim(Path.GetInvalidFileNameChars());

			return filename;
		}

	}
}
