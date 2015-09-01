using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Composite.Core.Extensions;
using Composite.Core.IO;
using Composite.Core.Routing;
using Composite.Core.WebClient;
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

		public static void WriteXmlElement(string key, string value)
		{
			if (!C1File.Exists(XmlFileName))
				WriteXml(new Dictionary<string, string> { { key, value } });
			else
			{
				var doc = XDocument.Load(XmlFileName);
				var root = doc.Root;
				if (root != null)
				{
					var element = root.Elements("Mapping").FirstOrDefault(el =>
																			{
																				var oldPathAttr = el.Attribute("OldPath");
																				return oldPathAttr != null && oldPathAttr.Value == key;
																			});
					if (element != null)
					{
						var newPathAttr = element.Attribute("NewPath");
						if (newPathAttr != null) newPathAttr.Value = value;
					}
					else
					{
						root.Add(new XElement("Mapping", new XAttribute("OldPath", key), new XAttribute("NewPath", value)));
					}
					doc.Save(XmlFileName);
				}
			}
		}


	    public class UrlMappings
	    {
	        public Dictionary<string, string> RawLinks { get; set; }
            public Dictionary<string, string> RelativeLinks { get; set; }
            public Dictionary<string, Dictionary<string, string>> RelativeLinksPerHostname { get; set; }

	        public string GetMappedUrl(string hostname, string relativePath)
	        {
	            string result;
	            if (RelativeLinks.TryGetValue(relativePath, out result))
	            {
	                return result;
	            }

	            Dictionary<string, string> hostnameBindings;
	            if (RelativeLinksPerHostname.TryGetValue(hostname, out hostnameBindings)
	                && hostnameBindings.TryGetValue(relativePath, out result))
	            {
	                return result;
	            }

                return null;
	        }
	    }

        public static UrlMappings GetMappingsFromXml()
		{
		    var result = new UrlMappings
		    {
		        RawLinks = new Dictionary<string, string>(),
                RelativeLinks = new Dictionary<string, string>(),
                RelativeLinksPerHostname = new Dictionary<string, Dictionary<string, string>>()
		    };

			if (C1File.Exists(XmlFileName))
			{
				var doc = XDocumentUtils.Load(XmlFileName).Descendants("Mapping");

				foreach (var m in doc)
				{
					var oldPath = m.Attribute("OldPath").Value;
					var newPath = m.Attribute("NewPath").Value;

					if (!result.RawLinks.ContainsKey(oldPath))
					{
                        result.RawLinks.Add(oldPath, newPath);
					}

				    if (oldPath.StartsWith("http://") || oldPath.StartsWith("https://") || oldPath.StartsWith("//"))
				    {
				        int hostnameOffset = oldPath.IndexOf("//") + 2;
				        int hostnameEndOffset = oldPath.IndexOf('/', hostnameOffset);

				        if (hostnameEndOffset > 0)
				        {
				            string hostname = oldPath.Substring(hostnameOffset, hostnameEndOffset - hostnameOffset);
				            string relativeUrl = oldPath.Substring(hostnameEndOffset);

				            Dictionary<string, string> linksPerHostname;
				            if (!result.RelativeLinksPerHostname.TryGetValue(hostname, out linksPerHostname))
				            {
				                result.RelativeLinksPerHostname.Add(hostname, linksPerHostname = new Dictionary<string, string>());
				            }

				            if (!linksPerHostname.ContainsKey(relativeUrl))
				            {
				                linksPerHostname.Add(relativeUrl, newPath);
				            }
				        }
				    }
                    else
                    {
                        result.RelativeLinks.Add(oldPath, newPath);
                    }
                }
			}
            return result;
		}

		public static Dictionary<string, Guid> GetMappingsFromSiteMap()
		{
			var result = new Dictionary<string, Guid>();

		    var urlSpace = new UrlSpace();

            // TODO: support for multiple languages as well
            var hostnameBindings = DataFacade.GetData<IHostnameBinding>().ToList();

		    var rootToHostnameMap = hostnameBindings.ToDictionary(h => h.HomePageId, h => h.Hostname);

			foreach (var cultureInfo in DataLocalizationFacade.ActiveLocalizationCultures.ToArray())
			{
                using (new DataScope(DataScopeIdentifier.Public, cultureInfo))
                {
                    var pages = DataFacade.GetData<IPage>().ToList();

					foreach (var page in pages)
					{
                        var url = PageUrls.BuildUrl(new PageUrlData(page), UrlKind.Public, urlSpace);
						if (url != null && !result.ContainsKey(url))
						{
							result.Add(url, page.Id);
						}

					    var rootPageId = GetRootPageId(page.Id);

					    string hostname;
                        if (rootToHostnameMap.TryGetValue(rootPageId, out hostname))
					    {
					        if (urlSpace.Hostname != hostname)
					        {
                                var hostnameBasedUrlSpace = new UrlSpace { ForceRelativeUrls = false, Hostname = hostname };
                                url = PageUrls.BuildUrl(new PageUrlData(page), UrlKind.Public, hostnameBasedUrlSpace);
                            }

					        if (url != null)
					        {
					            if (url.StartsWith("/"))
					            {
					                url = "http://{0}{1}".FormatWith(hostname, url);
					            }

					            if (!result.ContainsKey(url))
					            {
					                result.Add(url, page.Id);
					            }
					        }
					    }
					}
				}
			}
			
			return result;
		}

	    private static Guid GetRootPageId(Guid id)
	    {
	        Guid currentId = id;
	        Guid parentId = PageManager.GetParentId(id);

	        while (parentId != Guid.Empty)
	        {
	            currentId = parentId;
	            parentId = PageManager.GetParentId(currentId);
	        }

	        return currentId;
	    }

	    internal static void RedirectToLoginPage()
	    {
	        var context = HttpContext.Current;

	        var url = context.Request.RawUrl;

	        string loginUrl = UrlUtils.PublicRootPath + "/Composite/Login.aspx?ReturnUrl=" + HttpUtility.UrlEncode(url, Encoding.UTF8);

            context.Response.Redirect(loginUrl, true);
	    }
	}
}
