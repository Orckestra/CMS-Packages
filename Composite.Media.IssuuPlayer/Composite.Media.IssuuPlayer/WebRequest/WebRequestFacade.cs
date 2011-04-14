using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace Composite.Media.IssuuPlayer
{
	public class WebRequestFacade
	{
		public static string UploadFileEx(string url, Stream fileStream, string filename, string fileFormName, NameValueCollection nvc)
		{
			long length = 0;
			string boundary = "----------------------------" +
			DateTime.Now.Ticks.ToString("x");

			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
			httpWebRequest.Method = "POST";
			httpWebRequest.KeepAlive = true;
			httpWebRequest.Credentials =
			CredentialCache.DefaultCredentials;

			Stream memStream = new MemoryStream();
			byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
			string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
			foreach (string key in nvc.Keys)
			{
				string formitem = string.Format(formdataTemplate, key, nvc[key]);
				byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
				memStream.Write(formitembytes, 0, formitembytes.Length);
			}
			memStream.Write(boundarybytes, 0, boundarybytes.Length);

			string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
			string header = string.Format(headerTemplate, fileFormName, filename);
			byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
			memStream.Write(headerbytes, 0, headerbytes.Length);

			byte[] buffer = new byte[1024];
			int bytesRead = 0;
			while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
			{
				memStream.Write(buffer, 0, bytesRead);
			}
			memStream.Write(boundarybytes, 0, boundarybytes.Length);

			httpWebRequest.ContentLength = memStream.Length;
			Stream requestStream = httpWebRequest.GetRequestStream();
			memStream.Position = 0;
			byte[] tempBuffer = new byte[memStream.Length];
			memStream.Read(tempBuffer, 0, tempBuffer.Length);
			memStream.Close();
			requestStream.Write(tempBuffer, 0, tempBuffer.Length);
			requestStream.Close();

			WebResponse webResponse = httpWebRequest.GetResponse();
			Stream stream2 = webResponse.GetResponseStream();
			StreamReader reader2 = new StreamReader(stream2);
			return reader2.ReadToEnd();
		}
	}

}