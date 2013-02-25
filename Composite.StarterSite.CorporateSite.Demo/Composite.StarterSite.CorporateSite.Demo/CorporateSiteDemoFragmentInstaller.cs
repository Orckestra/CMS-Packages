using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Composite.Core;
using Composite.Core.IO;
using Composite.Core.PackageSystem;
using Composite.Core.PackageSystem.PackageFragmentInstallers;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.StarterSite.CorporateSite.Demo
{
	public class CorporateSiteDemoFragmentInstaller : BasePackageFragmentInstaller
	{
		private IList<DatatypeUpdate> _datatypeUpdate;
		internal static readonly string ContainerElementName = "Files";
		internal static readonly string ElementName = "Source";
		internal static readonly string SourceFileAttributeName = "file";

		public override IEnumerable<PackageFragmentValidationResult> Validate()
		{
			Log.LogError("Validate", "Start");
			var validationResult = new List<PackageFragmentValidationResult>();

			if (Configuration.Count(f => f.Name == SourceFileAttributeName) > 1)
			{
				validationResult.Add(new PackageFragmentValidationResult(PackageFragmentValidationResultType.Fatal, "No files found!"));

				return validationResult;
			}

			IEnumerable<XElement> filesElement = this.Configuration.Where(f => f.Name == ContainerElementName);

			_datatypeUpdate = new List<DatatypeUpdate>();
			foreach (XElement fileElement in filesElement.Elements(ElementName))
			{
				XAttribute sourceAttribute = fileElement.Attribute(SourceFileAttributeName);

				if (sourceAttribute == null)
				{
					validationResult.Add(new PackageFragmentValidationResult(PackageFragmentValidationResultType.Fatal, "Missing Attribute", fileElement));

					continue;
				}

				var datatypeUpdate = new DatatypeUpdate
				{
					SourceFile = sourceAttribute.Value,
				};

				_datatypeUpdate.Add(datatypeUpdate);
			}

			if (validationResult.Count > 0)
			{
				_datatypeUpdate = null;
			}

			return validationResult;
		}

		public override IEnumerable<XElement> Install()
		{
			if (_datatypeUpdate == null) throw new InvalidOperationException("CorporateSiteDemoFragmentInstaller has not been validated");

			//Composite.Media.ImageGallery.Slimbox2
			Package.InstallPackage(new Guid("e41ec59b-1e4c-42e9-8682-21806ae3072b"));

			foreach (var datatypeUpdate in _datatypeUpdate)
			{
				string targetXmlFile = PathUtil.Resolve(datatypeUpdate.SourceFile);

				using (Stream stream = this.InstallerContext.ZipFileSystem.GetFileStream(datatypeUpdate.SourceFile))
				{
					XElement source = XElement.Load(stream);
					//XDocument target = XDocumentUtils.Load(targetXmlFile);

					foreach (var dataScopeIdentifier in DataFacade.GetSupportedDataScopes(typeof (IPagePlaceholderContent)))
					{
						var cultures = DataConnection.AllLocales;
						foreach (var cultureInfo in cultures)
						{
							using (new DataScope(dataScopeIdentifier, cultureInfo))
							{
								foreach (var data in source.Elements())
								{
									Guid pageId = new Guid(data.Attribute("PageId").Value);
									string placeHolderId = data.Attribute("PlaceHolderId").Value;
									string content = data.Attribute("Content").Value;
									var placeHolder = DataFacade.GetData<IPagePlaceholderContent>().FirstOrDefault(p => p.PageId == pageId && p.PlaceHolderId == placeHolderId);

									if (placeHolder != null)
									{
										placeHolder.Content = content;
										DataFacade.Update(placeHolder);
									}
									else
									{
										placeHolder = DataFacade.BuildNew<IPagePlaceholderContent>();
										placeHolder.PageId = pageId;
										placeHolder.PlaceHolderId = placeHolderId;
										placeHolder.Content = content;
										placeHolder.CultureName = cultureInfo.Name;
										placeHolder.SourceCultureName = cultureInfo.Name;
										placeHolder.PublicationStatus = "published";
										DataFacade.AddNew(placeHolder);
									}
								}
							}
						}
					}
				}
			}
			return new[] { this.Configuration.FirstOrDefault() };
		}
	}

	public class CorporateSiteDemoFragmentUninstaller : BasePackageFragmentUninstaller
	{
		public override IEnumerable<PackageFragmentValidationResult> Validate()
		{
			yield break;
		}

		public override void Uninstall()
		{
			return;
		}
	}

	public sealed class DatatypeUpdate
	{
		public string SourceFile { get; set; }
	}
}
