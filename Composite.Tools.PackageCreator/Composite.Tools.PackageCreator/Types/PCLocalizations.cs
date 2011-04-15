using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.Tools.PackageCreator.ElementProvider;
using Composite.C1Console.Security;
using Composite.C1Console.Users;
using System.Xml.XPath;
using Composite.Core.Types;
using Composite.Core.ResourceSystem;
using System.Xml.Linq;

namespace Composite.Tools.PackageCreator.Types
{
	[PCCategory("Localizations")]
	internal sealed class PCLocalizations : SimplePackageCreatorItem, IPackagable
	{
		private static readonly XNamespace xsl = "http://www.w3.org/1999/XSL/Transform";

		internal Type XmlStringResourceProviderType
		{
			get
			{
				return TypeManager.TryGetType("Composite.Plugins.ResourceSystem.XmlStringResourceProvider.XmlStringResourceProvider, Composite");
			}
		}

		public PCLocalizations(string name)
			: base(name)
		{
		}

		public override ResourceHandle ItemIcon
		{
			get
			{
				return new ResourceHandle("Composite.Icons", "localization-element-localeitem"); 
			}
		}

		public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
		{
			if (entityToken is PackageCreatorPackageElementProviderEntityToken)
			{
				yield return new PCLocalizations(UserSettings.CultureInfo.ToString());
			}
		}

		public void Pack(PackageCreator creator)
		{

			var configuration = PackageCreatorFacade.GetConfigurationDocument();	
			
			foreach (var add in configuration.XPathSelectElements("/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add"))
			{
				var typeName = add.AttributeValue("type");
				Type type = TypeManager.TryGetType(typeName);
				if (type == XmlStringResourceProviderType)
				{
					var name = add.AttributeValue("name");
					foreach (var addCulture in add.XPathSelectElements(string.Format("Cultures/add[@cultureName='{0}']", this.Name)))
					{
						var xmlFile = addCulture.AttributeValue("xmlFile");
						creator.AddFile(xmlFile);
						creator.AddConfigurationXPath(string.Format(
							@"/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='{0}']/Cultures/add[@cultureName='{1}']", name, this.Name));
					}
					
				}
			
			}

			creator.AddConfigurationInstallTemplate(new XElement(xsl + "template",
									new XAttribute("match", "configuration/Composite.Core.Configuration.Plugins.GlobalSettingsProviderConfiguration/GlobalSettingsProviderPlugins/add"),
									new XElement(xsl + "copy",
										new XElement(xsl + "apply-templates",
											new XAttribute("select", "@*")),
										new XElement(xsl + "if",
											new XAttribute("test", string.Format("not(contains(@applicationCultureNames, '{0}'))", this.Name)),
											new XElement(xsl + "attribute",
												new XAttribute("name", "applicationCultureNames"),
												new XElement(xsl + "value-of",
													new XAttribute("select", string.Format("concat(@applicationCultureNames,',{0}')", this.Name))
												)
											)
										),
										new XElement(xsl + "apply-templates",
											new XAttribute("select", "node()"))
									)
								));
		}
	}
}
