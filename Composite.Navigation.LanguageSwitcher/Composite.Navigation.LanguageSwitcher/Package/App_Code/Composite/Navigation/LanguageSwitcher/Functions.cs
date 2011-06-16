using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml.Linq;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Functions;
using Composite.Data;

namespace Composite.Navigation.LanguageSwitcher
{
	public class Functions
	{
		public enum SwitcherMode
		{
			HomePages,
			TranslatedPages,
			TranslatedOrHomePages,
		}

		public enum SwitcherFormat
		{
			DisplayName,
			EnglishName,
			Name,
			NativeName,
			ThreeLetterISOLanguageName,
			ThreeLetterWindowsLanguageName,
			TwoLetterISOLanguageName,
			Image,
			Empty,
		}

		public static IEnumerable<string> GetSwitcherModes()
		{
			return Enum.GetNames(typeof(SwitcherMode)).AsEnumerable();
		}

		public static IEnumerable<string> GetSwitcherFormats()
		{
			return Enum.GetNames(typeof(SwitcherFormat)).AsEnumerable();
		}

		[FunctionParameterDescription("mode", "Mode", "")]
		[FunctionParameterDescription("format", "Format", "")]
		[FunctionParameterDescription("includeQuery", "IncludeQuery", "")]
		public static IEnumerable<XElement> GetPagesInfo(string mode, string format, bool includeQuery)
		{
			var switcherMode = (SwitcherMode)(Enum.Parse(typeof(SwitcherMode), mode, true));
			var switcherFormat = (SwitcherFormat)(Enum.Parse(typeof(SwitcherFormat), format, true));
			return GetPages(switcherMode, switcherFormat, includeQuery);
		}

		private static HttpRequest Request
		{
			get { return HttpContext.Current.Request; }
		} 

		private static IEnumerable<XElement> GetPages(SwitcherMode mode, SwitcherFormat format, bool includeQuery)
		{
			Func<CultureInfo, XNode> cultureFormat = culture => null;
			Func<string, object, XAttribute> getXAttribute = (name, value) => value == null ? null : new XAttribute(name, value);
			switch (format)
			{
				case SwitcherFormat.DisplayName:
					cultureFormat = c => new XText((c.CultureTypes & CultureTypes.SpecificCultures) != 0 ? c.Parent.DisplayName : c.DisplayName);
					break;
				case SwitcherFormat.EnglishName:
					cultureFormat = c => new XText((c.CultureTypes & CultureTypes.SpecificCultures) != 0 ? c.Parent.EnglishName : c.EnglishName);
					break;
				case SwitcherFormat.Name:
					cultureFormat = c => new XText(c.Name);
					break;
				case SwitcherFormat.NativeName:
					cultureFormat = c => new XText((c.CultureTypes & CultureTypes.SpecificCultures) != 0 ? c.Parent.NativeName : c.NativeName);
					break;
				case SwitcherFormat.ThreeLetterISOLanguageName:
					cultureFormat = c => new XText(c.ThreeLetterISOLanguageName);
					break;
				case SwitcherFormat.ThreeLetterWindowsLanguageName:
					cultureFormat = c => new XText(c.ThreeLetterWindowsLanguageName);
					break;
				case SwitcherFormat.TwoLetterISOLanguageName:
					cultureFormat = c => new XText(c.TwoLetterISOLanguageName);
					break;
				case SwitcherFormat.Image:
					cultureFormat = c =>
						new XElement(Namespaces.Xhtml + "img",
						new XAttribute("src", string.Format("~/Frontend/Composite/Navigation/LanguageSwitcher/Images/{0}.png", c.Name)),
						new XAttribute("alt", ""),
						new XAttribute("border", "0"),
						new XAttribute("width", "16"),
						new XAttribute("height", "11")
						);
					break;
				case SwitcherFormat.Empty:
				default:
					break;
			}

			// Grab all active languages...
			foreach (var culture in DataLocalizationFacade.ActiveLocalizationCultures)
			{
				XElement annotatedMatch;

				// enter the 'data scope' of the next language
				using (new DataScope(culture))
				{
					XElement match = null;
					switch (mode)
					{
						case SwitcherMode.HomePages:
							match = PageStructureInfo.GetSiteMap().FirstOrDefault();
							includeQuery = false;
							break;
						case SwitcherMode.TranslatedPages:
							match = PageStructureInfo.GetSiteMap().DescendantsAndSelf().Where(f => f.GetAttributeValue("Id") == PageRenderer.CurrentPageId.ToString()).FirstOrDefault();
							break;
						case SwitcherMode.TranslatedOrHomePages:
							match = PageStructureInfo.GetSiteMap().DescendantsAndSelf().Where(f => f.GetAttributeValue("Id") == PageRenderer.CurrentPageId.ToString()).FirstOrDefault();
							if(match == null)
							{
								match = PageStructureInfo.GetSiteMap().FirstOrDefault();
								includeQuery = false;
							}
							break;
						default:
							break;
					}
					if (match == null)
					{
						continue;
					}

					var url = match.GetAttributeValue("URL");

					if (includeQuery && Request.QueryString.Count > 0)
					{
						url = string.Format("{0}?{1}", url, Request.Url.Query);
					}
					annotatedMatch = new XElement("LanguageVersion"
							, getXAttribute("Culture", culture.Name)
							, getXAttribute("CurrentCulture", culture.Equals(Thread.CurrentThread.CurrentCulture))
							, getXAttribute("Id", match.GetAttributeValue("Id"))
							, getXAttribute("Title", match.GetAttributeValue("Title"))
							, getXAttribute("MenuTitle", match.GetAttributeValue("MenuTitle"))
							, getXAttribute("UrlTitle", match.GetAttributeValue("UrlTitle"))
							, getXAttribute("Description", match.GetAttributeValue("Description"))
							, getXAttribute("URL", url)
							, cultureFormat(culture)
							);
				}
				yield return annotatedMatch;
			}
		}
	}
	
}