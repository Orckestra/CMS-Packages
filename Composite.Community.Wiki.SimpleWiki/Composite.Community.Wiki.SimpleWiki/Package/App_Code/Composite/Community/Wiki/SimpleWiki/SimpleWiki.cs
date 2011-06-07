using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using Composite.Data;
using Composite.Data.Types;
using Composite.Core.Xml;
using System.Xml.Linq;
using Composite.Core.WebClient.Renderings.Template;


namespace Composite.Community.Wiki
{
	/// <summary>
	/// Summary description for SimpleWiki
	/// </summary>
	[WebService(Namespace = "http://www.composite.net/ns/management")]
	[System.Web.Script.Services.ScriptService]
	public class SimpleWiki : System.Web.Services.WebService
	{
		private List<string> invalidEnements = "script,meta,iframe".Split(',').ToList();

		public SimpleWiki()
		{
			//Uncomment the following line if using designed components 
			//InitializeComponent(); 
		}

		[WebMethod]
		[ScriptMethod]
		public string SavePageContent(string pageId, string placeholderId, string content)
		{
			Guid pageGuid = Guid.Parse(pageId);
			string contentHtml = HttpUtility.UrlDecode(content);
			contentHtml = HttpUtility.HtmlDecode(contentHtml.Replace("&amp;", "&amp;amp;").Replace("&lt;", "&amp;lt;").Replace("&gt;", "&amp;gt;"));

			string xhtmlDocumentWrapper = string.Format("<html xmlns='http://www.w3.org/1999/xhtml'><head></head><body>{0}</body></html>", contentHtml);
			XElement xElement = XElement.Parse(xhtmlDocumentWrapper);

			RemoveInvalidElements(xElement);

			contentHtml = string.Concat((xElement.Elements().Select(b => b.ToString())).ToArray());
			foreach (PublicationScope scope in Enum.GetValues(typeof(PublicationScope)))
			{
				using (DataConnection connection = new DataConnection(scope))
				{
					var phHolder = Composite.Data.PageManager.GetPlaceholderContent(pageGuid).Where(ph => ph.PlaceHolderId == placeholderId).SingleOrDefault();
					if (phHolder != null)
					{
						phHolder.Content = contentHtml;
						connection.Update<IPagePlaceholderContent>(phHolder);
					}
					else
					{
						var page = connection.Get<IPage>().Where(p => p.Id == pageGuid).SingleOrDefault();
						var templateInfo = TemplateInfo.GetRenderingPlaceHolders(page.TemplateId);
						foreach (var ph in templateInfo.Placeholders)
						{
							if (ph.Key == placeholderId)
							{
								IPagePlaceholderContent newPh = connection.CreateNew<IPagePlaceholderContent>();
								newPh.PageId = pageGuid;
								newPh.PlaceHolderId = placeholderId;
								newPh.Content = contentHtml;
								connection.Add<IPagePlaceholderContent>(newPh);
							}
						}
					}
				}
			}
			return "Success";
		}

		private void RemoveInvalidElements(XElement xElement)
		{
			foreach (var el in xElement.Elements())
			{
				if (invalidEnements.Contains(el.Name.LocalName))
					el.Remove();
				if (el.Elements().Count() > 0)
				{
					RemoveInvalidElements(el);
				}
			}
		}
	}
}
