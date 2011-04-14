using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Composite.Data.Types;

namespace Composite.Media.IssuuPlayer.Plugins
{
	public class IssuuPlayerFacade
	{
		public static XElement UploadDocument(IMediaFile media)
		{
			var data = IssuuApi.NewQuery("issuu.document.upload");
			data.Add("name", media.GetName());
			data.Add("title", media.Title);
			IssuuApi.Sign(data);

			var document = GetDocument(WebRequestFacade.UploadFileEx("http://upload.issuu.com/1_0", media.GetReadStream(), media.GetOrgName(), "file", data));
			document.SetAttributeValue("publishing", "true");
			return document;
		}

		public static XElement DeleteDocument(IMediaFile media)
		{
			ServicePoint servicePoint = ServicePointManager.FindServicePoint(new Uri("http://api.issuu.com/1_0"));
			servicePoint.Expect100Continue = false;

			var data = IssuuApi.NewQuery("issuu.document.delete");
			data.Add("names", media.GetName());
			IssuuApi.Sign(data);

			var client = new System.Net.WebClient();
			byte[] responseArray = client.UploadValues("http://api.issuu.com/1_0", data);
			return GetDocument(Encoding.ASCII.GetString(responseArray));
		}

		public static XElement GetDocument(IMediaFile media)
		{
			ServicePoint servicePoint = ServicePointManager.FindServicePoint(new Uri("http://api.issuu.com/1_0"));
			servicePoint.Expect100Continue = false;
			System.Net.ServicePointManager.Expect100Continue = false;
			var data = IssuuApi.NewQuery("issuu.documents.list");
			data.Add("orgDocName", media.GetOrgName());
			IssuuApi.Sign(data);

			var client = new System.Net.WebClient();
			byte[] responseArray = client.UploadValues("http://api.issuu.com/1_0", data);
			return GetDocument(Encoding.ASCII.GetString(responseArray));
		}

		private static XElement GetDocument(string response)
		{
			XElement result = null;
			try {
				result = XElement.Parse(response);
			}
			catch(Exception e){
				throw new InvalidOperationException("Server return invalid xml", e);
			}
			var error = result.XPathSelectElement("//error");
			if(error != null)
			{
				throw new InvalidOperationException(string.Format("{0}: {1}", error.AttributeValue("code"), error.AttributeValue("message")));
			}
			return result.XPathSelectElement("//document");
		}
	}

	public static class IssuuPlayerFacadeExtensions
	{
		public static string GetOrgName(this IMediaFile media)
		{
			return string.Format("{0}.pdf", media.GetName()); 
		}

		public static string GetName(this IMediaFile media)
		{
			return Regex.Replace(media.Id.ToString(), @"[^\w\d]", "");
		}

		public static string AttributeValue(this XElement element, XName name)
		{
			return element.Attributes(name).Select(d => d.Value).FirstOrDefault();
		}




	}
}