using System;
using System.Linq;
using System.Xml.Linq;
using Composite.Core.Application;
using Composite.Data;
using Composite.Data.Types;
using ICSharpCode.SharpZipLib.Zip;


namespace Composite.Tools.PackageServer
{
	[ApplicationStartup]
	public class PackageServerFacade
	{
		public static void OnBeforeInitialize()
		{

		}

		public static void OnInitialized()
		{
			DataEventSystemFacade.SubscribeToDataBeforeAdd<Package>(OnDataChanged, true);
			DataEventSystemFacade.SubscribeToDataBeforeUpdate<Package>(OnDataChanged, true);
		}

		private static void OnDataChanged(object sender, DataEventArgs args)
		{
			using (DataConnection connection = new DataConnection())
			{
				var package = args.Data as Package;
				if (package != null)
				{
					var media = connection.Get<IMediaFile>().Where(d => d.KeyPath == package.PackageFile).FirstOrDefault();
					if (media != null)
					{
						try
						{
							var zipInputStream = new ZipInputStream(media.GetReadStream());
							ZipEntry zipEntry = null;
							while ((zipEntry = zipInputStream.GetNextEntry()) != null)
							{
								if (zipEntry.Name.Equals("install.xml", StringComparison.CurrentCultureIgnoreCase))
								{
									var installXml = XDocument.Load(zipInputStream);
									XNamespace mi = "http://www.composite.net/ns/management/packageinstaller/1.0";

									var packageInformation = installXml.Root.Element(mi + "PackageInformation");
									var packageRequirements = installXml.Root.Element(mi + "PackageRequirements");
									package.Name = packageInformation.AttributeValue("name");
									package.GroupName = packageInformation.AttributeValue("groupName");
									package.PackageVersion = packageInformation.AttributeValue("version");
									package.Author = packageInformation.AttributeValue("author");
									package.ReadMoreUrl = packageInformation.AttributeValue("readMoreUrl");
									package.PackageId = new Guid(packageInformation.AttributeValue("id"));
									package.Description = packageInformation.ElementValue("Description");
									package.TechnicalDetails = packageInformation.ElementValue("TechnicalDetails");
									package.MinCompositeVersionSupported = packageRequirements.AttributeValue("minimumCompositeVersion");
									package.MaxCompositeVersionSupported = packageRequirements.AttributeValue("maximumCompositeVersion");
								};
							}
						}
						catch { }
					}
				}
			}
		}
	}

	internal static class PackageServerExtensions
	{
		public static string AttributeValue(this XElement element, XName attributeName)
		{
			return element.Attributes(attributeName).Select(d => d.Value).FirstOrDefault();
		}
		public static string ElementValue(this XElement element, XName elementName)
		{
			return element.Elements(elementName).Select(d => d.Value).FirstOrDefault();
		}
	}
}