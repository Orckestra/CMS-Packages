using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Functions;

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

		private static IEnumerable<XElement> GetPages(SwitcherMode mode, SwitcherFormat format, bool includeQuery)
		{
			Func<CultureInfo, XNode> cultureFormat = c => null;
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
			foreach (CultureInfo culture in DataLocalizationFacade.ActiveLocalizationCultures)
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
							break;
						case SwitcherMode.TranslatedPages:
							match = PageStructureInfo.GetSiteMap().DescendantsAndSelf().Where(f => f.Attribute("Id") != null && f.Attribute("Id").Value == PageRenderer.CurrentPageId.ToString()).FirstOrDefault();
							break;
						case SwitcherMode.TranslatedOrHomePages:
							match = PageStructureInfo.GetSiteMap().DescendantsAndSelf().Where(f => f.Attribute("Id") != null && f.Attribute("Id").Value == PageRenderer.CurrentPageId.ToString()).FirstOrDefault();
							if (match == null)
								match = PageStructureInfo.GetSiteMap().FirstOrDefault();
							break;
						default:
							break;
					}
					if (match == null)
					{
						continue;
					}

					var url = match.Attribute("URL").Value;
					var urlBuilder = PageUrl.Parse(url).Build();
					if (includeQuery && urlBuilder != null)
					{
						var queryParameters = System.Web.HttpContext.Current.Request.QueryString;
						urlBuilder.AddQueryParameters(queryParameters);
						url = urlBuilder.ToString();
					}
					annotatedMatch = new XElement("LanguageVersion"
							, new XAttribute("Culture", culture.Name)
							, new XAttribute("CurrentCulture", culture.Equals(Thread.CurrentThread.CurrentCulture))
							, new XAttribute("Id", match.Attribute("Id").Value)
							, new XAttribute("Title", match.Attribute("Title").Value)
							, (match.Attribute("MenuTitle") == null ? null : new XAttribute("MenuTitle", match.Attribute("MenuTitle").Value))
							, new XAttribute("UrlTitle", match.Attribute("UrlTitle").Value)
							, new XAttribute("Description", match.Attribute("Description").Value)
							, new XAttribute("URL", url)
							, cultureFormat(culture)
							);
				}
				yield return annotatedMatch;
			}
		}
	}
}