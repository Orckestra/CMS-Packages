using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Composite.Core.IO;
using Composite.Core.Types;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Data.ProcessControlled;
using Composite.Data.Types;
using Composite.Tools.PackageCreator.Actions;
using Composite.Tools.PackageCreator.Types;
using Utility.IO;

namespace Composite.Tools.PackageCreator
{
    public sealed class PackageCreator
    {
        private static readonly CompressionLevel PackageCompressionLevel = CompressionLevel.Optimal;

        public static readonly XNamespace mi = "http://www.composite.net/ns/management/packageinstaller/1.0";
        public static readonly XNamespace pc = "http://www.composite.net/ns/management/packagecreator/2.0";
        public static readonly XNamespace help = "http://www.composite.net/ns/help/1.0";
        private static readonly XNamespace xsl = "http://www.w3.org/1999/XSL/Transform";


        public static string InstallFilename { get { return "install.xml"; } }

        //public static readonly string configDirectoryName = "Config";
        public static readonly string configInstallFileName = "Install.xsl";
        public static readonly string configUninstallFileName = "Uninstall.xsl";

        public static readonly XName orderingAttributeName = "Ordering";


        private string _serviceDirectoryPath;
        private string _packageDirectoryPath;
        private string _zipDirectoryPath;

		public HashSet<Guid> ExcludedIds = new HashSet<Guid>();
		public HashSet<string> ExcludedPaths = new HashSet<string>();
        private List<XElement> Files = new List<XElement>();
        private List<XElement> Directories = new List<XElement>();
        private List<XElement> XslFiles = new List<XElement>();
        private Dictionary<string, HashSet<string>> ConfigurationXPaths = new Dictionary<string, HashSet<string>>();
        private Dictionary<string, List<XElement>> ConfigurationTemplates = new Dictionary<string, List<XElement>>();

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


        private string _packageName;
        public string PackageName
        {
            get
            {
                return _packageName;
            }
        }

        internal PackageCreator(string path, string packageName)
        {
            this._serviceDirectoryPath = path;
            this._packageName = packageName;
            if (!Directory.Exists(_serviceDirectoryPath))
            {
                Directory.CreateDirectory(_serviceDirectoryPath);
            }

            BuildDataPosition();
        }

        #region Creation
        public string CreatePackagePack()
        {
            using (new DataScope(DataScopeIdentifier.Public))
            {
                #region Init
                if (_packageName == string.Empty)
                {
                    return "";
                }

                _packageDirectoryPath = Path.Combine(_serviceDirectoryPath, _packageName);
                if (Directory.Exists(_packageDirectoryPath))
                {
                    Directory.Delete(_packageDirectoryPath, true);
                }

                var dataPackageDirectoryPath = Path.Combine(_serviceDirectoryPath, _packageName + "_Data");

                _zipDirectoryPath = Path.Combine(_packageDirectoryPath, "Release");
                _packageDirectoryPath = Path.Combine(_packageDirectoryPath, "Package");

                Directory.CreateDirectory(_packageDirectoryPath);
                Directory.CreateDirectory(_zipDirectoryPath);

                if (Directory.Exists(dataPackageDirectoryPath))
                {
                    FileSystem.CopyDirectory(dataPackageDirectoryPath, _packageDirectoryPath);
                }

                string packageConfigFilePath = Path.Combine(_serviceDirectoryPath, _packageName + ".xml");

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

				#region Init Categories
				foreach (var categoryType in PackageCreatorActionFacade.CategoryTypes)
				{
					if (categoryType.Value.IsInitable())
					{
						foreach (var packageItem in PackageCreatorFacade.GetItems(categoryType.Value, config.Root))
						{
							(packageItem as IPackInit).Init(this);
						}
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

                #region FilesInDirectory
                XElement InstallFilesInDirectory = config.Descendants(pc + "FilesInDirectories").FirstOrDefault();
                if (InstallFilesInDirectory != null)
                {
                    foreach (XElement item in InstallFilesInDirectory.Elements("Add"))
                    {

                        var path = item.IndexAttributeValue();
                        AddFilesInDirectory(path);


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

                            AddDirectory(filename);
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

                        Func<XElement, Func<IData, bool>> trueAttributes = e => e.Attributes().Where(x => x.Name.Namespace == XNamespace.None)
                            .Aggregate(new Func<IData, bool>(d => true), (f, x) => new Func<IData, bool>(d => (d.GetProperty(x.Name.LocalName) == x.Value) && f(d)));
                        Func<IData, bool> where = item.Elements("Add")
                            .Aggregate(new Func<IData, bool>(d => false), (f, e) => new Func<IData, bool>(d => trueAttributes(e)(d) || f(d)));

                        AddData(dataTypeName, where);
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
                        if (categoryType.Value.IsPackagable())
                        {
                            foreach (var packageItem in PackageCreatorFacade.GetItems(categoryType.Value, config.Root))
                            {
                                (packageItem as IPack).Pack(this);
                            }
                        }
                    }

                    #region DynamicDataTypesData

                    XElement element = config.Descendants(pc + "DynamicDataTypesData").FirstOrDefault();

                    if (element != null)
                    {

                        foreach (XElement item in element.Elements("Add"))
                        {
                            AddDataTypeData(TypeManager.TryGetType(item.IndexAttributeValue()));
                        }
                    }
                }
                    #endregion

                #region TransformationPackageFragmentInstallers

                XElement ConfigurationTransformationPackageFragmentInstallers = null;

                var sources = new HashSet<string>();
                sources.UnionWith(ConfigurationXPaths.Keys);
                sources.UnionWith(ConfigurationTemplates.Keys);

                foreach (var source in sources)
                {

                    XDocument installXsl = XDocument.Parse(blankXsl);
                    XDocument uninstallXsl = XDocument.Parse(blankXsl);


                    HashSet<string> xpaths;
                    if (ConfigurationXPaths.TryGetValue(source, out xpaths))
                    {
                        var nodes = new Dictionary<string, Dictionary<string, XElement>>();
                        foreach (string xpath in xpaths)
                        {
                            var configuration = PackageCreatorFacade.GetConfigurationDocument(source);
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
                    }

                    List<XElement> templates;
                    if (ConfigurationTemplates.TryGetValue(source, out templates))
                    {
                        installXsl.Root.Add(templates);
                    }

                    var configDirectory = Path.Combine(_packageDirectoryPath, source);
                    if (!Directory.Exists(configDirectory))
                        Directory.CreateDirectory(configDirectory);
                    installXsl.SaveTabbed(Path.Combine(configDirectory, configInstallFileName));
                    uninstallXsl.SaveTabbed(Path.Combine(configDirectory, configUninstallFileName));

                    if (PCCompositeConfig.Source == source)
                    {
                        ConfigurationTransformationPackageFragmentInstallers =
                            new XElement(mi + "Add",
                                new XAttribute("installerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.ConfigurationTransformationPackageFragmentInstaller, Composite"),
                                new XAttribute("uninstallerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.ConfigurationTransformationPackageFragmentUninstaller, Composite"),
                                new XElement("Install",
                                    new XAttribute("xsltFilePath", string.Format(@"~\{0}\{1}", source, configInstallFileName))
                                ),
                                new XElement("Uninstall",
                                    new XAttribute("xsltFilePath", string.Format(@"~\{0}\{1}", source, configUninstallFileName))
                                )
                            );

                    }
                    else
                    {

                        XslFiles.Add(
                            new XElement("XslFile",
                                new XAttribute("pathXml", PackageCreatorFacade.GetConfigurationPath(source)),
                                new XAttribute("installXsl", string.Format(@"~\{0}\{1}", source, configInstallFileName)),
                                new XAttribute("uninstallXsl", string.Format(@"~\{0}\{1}", source, configUninstallFileName))
                            )
                        );
                    }
                }
                #endregion







                XElement XPackageFragments = config.Descendants(pc + "PackageFragmentInstallers").FirstOrDefault();
                if (XPackageFragments != null)
                {
                    XPackageFragmentInstallers.Add(XPackageFragments.Elements());
                }


                XPackageFragmentInstallers.Add(ConfigurationTransformationPackageFragmentInstallers);

                #region FilePackageFragmentInstaller
                if (Files.Count + Directories.Count > 0)
                {
                    XElement FilePackageFragmentInstaller = new XElement(mi + "Add",
                        new XAttribute("installerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.FilePackageFragmentInstaller, Composite"),
                        new XAttribute("uninstallerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.FilePackageFragmentUninstaller, Composite"),
                        Files.Count == 0 ? null : new XElement("Files"
                                                    , Files.OrderBy(d => ReferencedAssemblies.AssemblyPosition(d))
                                                    ),
                        Directories.Count == 0 ? null : new XElement("Directories"
                                                    , Directories
                                                    )
                    );
                    XPackageFragmentInstallers.Add(FilePackageFragmentInstaller);
                }
                #endregion


                #region FileXslTransformationPackageFragmentInstaller
                if (XslFiles.Count > 0)
                {
                    XPackageFragmentInstallers.Add(new XElement(mi + "Add",
                        new XAttribute("installerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.FileXslTransformationPackageFragmentInstaller, Composite"),
                        new XAttribute("uninstallerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.FileXslTransformationPackageFragmentUninstaller, Composite"),
                        new XElement("XslFiles"
                            , XslFiles
                            )
                        )
                    );
                }
                #endregion

                XPackageFragmentInstallers.Add(DynamicDataTypePackageFragmentInstaller);

                #region DataPackageFragmentInstaller
                if (Datas.Count > 0)
                {
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
                    XPackageFragmentInstallers.Add(DataPackageFragmentInstaller);
                }
                #endregion

                DataFiles.Save();

                XPackageFragmentInstallers.ReplaceNodes(XPackageFragmentInstallers.Elements().OrderBy(FragmentPosition).Select(DeleteOrderingMark));
                XPackageInstaller.Add(XPackageFragmentInstallers);

                XPackage.Add(XPackageInstaller);

                XPackage.SaveTabbed(Path.Combine(_packageDirectoryPath, InstallFilename));

                var zipFilename = _packageName + ".zip";


                #region Zipping
                var filenames = Directory.GetFiles(_packageDirectoryPath, "*", SearchOption.AllDirectories);
                var directories = Directory.GetDirectories(_packageDirectoryPath, "*", SearchOption.AllDirectories);


                using(var archiveFileStream = File.Create(Path.Combine(_zipDirectoryPath, zipFilename)))
                using (var archive = new ZipArchive(archiveFileStream, ZipArchiveMode.Create))
                {
                    foreach (string directory in directories)
                    {
                        string directoryEntryName = directory.Replace(_packageDirectoryPath + "\\", "").Replace("\\", "/") + "/";

                        var dirEntry = archive.CreateEntry(directoryEntryName);
                        dirEntry.LastWriteTime = Directory.GetLastWriteTime(directory);
                    }

                    foreach (string file in filenames)
                    {
                        string fileEntryName = file.Replace(_packageDirectoryPath + "\\", "").Replace("\\", "/");

                        var entry = archive.CreateEntry(fileEntryName, PackageCompressionLevel);

                        using (var fileStream = File.OpenRead(file))
                        using (var entryStream = entry.Open())
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    }
                }
                #endregion

                var packageCreatorDirectory = Path.Combine(_packageDirectoryPath, "../_" + CREATOR_DRECTORY);
                try
                {
                    Directory.CreateDirectory(packageCreatorDirectory);

                    var packageCopyFilePath = packageCreatorDirectory + @"\" + _packageName + ".xml";
                    File.Copy(packageConfigFilePath, packageCopyFilePath);
                    SetFileReadAccess(packageCopyFilePath, false);

                }
                catch { }

                try
                {
                    if (Directory.Exists(dataPackageDirectoryPath))
                    {
                        FileSystem.CopyDirectory(dataPackageDirectoryPath, packageCreatorDirectory + @"\" + _packageName + "_Data");
                    }
                }
                catch { }

                return Path.Combine(_packageName, "Release", zipFilename);
            }

        }

        public void AddDirectory(string filename)
        {
            var targetFilename = Path.Combine(_packageDirectoryPath, filename);
            var targetDirectory = Path.GetDirectoryName(targetFilename);
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            FileSystem.CopyDirectory(Path.Combine(PathUtil.Resolve(PathUtil.BaseDirectory), filename), targetFilename);

            Directories.Add(
                new XElement("Directory",
                    new XAttribute("sourceDirectory", "~\\" + filename),
                    new XAttribute("targetDirectory", "~\\" + filename),
                    new XAttribute("allowOverwrite", "true"),
                    new XAttribute("deleteTargetDirectory", "false")
                )
            );
        }

        internal void AddFilesInDirectory(string path)
        {
            var baseDirectory = PathUtil.Resolve(PathUtil.BaseDirectory);
            foreach (var filename in Directory.GetFiles(Path.Combine(baseDirectory, path), "*", SearchOption.AllDirectories))
            {
                AddFile(filename.Replace(baseDirectory, ""));
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
                    if (!type.Value.Any())
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
            new FileInfo(fileName) { IsReadOnly = setReadOnly };
        }

        #region XsltConfigiration
        public void AddConfigurationXPath(string source, string xpath)
        {
            ConfigurationXPaths.Add(source, xpath);
        }

        public void AddConfigurationInstallTemplate(string source, XElement template)
        {
            ConfigurationTemplates.Add(source, template);
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

		internal void AddData<T>(DataScopeIdentifier dataScopeIdentifier, Func<T, bool> where) where T : class, IData
		{
			Func<IData, bool> f = d => where((T)d);
			AddData(typeof(T), dataScopeIdentifier, f);
		}

        internal void AddData(Type type, DataScopeIdentifier dataScopeIdentifier, Func<IData, bool> where)
        {
            using (new DataScope(dataScopeIdentifier))
            {
				foreach (var data in DataFacade.GetData(type).ToDataEnumerable().Where(where).OrderBy(d => d.GetSelfPosition()))
                {
                    AddData(data);
                }

            }
        }


        internal void AddDataTypeData(Type type)
        {
            IEnumerable<Type> globalDataTypeInterfaces = DataFacade
                .GetAllInterfaces(UserType.Developer)
                .Where(interfaceType => interfaceType.Assembly != typeof(IData).Assembly)
                .OrderBy(t => t.FullName)
                .Except(PageFolderFacade.GetAllFolderTypes())
                .Except(PageMetaDataFacade.GetAllMetaDataTypes());

            IEnumerable<Type> pageDataTypeInterfaces = PageFolderFacade.GetAllFolderTypes();
            IEnumerable<Type> pageMetaTypeInterfaces = PageMetaDataFacade.GetAllMetaDataTypes();

            DataTypeDescriptor dataTypeDescriptor = DynamicTypeManager.GetDataTypeDescriptor(type);

            var resolvedType = dataTypeDescriptor.GetInterfaceType();

            // TODO: make by association
            if (globalDataTypeInterfaces.Contains(resolvedType))
            {
                AddData(type);
            }
            else if (pageDataTypeInterfaces.Contains(resolvedType))
            {
                AddData<IPageFolderDefinition>(d => d.FolderTypeId == dataTypeDescriptor.DataTypeId);
                AddData(type);
            }
            else if (pageMetaTypeInterfaces.Contains(resolvedType))
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

			var keyProperties = data.GetKeyProperties();
			if (keyProperties.Count == 1 && keyProperties.First().Name == "Id" && keyProperties.First().PropertyType == typeof(Guid))
			{
				var id = data.GetProperty<Guid>("Id");
				if (ExcludedIds.Contains(id))
					return;
			}

            string dataTypeName = TypeManager.SerializeType(data.DataSourceId.InterfaceType);
            string dataScopeIdentifier = DataScopeManager.CurrentDataScope.Name;

            string cultureName = string.Empty;
            ILocalizedControlled localizedData = data as ILocalizedControlled;
            if (localizedData != null)
            {
                cultureName = "?";
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
            var filePath = Path.Combine(_packageDirectoryPath, shortFilePath);

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

            string targetFilename = Path.Combine(_packageDirectoryPath, newFilename);
            string targetDirectory = Path.GetDirectoryName(targetFilename);
            if (!Directory.Exists(targetDirectory))
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
