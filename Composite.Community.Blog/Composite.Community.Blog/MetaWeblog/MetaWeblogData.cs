using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using Composite.Core.Types;
using Composite.Core.WebClient;
using Composite.Data;
using Composite.Data.Types;
using CookComputing.XmlRpc;

namespace Composite.Community.Blog.MetaWeblog
{

	internal class MetaWeblogData : IDisposable
	{
		public Authors Author { get; private set; }
		private DataScope scope;

		public MetaWeblogData(string username, string password)
		{
			scope = new DataScope(DataScopeIdentifier.Administrated, DataLocalizationFacade.DefaultLocalizationCulture);
			Author = GetAuthor(username, password);
		}

		protected Authors GetAuthor(string username, string password)
		{
			Verify.ArgumentNotNullOrEmpty(username, "username");
			Verify.ArgumentNotNullOrEmpty(password, "password");
			var author = DataFacade.GetData<Authors>(d => d.Name == username && d.Password == password).FirstOrDefault();
			if (author != null)
			{
				return author;
			}
			throw new XmlRpcFaultException(0, "User is not valid!");
		}

		public IEnumerable<IPage> GetBlogPages()
		{
			foreach (var point in DataFacade.GetData<IDataItemTreeAttachmentPoint>(d => d.TreeId == "Composite.Community.Blog.Entries.xml"))
			{
				Type type = TypeManager.TryGetType(point.InterfaceType);
				if (type == typeof(IPage))
				{
					Guid pageId;
					if (Guid.TryParse(point.KeyValue, out pageId))
					{
						var page = PageManager.GetPageById(pageId, true);
						if (page != null)
							yield return page;
					}
				}
			}
		}

		public string FullMediaUrl(string s)
		{
			var content = XElement.Parse(s);
			foreach (var img in content.Descendants(content.Name.Namespace + "img"))
			{
				try
				{

					var src = img.Attribute("src");
					if (src != null)
					{
						var match = Regex.Match(src.Value, @"Renderers/ShowMedia\.ashx.*");
						if (match.Success)
						{
							img.SetAttributeValue("src", BlogFacade.GetFullPath(UrlUtils.PublicRootPath + "/" + match.Value));
						}
					}
				}
				catch { }
			}
			return content.ToString();
		}

		public string RelativeMediaUrl(XElement content)
		{
			foreach (var src in 
				content.Descendants(content.Name.Namespace + "img").Attributes("src")
				.Union(content.Descendants(content.Name.Namespace + "a").Attributes("href")))
			{
					var match = Regex.Match(src.Value, @"Renderers/ShowMedia\.ashx.*");
					if (match.Success)
					{
						if(new Uri(src.Value).Host == HttpContext.Current.Request.Url.Host)
							src.Value = UrlUtils.PublicRootPath + "/" + match.Value;
					}
			}
			return content.ToString();
		}

		public void Dispose()
		{
			if (scope != null) ((IDisposable)scope).Dispose();
		}
	}
}