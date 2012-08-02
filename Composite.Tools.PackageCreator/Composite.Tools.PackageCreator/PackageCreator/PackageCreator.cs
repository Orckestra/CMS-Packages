using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Data.ProcessControlled;
using Composite.Data.Types;
using Composite.Core.IO;
using Composite.Tools.PackageCreator.Types;
using Composite.Core.Types;
using ICSharpCode.SharpZipLib.Zip;
using Utility.IO;

namespace Composite.Tools.PackageCreator
{
	public sealed class PackageCreator
	{
		public static readonly XNamespace mi = "http://www.composite.net/ns/management/packageinstaller/1.0";
		public static readonly XNamespace pc = "http://www.composite.net/ns/management/packagecreator/2.0";
		public static readonly XNamespace help = "http://www.composite.net/ns/help/1.0";
		private static readonly XNamespace xsl = "http://www.w3.org/1999/XSL/Transform";

		
		public static string InstallFilename { get { return "install.xml"; } }

		public static readonly string configDirectoryName = "Config";
		public static readonly string configInstallFileName = "Install.xsl";
		public static readonly string configUninstallFileName = "Uninstall.xsl";

		public static readonly XName orderingAttributeName = "Ordering";


		private string serviceDirectoryPath;
		private string packageDirectoryPath;
		private string zipDirectoryPath;
		private List<XElement> Files = new List<XElement>();
		private List<XElement> Directories = new List<XElement>();
		private List<XElement> XslFiles = new List<XElement>();
		private HashSet<string> ConfigurationXPaths = new HashSet<string>();
		private List<XElement> ConfigurationInstallXsltTemplates =new List<XElement>();

		private Dictionary<string, XElement> Datas = new Dictionary<string, XElement>();
		private DataFileDictionary DataFiles = new DataFileDictionary();
		private List<string> installDataTypeNamesList = new List<string>();
		private const string CREATOR_DRECTORY = "PackageCreator";
		private Dictionary<Type, int> dataPosition = new Dictionary<Type, int>();

		private const string blankXsl = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">

        <xsl:template match=""@* | node()"">
                <xsl:copy>
                        <xsl:apply-templates select=""@* | node()""/>
                </xsl:copy>
        </xsl:template>

</xsl:stylesheet>";


		private string packageName;
		public string PackageName
		{
			get
			{
				return packageName;
			}
		}
#warning TODO: Make work locales
		public enum LocaleActions
		{
			DefaultLocalesToAllLocales,
			DefaultLocalesToCurrentLocale,
			AllLocales
		}

		public LocaleActions LocaleAction
		{
			get
			{
				return LocaleActions.DefaultLocalesToCurrentLocale;
			}
		}

		internal PackageCreator(string path, string packageName)
		{
			this.serviceDirectoryPath = path;
			this.packageName = packageName;
			if (!Directory.Exists(serviceDirectoryPath))
			{
				Directory.CreateDirectory(serviceDirectoryPath);
			}

			BuildDataPosition();
		}

		#region Creation
		public string CreatePackagePack()
		{
			using (new DataScope(DataScopeIdentifier.Public))
			{
				#region Init
				if (packageName == string.Empty)
				{
					return "";
				}

				packageDirectoryPath = Path.Combine(serviceDirectoryPath, packageName);
				if (Directory.Exists(packageDirectoryPath))
				{
					Directory.Delete(packageDirectoryPath, true);
				}

				var dataPackageDirectoryPath = Path.Combine(serviceDirectoryPath, packageName + "_Data");

				zipDirectoryPath = Path.Combine(packageDirectoryPath, "Release");
				packageDirectoryPath = Path.Combine(packageDirectoryPath, "Package");

				Directory.CreateDirectory(packageDirectoryPath);
				Directory.CreateDirectory(zipDirectoryPath);

				if (Directory.Exists(dataPackageDirectoryPath))
				{
					FileSystem.copyDirectory(dataPackageDirectoryPath, packageDirectoryPath);
				}

				string packageConfigFilePath = Path.Combine(serviceDirectoryPath, packageName + ".xml");

				XDocument config = XDocument.Load(packageConfigFilePath);

				var rootConfigElement = config.Element(pc + "PackageCreator");
				if (rootConfigElement == null)
				{
					throw new InvalidOperationException("Root element of config PackageCreator 2.0 not found");
				}



				XElement blockElement;

				#endregion

				#region Init install.xml
				XDocument XPackage = new XDocument();
				XPackage.Declaration = new XDeclaration("1.0", "utf-8", "yes");

				XElement XPackageInstaller = new XElement(mi + "PackageInstaller");
				XPackageInstaller.Add(new XAttribute(XNamespace.Xmlns + "mi", mi));

				XElement XPackageRequirements = config.Descendants(mi + "PackageRequirements").FirstOrDefault();
				XPackageInstaller.Add(XPackageRequirements);


				XElement XPackageInformation = config.Descendants(mi + "PackageInformation").FirstOrDefault();



				var packageId = XPackageInformation.Attribute("id").Value;
				var installedPackageFilename = Path.Combine(Path.Combine(Path.Combine(PathUtil.Resolve(PathUtil.BaseDirectory), "App_Data\\Composite\\Packages"), packageId), "package.zip");

				XPackageInstaller.Add(XPackageInformation);

				XElement XPackageFragmentInstallerBinaries = new XElement(mi + "PackageFragmentInstallerBinaries");
				try
				{
					XPackageFragmentInstallerBinaries = config.Descendants(mi + "PackageFragmentInstallerBinaries").First();
				}
				catch
				{
				}

				XPackageFragmentInstallerBinaries.ReplaceNodes(XPackageFragmentInstallerBinaries.Elements().OrderBy(ReferencedAssemblies.AssemblyPosition));
				XPackageInstaller.Add(XPackageFragmentInstallerBinaries);


				XElement XPackageFragmentInstallers = new XElement(mi + "PackageFragmentInstallers");

				#endregion

				#region XsltFunctions
				XElement XsltFunctions = config.Descendants(PackageCreator.pc + "XsltFunctions").FirstOrDefault();
				if (XsltFunctions != null)
				{
					foreach (XElement item in XsltFunctions.Elements("Add").OrderBy(d => d.IndexAttributeValue()))
					{
						IXsltFunction xsltFunction;
						try
						{
							xsltFunction = (from i in DataFacade.GetData<IXsltFunction>()
											where i.Namespace + "." + i.Name == item.IndexAttributeValue()
											select i).First();
						}
						catch (Exception)
						{
							throw new ArgumentException(string.Format(@"XSLT Function '{0}' doesn't exists", item.IndexAttributeValue()));
						}

						var newXslFilePath = "\\" + xsltFunction.Namespace.Replace(".", "\\") + "\\" + xsltFunction.Name + ".xsl";


						AddFile("App_Data\\Xslt" + xsltFunction.XslFilePath, "App_Data\\Xslt" + newXslFilePath);
						xsltFunction.XslFilePath = newXslFilePath;
						//AddData(xsltFunction, "Composite.Data.Types.IXsltFunction" + ", Composite");
						AddData(xsltFunction);

						var parameters = from i in DataFacade.GetData<IParameter>()
										 where i.OwnerId == xsltFunction.Id
										 orderby i.Position
										 select i;
						//foreach (var parameter in parameters) AddData(parameter, "Composite.Data.Types.IParameter" + ", Composite");
						foreach (var parameter in parameters) AddData(parameter);

						var namedFunctionCalls = from i in DataFacade.GetData<INamedFunctionCall>()
												 where i.XsltFunctionId == xsltFunction.Id
												 orderby i.Name
												 select i;

						foreach (var namedFunctionCall in namedFunctionCalls) AddData(namedFunctionCall);

					}
				}


				#endregion

				#region CSharpFunctions
				XElement CSharpFunctions = config.Descendants(pc + "CSharpFunctions").FirstOrDefault();
				if (CSharpFunctions != null)
				{
					foreach (XElement item in CSharpFunctions.Elements("Add"))
					{
						IMethodBasedFunctionInfo csharpFunction;
						try
						{
							csharpFunction = (from i in DataFacade.GetData<IMethodBasedFunctionInfo>()
											  where i.Namespace + "." + i.UserMethodName == item.IndexAttributeValue()
											  select i).First();
						}
						catch (Exception)
						{
							throw new ArgumentException(string.Format(@"C# Function '{0}' doesn't exists", item.IndexAttributeValue()));
						}

						AddData(csharpFunction);

					}
				}
				#endregion

				#region VisualFunctions
				XElement VisualFunctions = config.Descendants(pc + "VisualFunctions").FirstOrDefault();
				if (VisualFunctions != null)
				{
					foreach (XElement item in VisualFunctions.Elements("Add"))
					{
						IVisualFunction visualFunction;
						try
						{
							visualFunction = (from i in DataFacade.GetData<IVisualFunction>()
											  where i.Namespace + "." + i.Name == item.IndexAttributeValue()
											  select i).First();
						}
						catch (Exception)
						{
							throw new ArgumentException(string.Format(@"Visual Function '{0}' doesn't exists", item.IndexAttributeValue()));
						}

						//AddData(visualFunction, "Composite.Data.Types.IVisualFunction" + ", Composite");
						AddData(visualFunction);
					}
				}
				#endregion


				#region PageTemplates
				XElement PageTemplates = config.Descendants(pc + "PageTemplates").FirstOrDefault();
				if (PageTemplates != null)
				{
					foreach (XElement item in PageTemplates.Elements("Add"))
					{
						var pageTemplate = (from i in DataFacade.GetData<IXmlPageTemplate>()
											where i.Title == item.IndexAttributeValue()
											select i).FirstOrDefault();

						if (pageTemplate != null)
						{
							var newPageTemplateFilePath = "\\" + pageTemplate.Title + ".xml";

							AddFile("App_Data\\PageTemplates" + pageTemplate.PageTemplateFilePath, "App_Data\\PageTemplates" + newPageTemplateFilePath);
							pageTemplate.PageTemplateFilePath = newPageTemplateFilePath;
							AddData(pageTemplate);
						}

					}
				}

				#endregion

				#region Files
				XElement InstallFiles = config.Descendants(pc + "Files").FirstOrDefault();
				if (InstallFiles != null)
				{
					foreach (XElement item in InstallFiles.Elements("Add"))
					{
						string filename = item.IndexAttributeValue();

						if (string.IsNullOrEmpty(filename))
						{
							throw new InvalidOperationException("Files->Add attribute 'name' must be spesified");
						}

						AddFile(filename);
					}
				}
				#endregion

				#region FileXslTransformation
				/*XElement FileXslTransformation = config.Descendants(pc + "FileXslTransformation").FirstOrDefault();
				if (FileXslTransformation != null)
				{
					foreach (XElement item in FileXslTransformation.Elements("Add"))
					{
						string pathXml = item.IndexAttributeValue();
						string pathXsl = item.AttributeValue("pathXsl");

						if (string.IsNullOrEmpty(filename))
						{
							throw new InvalidOperationException("Files->Add attribute 'name' must be spesified");
						}
						XslFiles.Add(
							new XElement("XsltFile",
								new XAttribute("pathXml", "~\\" + pathXml),
								new XAttribute("pathXsl", "~\\" + pathXsl)
							)
						);
					}
				}*/
				#endregion

				#region FilesInDirectory
				XElement InstallFilesInDirectory = config.Descendants(pc + "FilesInDirectories").FirstOrDefault();
				if (InstallFilesInDirectory != null)
				{
					foreach (XElement item in InstallFilesInDirectory.Elements("Add"))
					{

						var path = item.IndexAttributeValue();
						AddFilesinDirectory(path);


					}
				}
				#endregion

				#region Directories
				XElement InstallDirectories = config.Descendants(pc + "Directories").FirstOrDefault();
				if (InstallDirectories != null)
				{
					foreach (XElement item in InstallDirectories.Elements("Add"))
					{

						try
						{
							var filename = item.IndexAttributeValue();
							var targetFilename = Path.Combine(packageDirectoryPath, filename);
							if (Directory.Exists(Path.GetDirectoryName(targetFilename)) == false)
							{
								Directory.CreateDirectory(Path.GetDirectoryName(targetFilename));
							}
							FileSystem.copyDirectory(Path.Combine(PathUtil.Resolve(PathUtil.BaseDirectory), filename), targetFilename);

							Directories.Add(
								new XElement("Directory",
									new XAttribute("sourceDirectory", "~\\" + filename),
									new XAttribute("targetDirectory", "~\\" + filename),
									new XAttribute("allowOverwrite", "true"),
									new XAttribute("deleteTargetDirectory", "false")
								)
							);
						}
						catch { }
					}
				}
				#endregion

				#region Datas
				blockElement = rootConfigElement.Element(pc + "Datas");
				if (blockElement != null)
				{
					foreach (XElement item in blockElement.Elements("Type"))
					{
						var dataTypeName = item.Attribute("type").Value;
						var dataScopeIdentifier = item.Element("Data").Attribute("dataScopeIdentifier").Value;
						AddData(dataTypeName, dataScopeIdentifier);
					}
				}
				#endregion

				#region DataItems
				blockElement = rootConfigElement.Element(pc + "DataItems");
				if (blockElement != null)
				{
					foreach (XElement item in blockElement.Elements("Type"))
					{
						var dataTypeName = item.Attribute("type").Value;
						var dataScopeIdentifier = item.Element("Data").Attribute("dataScopeIdentifier").Value;

						Func<XElement, Func<IData, bool>> trueAttributes = e => e.Attributes().Where(x => x.Name.Namespace == XNamespace.None)
							.Aggregate(new Func<IData, bool>(d => true), (f, x) => new Func<IData, bool>(d => (d.GetProperty(x.Name.LocalName) == x.Value) && f(d)));
						Func<IData, bool> where = item.Element("Data").Elements("Add")
							.Aggregate(new Func<IData, bool>(d => false), (f, e) => new Func<IData, bool>(d => trueAttributes(e)(d) || f(d)));


						AddData(dataTypeName, dataScopeIdentifier, where);
					}
				}
				#endregion

				#region Configuration

				XElement ConfigurationTransformationPackageFragmentInstaller = null;

				blockElement = rootConfigElement.Element(pc + "Configuration");
				if (blockElement != null)
				{
					if (blockElement.Elements("Add").Count() > 0)
					{


						XDocument installXsl = XDocument.Parse(blankXsl);
						XDocument uninstallXsl = XDocument.Parse(blankXsl);

						var nodes = new Dictionary<string, Dictionary<string, XElement>>();


						foreach (XElement item in blockElement.Elements("Add"))
						{
							string xpath = item.IndexAttributeValue();
							AddConfigurationXPath(xpath);
						}
					}
				}



				#endregion


#warning TODO: Datatypes data format depends from datatype exists in package or not, thats why list of dtatype must created before adding data

				#region DynamicDataTypes
				XElement DynamicDataTypes = config.Descendants(pc + "DynamicDataTypes").FirstOrDefault();
				XElement DynamicDataTypePackageFragmentInstaller = null;
				if (DynamicDataTypes != null)
				{
					List<XElement> dataTypes = new List<XElement>();
					foreach (XElement item in DynamicDataTypes.Elements("Add").OrderBy(e => DataPosition(e.IndexAttributeValue())))
					{
						var dynamicDataType = TypeManager.GetType(item.IndexAttributeValue());
						var dynamicDataTypeName = TypeManager.SerializeType(dynamicDataType);

						AddData<IGeneratedTypeWhiteList>(d => d.TypeManagerTypeName == dynamicDataTypeName);

						installDataTypeNamesList.Add(dynamicDataTypeName);
						var type = TypeManager.GetType(item.IndexAttributeValue());
						DataTypeDescriptor dataTypeDescriptor = DynamicTypeManager.GetDataTypeDescriptor(type);
						dataTypes.Add(new XElement("Type",
									   new XAttribute("providerName", "GeneratedDataTypesElementProvider"),
									   new XAttribute("dataTypeDescriptor", dataTypeDescriptor.ToXml().ToString())
									   )
								   );

						AddFileIfExists("App_Data\\Composite\\DynamicTypeForms\\" + item.IndexAttributeValue().Replace('.', '\\') + ".xml");
					}


					DynamicDataTypePackageFragmentInstaller = new XElement(mi + "Add",
								new XAttribute("installerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.DynamicDataTypePackageFragmentInstaller, Composite"),
								new XAttribute("uninstallerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.DynamicDataTypePackageFragmentUninstaller, Composite"),
								new XElement("Types",
									dataTypes
									)
								);


				}

				#endregion


				//Add only one MetaTypeTab
				using (new GuidReplacer(from cd in DataFacade.GetData<ICompositionContainer>()
										select new KeyValuePair<Guid, Guid>(cd.GetProperty<Guid>("Id"), GuidReplacer.CompositionContainerGuid)
										))
				{

					foreach (var categoryType in PackageCreatorActionFacade.CategoryTypes)
					{
						if (categoryType.Value.IsIPackagable())
						{
							foreach (var packageItem in PackageCreatorFacade.GetItems(categoryType.Value, config.Root))
							{
								(packageItem as IPackagable).Pack(this);
							}
						}
					}

					#region DynamicDataTypesData

					XElement element = config.Descendants(pc + "DynamicDataTypesData").FirstOrDefault();
					//XElement DynamicDataTypePackageFragmentInstaller = null;
					if (element != null)
					{

						foreach (XElement item in element.Elements("Add"))
						{
							AddDinamicDataTypeData(TypeManager.TryGetType(item.IndexAttributeValue()));
						}
					}
				}
					#endregion

				#region ConfigurationTransformationPackageFragmentInstaller
				if (ConfigurationXPaths.Count > 0 || ConfigurationInstallXsltTemplates.Count > 0)
				{
					{
						XDocument installXsl = XDocument.Parse(blankXsl);
						XDocument uninstallXsl = XDocument.Parse(blankXsl);

						var nodes = new Dictionary<string, Dictionary<string, XElement>>();
						foreach (string xpath in ConfigurationXPaths)
						{
							var configuration = PackageCreatorFacade.GetConfigurationDocument();
							var element = configuration.XPathSelectElement(xpath);
							Regex re = new Regex(@"^(.*?)/([^/]*(\[[^\]]*\])?)$");
							Match match = re.Match(xpath);
							if (match.Success)
							{
								var itemPath = match.Groups[1].Value;
								var itemKey = match.Groups[2].Value;

								nodes.Add(itemPath, itemKey, element);

								uninstallXsl.Root.Add(
									new XElement(xsl + "template",
										new XAttribute("match", xpath)
										)
									);
							}
						}
						foreach (var node in nodes)
						{
							installXsl.Root.Add(
								new XElement(xsl + "template",
									new XAttribute("match", node.Key),
									new XElement(xsl + "copy",
										new XElement(xsl + "apply-templates",
											new XAttribute("select", "@* | node()")),
										from e in node.Value
										select
											new XElement(xsl + "if",
												new XAttribute("test", string.Format("count({0})=0", e.Key)),
												e.Value
											)

										)

									)
								);
						}
						installXsl.Root.Add(ConfigurationInstallXsltTemplates);

						var configDirectory = Path.Combine(packageDirectoryPath, configDirectoryName);
						if (!Directory.Exists(configDirectory))
							Directory.CreateDirectory(configDirectory);
						installXsl.SaveTabbed(Path.Combine(configDirectory, configInstallFileName));
						uninstallXsl.SaveTabbed(Path.Combine(configDirectory, configUninstallFileName));



						ConfigurationTransformationPackageFragmentInstaller =
							new XElement(mi + "Add",
								new XAttribute("installerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.ConfigurationTransformationPackageFragmentInstaller, Composite"),
								new XAttribute("uninstallerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.ConfigurationTransformationPackageFragmentUninstaller, Composite"),
								new XElement("Install",
									new XAttribute("xsltFilePath", string.Format(@"~\{0}\{1}", configDirectoryName, configInstallFileName))
								),
								new XElement("Uninstall",
									new XAttribute("xsltFilePath", string.Format(@"~\{0}\{1}", configDirectoryName, configUninstallFileName))
								)
							);


					}
				}
				#endregion

				#region FilePackageFragmentInstaller
				XElement FilePackageFragmentInstaller = new XElement(mi + "Add",
				new XAttribute("installerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.FilePackageFragmentInstaller, Composite"),
				new XAttribute("uninstallerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.FilePackageFragmentUninstaller, Composite"),
				new XElement("Files"
					, Files.OrderBy(d => ReferencedAssemblies.AssemblyPosition(d))
					),
				new XElement("Directories"
					, Directories
					)
				);

				#endregion

				#region FileXslTransformationPackageFragmentInstaller
				XElement FileXslTransformationPackageFragmentInstaller = new XElement(mi + "Add",
				new XAttribute("installerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.FileXslTransformationPackageFragmentInstaller, Composite"),
				new XAttribute("uninstallerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.FileXslTransformationPackageFragmentUninstaller, Composite"),
				new XElement("XslFiles"
					, XslFiles
					)
				);
				#endregion

				#region DataPackageFragmentInstaller

				var DataPackageFragmentInstaller = new XElement(mi + "Add",
						new XAttribute("installerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.DataPackageFragmentInstaller, Composite"),
						new XAttribute("uninstallerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.DataPackageFragmentUninstaller, Composite"),
						new XElement("Types",
							from t in Datas
							orderby t.Key
							orderby DataPosition(t.Value.AttributeValue("type"))
							select t.Value
							)
						);
				#endregion

				XElement XPackageFragments = config.Descendants(pc + "PackageFragmentInstallers").FirstOrDefault();
				if (XPackageFragments != null)
				{
					XPackageFragmentInstallers.Add(XPackageFragments.Elements());
				}


				XPackageFragmentInstallers.Add(ConfigurationTransformationPackageFragmentInstaller);
				XPackageFragmentInstallers.Add(FilePackageFragmentInstaller);
				XPackageFragmentInstallers.Add(DynamicDataTypePackageFragmentInstaller);
				XPackageFragmentInstallers.Add(DataPackageFragmentInstaller);
				DataFiles.Save();

				XPackageFragmentInstallers.ReplaceNodes(XPackageFragmentInstallers.Elements().OrderBy(FragmentPosition).Select(DeleteOrderingMark));
				XPackageInstaller.Add(XPackageFragmentInstallers);

				XPackage.Add(XPackageInstaller);

				XPackage.SaveTabbed(Path.Combine(packageDirectoryPath, InstallFilename));

				var zipFilename = packageName + ".zip";


				#region Zipping
				var filenames = Directory.GetFiles(packageDirectoryPath, "*", SearchOption.AllDirectories);
				var directories = Directory.GetDirectories(packageDirectoryPath, "*", SearchOption.AllDirectories);
				using (var s = new ZipOutputStream(File.Create(Path.Combine(zipDirectoryPath, zipFilename))))
				{
					s.SetLevel(9); // 0 - store only to 9 - means best compression
					var buffer = new byte[4096];

					foreach (string directory in directories)
					{
						var entry = new ZipEntry(directory.Replace(packageDirectoryPath + "\\", "").Replace("\\", "/") + "/")
										{
											IsUnicodeText = true,
											DateTime = DateTime.Now
										};
						s.PutNextEntry(entry);
						s.CloseEntry();
					}

					foreach (string file in filenames)
					{
						var entry = new ZipEntry(file.Replace(packageDirectoryPath + "\\", "").Replace("\\", "/"))
										{
											IsUnicodeText = true,
											DateTime = DateTime.Now
										};
						s.PutNextEntry(entry);

						using (var fs = File.OpenRead(file))
						{
							int sourceBytes;
							do
							{
								sourceBytes = fs.Read(buffer, 0, buffer.Length);
								s.Write(buffer, 0, sourceBytes);
							} while (sourceBytes > 0);
						}
					}
					s.Finish();
					s.Close();
				}
				#endregion

				try
				{
					Directory.CreateDirectory(Path.Combine(packageDirectoryPath, "../_" + CREATOR_DRECTORY + ""));
					File.Copy(packageConfigFilePath, Path.Combine(packageDirectoryPath, "../_" + CREATOR_DRECTORY + "/" + packageName + ".xml"));
					SetFileReadAccess(Path.Combine(packageDirectoryPath, "../_" + CREATOR_DRECTORY + "/" + packageName + ".xml"), false);

				}
				catch { }

				try
				{
					if (Directory.Exists(dataPackageDirectoryPath))
					{
						FileSystem.copyDirectory(dataPackageDirectoryPath, Path.Combine(packageDirectoryPath, "../_" + CREATOR_DRECTORY + "/" + packageName + "_Data"));
					}
				}
				catch { }

				return Path.Combine(packageName, "Release", zipFilename);
			}

		}

		internal void AddFilesinDirectory(string path)
		{
			foreach (var filename in Directory.GetFiles(Path.Combine(PathUtil.Resolve(PathUtil.BaseDirectory), path), "*", SearchOption.AllDirectories))
			{
				AddFile(filename.Replace(PathUtil.Resolve(PathUtil.BaseDirectory), ""));
			}
		}

		#region SortingByForeignKey

		/*private int DataPosition(string dataname)
		{
			try
			{
				var position = (from d in dataPosition
								where dataname.Replace(" ", "").Contains(d.Key.Replace(" ", ""))
								select d.Value).First();

				return position;
			}
			catch
			{
			}

			return int.MaxValue;
		}*/

		private int DataPosition(string dataname)
		{
			try
			{
				var type = TypeManager.GetType(dataname);
				return DataPosition(type);
			}
			catch
			{
			}
			return int.MaxValue;
		}

		private int DataPosition(Type type)
		{
			try
			{
				return dataPosition[type];
			}
			catch
			{
			}
			return int.MaxValue;
		}

		public void BuildDataPosition()
		{

			Dictionary<Type, IEnumerable<Type>> foreignTypes = new Dictionary<Type, IEnumerable<Type>>();

			foreach (var type in DataFacade.GetAllInterfaces())
			{
				try
				{
					foreignTypes.Add(type, GetForeignTypes(type));
				}
				catch
				{ }
			}

			for (int i = 1; i <= 100; i++)
			{
				List<Type> toremove = new List<Type>();
				foreach (var type in foreignTypes)
				{
					if (type.Value.Count() == 0)
					{
						dataPosition.Add(type.Key, i);
						toremove.Add(type.Key);
					}

				}
				foreach (var key in toremove)
				{
					foreignTypes.Remove(key);
				}
				var prevDataPosition = dataPosition.Keys.ToList();
				foreignTypes = foreignTypes.ToDictionary(d => d.Key, d => d.Value.Except(prevDataPosition));

			}

			foreach (var type in foreignTypes)
			{
				dataPosition.Add(type.Key, int.MaxValue);
			}
			//IDataItemTreeAttachmentPoint doesnot have foreign key but referenced to other types
			dataPosition[typeof(IDataItemTreeAttachmentPoint)] = int.MaxValue;

		}

		public static IEnumerable<Type> GetForeignTypes(Type type)
		{
			return DataAttributeFacade.GetDataReferencePropertyInfoes(type).Select(d => d.TargetType);
		}


		#endregion

		#region SortingByOrdering
		public static int FragmentPosition(XElement element)
		{
			try
			{
				return int.Parse(element.AttributeValue(orderingAttributeName));
			}
			catch
			{

			}
			return 0;
		}

		public static XElement DeleteOrderingMark(XElement element)
		{
			element.SetAttributeValue(orderingAttributeName, null);
			return element;
		}

		#endregion


		public static void SetFileReadAccess(string fileName, bool setReadOnly)
		{
			new FileInfo(fileName) {IsReadOnly = setReadOnly};
		}

		#region XsltConfigiration
		public void AddConfigurationXPath(string xpath)
		{
			ConfigurationXPaths.Add(xpath);
		}

		public void AddConfigurationInstallTemplate(XElement template)
		{
			ConfigurationInstallXsltTemplates.Add(template);
		}
		#endregion

		#region AddData

		internal void AddData(Type type)
		{
			AddData(type, d => true);
		}

		internal void AddData(string dataTypeName)
		{
			AddData(TypeManager.TryGetType(dataTypeName), d => true);
		}

		internal void AddData<T>() where T : class, IData
		{
			AddData(typeof(T));
		}

		public void AddData<T>(Func<T, bool> where) where T : class, IData
		{
			Func<IData, bool> f = d => where((T)d);
			AddData(typeof(T), f);
		}

		internal void AddData(Type type, Func<IData, bool> where)
		{
			foreach (var dataScopeIdentifier in DataFacade.GetSupportedDataScopes(type))
			{
				AddData(type, dataScopeIdentifier, where);
			}
		}

		internal void AddData(string dataTypeName, Func<IData, bool> where)
		{
			AddData(TypeManager.TryGetType(dataTypeName), where);
		}

		internal void AddData(string dataTypeName, string dataScopeIdentifier)
		{
			AddData(dataTypeName, dataScopeIdentifier, f => true);
		}


		internal void AddData(string dataTypeName, string dataScopeIdentifier, Func<IData, bool> where)
		{
			AddData(TypeManager.TryGetType(dataTypeName), DataScopeIdentifier.Deserialize(dataScopeIdentifier), where);
		}

		internal void AddData(Type type, DataScopeIdentifier dataScopeIdentifier, Func<IData, bool> where)
		{
			using (new DataScope(dataScopeIdentifier))
			{
				if (DataLocalizationFacade.IsLocalized(type))
				{
					switch (LocaleAction)
					{
						case LocaleActions.AllLocales:
							foreach (var locale in DataLocalizationFacade.ActiveLocalizationCultures)
							{
								using (new DataScope(locale))
								{
									foreach (var data in DataFacade.GetData(type).ToDataEnumerable().Where(where).OrderBy(d => d.GetSelfPosition()))
									{
										AddData(data);
									}
								}
							}
							break;
						case LocaleActions.DefaultLocalesToCurrentLocale:
						case LocaleActions.DefaultLocalesToAllLocales:
							using (new DataScope(DataLocalizationFacade.DefaultLocalizationCulture))
							{
								foreach (var data in DataFacade.GetData(type).ToDataEnumerable().Where(where).OrderBy(d => d.GetSelfPosition()))
								{
									AddData(data);
								}
							}
							break;
					}
				}
				else
				{
					foreach (var data in DataFacade.GetData(type).ToDataEnumerable().Where(where).OrderBy(d => d.GetSelfPosition()))
					{
						AddData(data);
					}
				}
			}
		}


		internal void AddDinamicDataTypeData(Type type)
		{
			IEnumerable<Type> globalDataTypeInterfaces = DataFacade.GetGeneratedInterfaces().OrderBy(t => t.FullName);
			globalDataTypeInterfaces = globalDataTypeInterfaces.Except(PageFolderFacade.GetAllFolderTypes());
			globalDataTypeInterfaces = globalDataTypeInterfaces.Except(PageMetaDataFacade.GetAllMetaDataTypes());

			IEnumerable<Type> pageDataTypeInterfaces = PageFolderFacade.GetAllFolderTypes();
			IEnumerable<Type> pageMetaTypeInterfaces = PageMetaDataFacade.GetAllMetaDataTypes();

			DataTypeDescriptor dataTypeDescriptor = DynamicTypeManager.GetDataTypeDescriptor(type);
			//todo make by assosiation
			if (globalDataTypeInterfaces.Contains(dataTypeDescriptor.GetInterfaceType()))
			{
				AddData(type);
			}
			else if (pageDataTypeInterfaces.Contains(dataTypeDescriptor.GetInterfaceType()))
			{
				AddData<IPageFolderDefinition>(d => d.FolderTypeId == dataTypeDescriptor.DataTypeId);
				AddData(type);
			}
			else if (pageMetaTypeInterfaces.Contains(dataTypeDescriptor.GetInterfaceType()))
			{
				foreach (var j in DataFacade.GetData<IPageMetaDataDefinition>(d => d.MetaDataTypeId == dataTypeDescriptor.DataTypeId))
				{
					//Add only one MetaTypeTab
					AddData<ICompositionContainer>(d => d.Id == j.MetaDataContainerId);
				}
				AddData<IPageMetaDataDefinition>(d => d.MetaDataTypeId == dataTypeDescriptor.DataTypeId);
				AddData(type);
			}
		}

		public void AddData(IData data)
		{
#warning #3102 Do not export ICompositionContainer with id eb210a75-be25-401f-b0d4-b3787bce36fa
			if (data is ICompositionContainer)
			{
				if ((data as ICompositionContainer).Id == new Guid("eb210a75-be25-401f-b0d4-b3787bce36fa"))
					return;
			}

			string dataTypeName = TypeManager.SerializeType(data.DataSourceId.InterfaceType);
			string dataScopeIdentifier = DataScopeManager.CurrentDataScope.Name;

			string cultureName = string.Empty;
			ILocalizedControlled localizedData = data as ILocalizedControlled;
			if (localizedData != null)
			{
				if (LocaleActions.AllLocales == LocaleAction)
				{
					cultureName = localizedData.CultureName;
				}
				else if (LocaleActions.DefaultLocalesToAllLocales == LocaleAction)
				{
					cultureName = "*";
				}
				else if (LocaleActions.DefaultLocalesToCurrentLocale == LocaleAction)
				{
					cultureName = "?";
				}

			}

			var dataTypeKey = dataTypeName + dataScopeIdentifier + (string.IsNullOrEmpty(cultureName) ? string.Empty : ("_" + cultureName));
			var dataTypeFileName = Regex.Replace(dataTypeName, "(.+:|,.+)", "") + "s_" + dataScopeIdentifier + ((string.IsNullOrEmpty(cultureName) || cultureName.Length <= 1) ? string.Empty : ("_" + cultureName)) + ".xml";


			//Add only one MetaTypeTab
			if (Datas.ContainsKey(dataTypeKey))
			{
				if (dataTypeName.Contains("Composite.Data.Types.ICompositionContainer"))
					return;
			}

			var shortFilePath = "Datas\\" + dataTypeFileName;
			var filePath = Path.Combine(packageDirectoryPath, shortFilePath);

			if (!Datas.ContainsKey(dataTypeKey))
			{
				Datas.Add(
					dataTypeKey
					, new XElement("Type",
						(installDataTypeNamesList.Contains(dataTypeName)) ? new XAttribute("isDynamicAdded", "true") : null,
						new XAttribute("type", dataTypeName),
						new XElement("Data",
							new XAttribute("dataScopeIdentifier", dataScopeIdentifier),
							string.IsNullOrEmpty(cultureName) ? null : new XAttribute("locale", cultureName),
							new XAttribute("dataFilename", "~\\" + shortFilePath)
							)
						)
					);
				//x = new XElement("Data");
				//string targetDirectory = Path.GetDirectoryName(filePath);
				//if (Directory.Exists(targetDirectory) == false)
				//{
				//    Directory.CreateDirectory(targetDirectory);
				//}
				//x.Save(filePath);
			}
			
			DataFiles.Add(filePath,
				new XElement("Add",
					from s in TypeManager.GetType(dataTypeName).GetPropertiesRecursively()
					where s.CanWrite && s.CanRead
					let val = s.GetValue(data, null)
					where val != null
					select new XAttribute(s.Name, SerializeDataField(val))
				)
			);
			//x = XElement.Load(filePath);
			//x.Add(
			//    new XElement("Add",
			//        from s in TypeManager.GetType(dataTypeName).GetPropertiesRecursively()

			//        where s.CanWrite && s.CanRead
			//        let val = s.GetValue(data, null)
			//        where val != null
			//        select new XAttribute(s.Name, SerializeDataField(val))
			//    )
			//);
			//x.Save(filePath);
		}

		private static string SerializeDataField(object o)
		{
			if (o is Guid)
			{
				return GuidReplacer.Replace((Guid)o).ToString();
			}
			if (o is DateTime)
				return ((DateTime)o).ToString("u");
			return o.ToString();
		}
		#endregion
		#region AddFile


		internal void AddFileIfExists(string filename)
		{
			if (File.Exists(Path.Combine(PathUtil.Resolve(PathUtil.BaseDirectory), filename)))
				AddFile(filename);
		}

		internal void AddXslFile(string pathXml, string pathXsl)
		{
			XslFiles.Add(
				new XElement("XsltFile",
					new XAttribute("pathXml", "~\\" + pathXml),
					new XAttribute("pathXsl", "~\\" + pathXsl)
				)
			);
		}

		public void AddFile(string filename)
		{
			AddFile(filename, filename);
		}

		internal void AddFile(string filename, string newFilename)
		{
			if (filename.StartsWith("~"))
				filename = filename.Substring(2);
			if (newFilename.StartsWith("~"))
				newFilename = newFilename.Substring(2);
			string targetFilename = Path.Combine(packageDirectoryPath, newFilename);
			string targetDirectory = Path.GetDirectoryName(targetFilename);
			if (Directory.Exists(targetDirectory) == false)
			{
				Directory.CreateDirectory(targetDirectory);
			}
			File.Copy(Path.Combine(PathUtil.Resolve(PathUtil.BaseDirectory), filename), targetFilename, true);
			Files.Add(
				new XElement("File",
					new XAttribute("sourceFilename", "~\\" + newFilename),
					new XAttribute("targetFilename", "~\\" + newFilename),
					new XAttribute("allowOverwrite", "false")
				)
			);
		}


		#endregion




		#endregion
	}
}
