using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Composite.Core.IO;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Tools.LegacyUrlHandler
{
	public class LegacyUrlHandlerFacade
	{
		public static string XmlFileName = HttpContext.Current.Server.MapPath("~/App_Data/LegacyUrlMappings.xml");

		public static void WriteXml(Dictionary<string, string> mappings)
		{
			var doc = new XDocument();
			var xe = new XElement("Mappings");
			foreach (var s in mappings)
			{
				xe.Add(new XElement("Mapping", new XAttribute("OldPath", s.Key), new XAttribute("NewPath", s.Value)));
			}

			doc.Add(xe);

			//TODO: use XmlWriterUtils.Create() when it's will be public. Now it's internal.
			var writer = new XmlTextWriter(XmlFileName, null)
			             	{
			             		Formatting = Formatting.Indented,
			             		Indentation = 1,
			             		IndentChar = '\t'
			             	};

			doc.Save(writer);
			writer.Close();
		}

		public static Dictionary<string, string> GetMappingsFromXml()
		{
			var mappings = new Dictionary<string, string>();

			if (C1File.Exists(XmlFileName))
			{
				var doc = XDocumentUtils.Load(XmlFileName).Descendants("Mapping");

				foreach (var m in doc)
				{
					var oldPath = m.Attribute("OldPath").Value;
					var newPath = m.Attribute("NewPath").Value;

					if (!mappings.ContainsKey(oldPath))
					{
						mappings.Add(oldPath, newPath);
					}
				}
			}
			return mappings;
		}

		public static Dictionary<string, string> GetMappingsFromSiteMap()
		{
			var mappings = new Dictionary<string, string>();

			foreach (var dataScopeIdentifier in DataFacade.GetSupportedDataScopes(typeof(IPage)).Where(c => c.Name == DataScopeIdentifier.Public.Name))
			{
				foreach (var cultureInfo in DataLocalizationFacade.ActiveLocalizationCultures.ToArray())
				{
					using (new DataScope(dataScopeIdentifier, cultureInfo))
					{
						var siteMap = PageStructureInfo.GetUrlToIdLookup();
						foreach (var x in siteMap)
						{
							var id = x.Value.ToString();
							var url = x.Key;
							if (!mappings.ContainsKey(url))
							{
								mappings.Add(url, id);
							}
						}
					}
				}
			}

			return mappings;
		}
	}
}
