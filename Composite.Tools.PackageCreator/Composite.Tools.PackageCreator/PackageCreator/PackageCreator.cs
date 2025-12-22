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
        private const CompressionLevel PackageCompressionLevel = CompressionLevel.Optimal;
        private const string CreatorDirectory = "PackageCreator";

        private const string BlankXsl = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">

        <xsl:template match=""@* | node()"">
                <xsl:copy>
                        <xsl:apply-templates select=""@* | node()""/>
                </xsl:copy>
        </xsl:template>

</xsl:stylesheet>";

        public static readonly XNamespace Mi = "http://www.composite.net/ns/management/packageinstaller/1.0";
        public static readonly XNamespace Pc = "http://www.composite.net/ns/management/packagecreator/2.0";
        public static readonly XNamespace Help = "http://www.composite.net/ns/help/1.0";
        private static readonly XNamespace Xsl = "http://www.w3.org/1999/XSL/Transform";

        //public static readonly string configDirectoryName = "Config";
        public static readonly string ConfigInstallFileName = "Install.xsl";
        public static readonly string ConfigUninstallFileName = "Uninstall.xsl";

        public static readonly XName OrderingAttributeName = "Ordering";

        private readonly Dictionary<string, List<XElement>> _configurationTemplates =
            new Dictionary<string, List<XElement>>();

        private readonly Dictionary<string, HashSet<string>> _configurationXPaths =
            new Dictionary<string, HashSet<string>>();

        private readonly DataFileDictionary _dataFiles = new DataFileDictionary();
        private readonly Dictionary<Type, int> _dataPosition = new Dictionary<Type, int>();

        private readonly Dictionary<string, XElement> _datas = new Dictionary<string, XElement>();
        private readonly List<XElement> _directories = new List<XElement>();
        private readonly List<XElement> _files = new List<XElement>();
        private readonly List<string> _installDataTypeNamesList = new List<string>();


        private readonly string _serviceDirectoryPath;
        private readonly List<XElement> _xslFiles = new List<XElement>();
        private bool _allowOverwriteDataOnInstall;
        private string _packageDirectoryPath;
        private string _zipDirectoryPath;
        public HashSet<Guid> AlreadyAddedIds = new HashSet<Guid>();

        public HashSet<Guid> ExcludedIds = new HashSet<Guid>();
        public HashSet<string> ExcludedPaths = new HashSet<string>();

        internal PackageCreator(string path, string packageName)
        {
            _serviceDirectoryPath = path;
            PackageName = packageName;
            if (!Directory.Exists(_serviceDirectoryPath)) Directory.CreateDirectory(_serviceDirectoryPath);

            BuildDataPosition();
        }


        public static string InstallFilename => "install.xml";
        public string PackageName { get; }

        #region Creation

        public string CreatePackagePack()
        {
            using (new DataScope(DataScopeIdentifier.Public))
            {
                #region Init

                if (PackageName == string.Empty) 
                    return "";

                _packageDirectoryPath = Path.Combine(_serviceDirectoryPath, PackageName);

                if (Directory.Exists(_packageDirectoryPath)) 
                    Directory.Delete(_packageDirectoryPath, true);

                var dataPackageDirectoryPath = Path.Combine(_serviceDirectoryPath, $"{PackageName}_Data");

                _zipDirectoryPath = Path.Combine(_packageDirectoryPath, "Release");
                _packageDirectoryPath = Path.Combine(_packageDirectoryPath, "Package");

                Directory.CreateDirectory(_packageDirectoryPath);
                Directory.CreateDirectory(_zipDirectoryPath);

                if (Directory.Exists(dataPackageDirectoryPath))
                    FileSystem.CopyDirectory(dataPackageDirectoryPath, _packageDirectoryPath);

                var packageConfigFilePath = Path.Combine(_serviceDirectoryPath, $"{PackageName}.xml");

                var config = XDocument.Load(packageConfigFilePath);

                var rootConfigElement = config.Element(Pc + "PackageCreator");
                if (rootConfigElement == null)
                    throw new InvalidOperationException("Root element of config PackageCreator 2.0 not found");

                #endregion

                #region Init install.xml

                var xPackage = new XDocument
                {
                    Declaration = new XDeclaration("1.0", "utf-8", "yes")
                };

                var xPackageInstaller = new XElement(Mi + "PackageInstaller");
                xPackageInstaller.Add(new XAttribute(XNamespace.Xmlns + "mi", Mi));

                var xPackageRequirements = config.Descendants(Mi + "PackageRequirements").FirstOrDefault();
                xPackageInstaller.Add(xPackageRequirements);


                var xPackageInformation = config.Descendants(Mi + "PackageInformation").FirstOrDefault();
                _allowOverwriteDataOnInstall = xPackageInformation.AllowOverwriteAttributeValue();


                xPackageInstaller.Add(xPackageInformation);

                var xPackageFragmentInstallerBinaries = new XElement(Mi + "PackageFragmentInstallerBinaries");
                try
                {
                    xPackageFragmentInstallerBinaries =
                        config.Descendants(Mi + "PackageFragmentInstallerBinaries").First();
                }
                catch
                {
                    // ignored
                }

                xPackageFragmentInstallerBinaries.ReplaceNodes(xPackageFragmentInstallerBinaries.Elements()
                    .OrderBy(ReferencedAssemblies.AssemblyPosition));
                xPackageInstaller.Add(xPackageFragmentInstallerBinaries);


                var xPackageFragmentInstallers = new XElement(Mi + "PackageFragmentInstallers");

                #endregion

                #region Init Categories

                foreach (var packageItem in PackageCreatorActionFacade.CategoryTypes
                             .Where(categoryType => categoryType.Value.IsInitable())
                             .SelectMany(categoryType =>
                                 PackageCreatorFacade.GetItems(categoryType.Value, config.Root)))
                    (packageItem as IPackInit).Init(this);

                #endregion

                #region XML PageTemplates

                var pageTemplates = config.Descendants(Pc + "PageTemplates").FirstOrDefault();
                if (pageTemplates != null)
                    foreach (var item in pageTemplates.Elements("Add"))
                    {
                        var pageTemplate = DataFacade.GetData<IXmlPageTemplate>()
                            .FirstOrDefault(i => i.Title == item.IndexAttributeValue());

                        if (pageTemplate == null)
                            continue;

                        var newPageTemplateFilePath = $"\\{pageTemplate.Title}.xml";

                        AddFile($"App_Data\\PageTemplates{pageTemplate.PageTemplateFilePath}",
                            $"App_Data\\PageTemplates{newPageTemplateFilePath}");

                        pageTemplate.PageTemplateFilePath = newPageTemplateFilePath;
                        AddData(pageTemplate);
                    }

                #endregion

                #region Files

                var installFiles = config.Descendants(Pc + "Files").FirstOrDefault();
                if (installFiles != null)
                    foreach (var item in installFiles.Elements("Add"))
                    {
                        var filename = item.IndexAttributeValue();

                        if (string.IsNullOrEmpty(filename))
                            throw new InvalidOperationException("Files->Add attribute 'name' must be specified");

                        AddFile(filename);
                    }

                #endregion

                #region FilesInDirectory

                var installFilesInDirectory = config.Descendants(Pc + "FilesInDirectories").FirstOrDefault();
                if (installFilesInDirectory != null)
                    foreach (var item in installFilesInDirectory.Elements("Add"))
                    {
                        var path = item.IndexAttributeValue();
                        AddFilesInDirectory(path);
                    }

                #endregion

                #region Directories

                var installDirectories = config.Descendants(Pc + "Directories").FirstOrDefault();
                if (installDirectories != null)
                    foreach (var item in installDirectories.Elements("Add"))
                        try
                        {
                            var filename = item.IndexAttributeValue();

                            AddDirectory(filename);
                        }
                        catch
                        {
                        }

                #endregion

                #region Datas

                var blockElement = rootConfigElement.Element(Pc + "Datas");

                if (blockElement != null)
                    foreach (var item in blockElement.Elements("Type"))
                    {
                        var dataTypeName = item.Attribute("type").Value;
                        var dataScopeIdentifier = item.Element("Data").Attribute("dataScopeIdentifier").Value;
                        AddData(dataTypeName, dataScopeIdentifier);
                    }

                #endregion

                #region DataItems

                blockElement = rootConfigElement.Element(Pc + "DataItems");

                if (blockElement != null)
                    foreach (var item in blockElement.Elements("Type"))
                    {
                        var dataTypeName = item.Attribute("type").Value;

                        Func<XElement, Func<IData, bool>> trueAttributes = e => e.Attributes()
                            .Where(x => x.Name.Namespace == XNamespace.None)
                            .Aggregate(new Func<IData, bool>(d => true), (f, x) => d => d.GetProperty(x.Name.LocalName) == x.Value && f(d));

                        var where = item.Elements("Add")
                            .Aggregate(new Func<IData, bool>(d => false), (f, e) => d => trueAttributes(e)(d) || f(d));

                        AddData(dataTypeName, where);
                    }

                #endregion

#warning TODO: Datatypes data format depends from datatype exists in package or not, thats why list of datatype must created before adding data

                #region DynamicDataTypes

                var dynamicDataTypes = config.Descendants(Pc + "DynamicDataTypes").FirstOrDefault();
                XElement dynamicDataTypePackageFragmentInstaller = null;
                if (dynamicDataTypes != null)
                {
                    var dataTypes = new List<XElement>();

                    foreach (var item in dynamicDataTypes.Elements("Add").OrderBy(e => DataPosition(e.IndexAttributeValue())))
                    {
                        var dynamicDataType = TypeManager.GetType(item.IndexAttributeValue());
                        var dynamicDataTypeName = TypeManager.SerializeType(dynamicDataType);

                        AddData<IGeneratedTypeWhiteList>(d => d.TypeManagerTypeName == dynamicDataTypeName);

                        _installDataTypeNamesList.Add(dynamicDataTypeName);
                        var type = TypeManager.GetType(item.IndexAttributeValue());
                        var dataTypeDescriptor = DynamicTypeManager.GetDataTypeDescriptor(type);

                        dataTypes.Add(new XElement("Type",
                            new XAttribute("providerName", "GeneratedDataTypesElementProvider"),
                            new XAttribute("dataTypeDescriptor", dataTypeDescriptor.ToXml().ToString()),
                            new XAttribute("allowOverwrite", _allowOverwriteDataOnInstall))
                        );

                        AddFileIfExists($"App_Data\\Composite\\DynamicTypeForms\\{item.IndexAttributeValue().Replace('.', '\\')}.xml");
                    }

                    dynamicDataTypePackageFragmentInstaller = new XElement(Mi + "Add",
                        new XAttribute("installerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.DynamicDataTypePackageFragmentInstaller, Composite"),
                        new XAttribute("uninstallerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.DynamicDataTypePackageFragmentUninstaller, Composite"),
                        new XElement("Types", dataTypes));
                }

                #endregion

                var guidReplacer = new GuidReplacer(DataFacade.GetData<ICompositionContainer>()
                    .Select(cd => new KeyValuePair<Guid, Guid>(cd.GetProperty<Guid>("Id"), GuidReplacer.CompositionContainerGuid)));

                //Add only one MetaTypeTab
                using (guidReplacer)
                {
                    var packageItems = PackageCreatorActionFacade.CategoryTypes
                        .Where(categoryType => categoryType.Value.IsPackagable())
                        .SelectMany(categoryType => PackageCreatorFacade.GetItems(categoryType.Value, config.Root));

                    foreach (var packageItem in packageItems)
                        (packageItem as IPack).Pack(this);

                    #region DynamicDataTypesData

                    var element = config.Descendants(Pc + "DynamicDataTypesData").FirstOrDefault();

                    if (element != null)
                        foreach (var item in element.Elements("Add"))
                        {
                            var indexAttrValue = item.IndexAttributeValue();
                            AddDataTypeData(TypeManager.TryGetType(indexAttrValue));
                        }
                }

                #endregion

                #region TransformationPackageFragmentInstallers

                XElement configurationTransformationPackageFragmentInstallers = null;

                var sources = new HashSet<string>();
                sources.UnionWith(_configurationXPaths.Keys);
                sources.UnionWith(_configurationTemplates.Keys);

                foreach (var source in sources)
                {
                    var installXsl = XDocument.Parse(BlankXsl);
                    var uninstallXsl = XDocument.Parse(BlankXsl);
                    
                    if (_configurationXPaths.TryGetValue(source, out var xPaths))
                    {
                        var nodes = new Dictionary<string, Dictionary<string, XElement>>();

                        foreach (var xpath in xPaths)
                        {
                            var configuration = PackageCreatorFacade.GetConfigurationDocument(source);
                            var element = configuration.XPathSelectElement(xpath);
                            var re = new Regex(@"^(.*?)/([^/]*(\[[^\]]*\])?)$");
                            var match = re.Match(xpath);

                            if (!match.Success)
                                continue;

                            var itemPath = match.Groups[1].Value;
                            var itemKey = match.Groups[2].Value;

                            nodes.Add(itemPath, itemKey, element);

                            uninstallXsl.Root.Add(new XElement(Xsl + "template", new XAttribute("match", xpath)));
                        }

                        foreach (var node in nodes)
                            installXsl.Root.Add(
                                new XElement(Xsl + "template", new XAttribute("match", node.Key),
                                    new XElement(Xsl + "copy",
                                        new XElement(Xsl + "apply-templates", new XAttribute("select", "@* | node()")),
                                        from e in node.Value
                                        select
                                            new XElement(Xsl + "if", new XAttribute("test", $"count({e.Key})=0"),
                                                e.Value
                                            )
                                    )
                                )
                            );
                    }

                    if (_configurationTemplates.TryGetValue(source, out var templates)) 
                        installXsl.Root.Add(templates);

                    var configDirectory = Path.Combine(_packageDirectoryPath, source);

                    if (!Directory.Exists(configDirectory))
                        Directory.CreateDirectory(configDirectory);

                    installXsl.SaveTabbed(Path.Combine(configDirectory, ConfigInstallFileName));
                    uninstallXsl.SaveTabbed(Path.Combine(configDirectory, ConfigUninstallFileName));

                    if (PCCompositeConfig.Source == source)
                        configurationTransformationPackageFragmentInstallers =
                            new XElement(Mi + "Add",
                                new XAttribute("installerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.ConfigurationTransformationPackageFragmentInstaller, Composite"),
                                new XAttribute("uninstallerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.ConfigurationTransformationPackageFragmentUninstaller, Composite"),
                                new XElement("Install", new XAttribute("xsltFilePath", $@"~\{source}\{ConfigInstallFileName}")
                                ),
                                new XElement("Uninstall",
                                    new XAttribute("xsltFilePath", $@"~\{source}\{ConfigUninstallFileName}")
                                )
                            );
                    else
                        _xslFiles.Add(
                            new XElement("XslFile",
                                new XAttribute("pathXml", PackageCreatorFacade.GetConfigurationPath(source)),
                                new XAttribute("installXsl", $@"~\{source}\{ConfigInstallFileName}"),
                                new XAttribute("uninstallXsl", $@"~\{source}\{ConfigUninstallFileName}")
                            )
                        );
                }

                #endregion


                var xPackageFragments = config.Descendants(Pc + "PackageFragmentInstallers").FirstOrDefault();
                if (xPackageFragments != null) 
                    xPackageFragmentInstallers.Add(xPackageFragments.Elements());


                xPackageFragmentInstallers.Add(configurationTransformationPackageFragmentInstallers);

                #region FilePackageFragmentInstaller

                if (_files.Count + _directories.Count > 0)
                {
                    var filePackageFragmentInstaller = new XElement(Mi + "Add",
                        new XAttribute("installerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.FilePackageFragmentInstaller, Composite"),
                        new XAttribute("uninstallerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.FilePackageFragmentUninstaller, Composite"),
                        _files.Count == 0
                            ? null
                            : new XElement("Files", _files.OrderBy(ReferencedAssemblies.AssemblyPosition)),
                        _directories.Count == 0 ? null : new XElement("Directories", _directories));

                    xPackageFragmentInstallers.Add(filePackageFragmentInstaller);
                }

                #endregion


                #region FileXslTransformationPackageFragmentInstaller

                if (_xslFiles.Count > 0)
                    xPackageFragmentInstallers.Add(new XElement(Mi + "Add",
                        new XAttribute("installerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.FileXslTransformationPackageFragmentInstaller, Composite"),
                        new XAttribute("uninstallerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.FileXslTransformationPackageFragmentUninstaller, Composite"),
                        new XElement("XslFiles", _xslFiles)));

                #endregion

                xPackageFragmentInstallers.Add(dynamicDataTypePackageFragmentInstaller);

                #region DataPackageFragmentInstaller

                if (_datas.Count > 0)
                {
                    var dataPackageFragmentInstaller = new XElement(Mi + "Add",
                        new XAttribute("installerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.DataPackageFragmentInstaller, Composite"),
                        new XAttribute("uninstallerType", "Composite.Core.PackageSystem.PackageFragmentInstallers.DataPackageFragmentUninstaller, Composite"),
                        new XElement("Types",
                            _datas.OrderBy(t => t.Key)
                                .ThenBy(t => DataPosition(t.Value.AttributeValue("type")))
                                .Select(t => t.Value)));

                    xPackageFragmentInstallers.Add(dataPackageFragmentInstaller);
                }

                #endregion

                _dataFiles.Save();

                xPackageFragmentInstallers
                    .ReplaceNodes(xPackageFragmentInstallers.Elements()
                    .OrderBy(FragmentPosition)
                    .Select(DeleteOrderingMark));

                xPackageInstaller.Add(xPackageFragmentInstallers);

                xPackage.Add(xPackageInstaller);

                xPackage.SaveTabbed(Path.Combine(_packageDirectoryPath, InstallFilename));

                var zipFilename = $"{PackageName}.zip";


                #region Zipping

                var filenames = Directory.GetFiles(_packageDirectoryPath, "*", SearchOption.AllDirectories);
                var directories = Directory.GetDirectories(_packageDirectoryPath, "*", SearchOption.AllDirectories);


                using (var archiveFileStream = File.Create(Path.Combine(_zipDirectoryPath, zipFilename)))
                using (var archive = new ZipArchive(archiveFileStream, ZipArchiveMode.Create))
                {
                    foreach (var directory in directories)
                    {
                        var directoryEntryName = $"{directory.Replace($"{_packageDirectoryPath}\\", "").Replace("\\", "/")}/";

                        var dirEntry = archive.CreateEntry(directoryEntryName);
                        dirEntry.LastWriteTime = Directory.GetLastWriteTime(directory);
                    }

                    foreach (var file in filenames)
                    {
                        var fileEntryName = file.Replace($"{_packageDirectoryPath}\\", "").Replace("\\", "/");

                        var entry = archive.CreateEntry(fileEntryName, PackageCompressionLevel);

                        using (var fileStream = File.OpenRead(file))
                        using (var entryStream = entry.Open())
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    }
                }

                #endregion

                var packageCreatorDirectory = Path.Combine(_packageDirectoryPath, $"../_{CreatorDirectory}");
                try
                {
                    Directory.CreateDirectory(packageCreatorDirectory);

                    var packageCopyFilePath = $@"{packageCreatorDirectory}\{PackageName}.xml";
                    File.Copy(packageConfigFilePath, packageCopyFilePath);
                    SetFileReadAccess(packageCopyFilePath, false);
                }
                catch
                {
                    // ignored
                }

                try
                {
                    if (Directory.Exists(dataPackageDirectoryPath))
                        FileSystem.CopyDirectory(dataPackageDirectoryPath, $@"{packageCreatorDirectory}\{PackageName}_Data");
                }
                catch
                {
                    // ignored
                }

                return Path.Combine(PackageName, "Release", zipFilename);
            }
        }

        public void AddDirectory(string filename)
        {
            var targetFilename = Path.Combine(_packageDirectoryPath, filename);
            var targetDirectory = Path.GetDirectoryName(targetFilename);
            if (!Directory.Exists(targetDirectory)) 
                Directory.CreateDirectory(targetDirectory);

            FileSystem.CopyDirectory(Path.Combine(PathUtil.Resolve(PathUtil.BaseDirectory), filename), targetFilename);

            _directories.Add(
                new XElement("Directory",
                    new XAttribute("sourceDirectory", $"~\\{filename}"),
                    new XAttribute("targetDirectory", $"~\\{filename}"),
                    new XAttribute("allowOverwrite", _allowOverwriteDataOnInstall),
                    new XAttribute("deleteTargetDirectory", "false")
                )
            );
        }

        internal void AddFilesInDirectory(string path)
        {
            var baseDirectory = PathUtil.Resolve(PathUtil.BaseDirectory);

            foreach (var filename in Directory.GetFiles(Path.Combine(baseDirectory, path), "*", SearchOption.AllDirectories)) 
                AddFile(filename.Replace(baseDirectory, ""));
        }

        #region SortingByForeignKey

        private int DataPosition(string dataName)
        {
            try
            {
                var type = TypeManager.GetType(dataName);
                return DataPosition(type);
            }
            catch
            {
                // ignored
            }

            return int.MaxValue;
        }

        private int DataPosition(Type type)
        {
            try
            {
                return _dataPosition[type];
            }
            catch
            {
                // ignored
            }

            return int.MaxValue;
        }

        public void BuildDataPosition()
        {
            var foreignTypes = new Dictionary<Type, IEnumerable<Type>>();

            foreach (var type in DataFacade.GetAllInterfaces())
                try
                {
                    foreignTypes.Add(type, GetForeignTypes(type));
                }
                catch
                {
                    // ignored
                }

            for (var i = 1; i <= 100; i++)
            {
                var toRemove = new List<Type>();
                foreach (var type in foreignTypes.Where(type => !type.Value.Any()))
                {
                    _dataPosition.Add(type.Key, i);
                    toRemove.Add(type.Key);
                }

                foreach (var key in toRemove) 
                    foreignTypes.Remove(key);

                var prevDataPosition = _dataPosition.Keys.ToList();
                foreignTypes = foreignTypes.ToDictionary(d => d.Key, d => d.Value.Except(prevDataPosition));
            }

            foreach (var type in foreignTypes) 
                _dataPosition.Add(type.Key, int.MaxValue);

            //IDataItemTreeAttachmentPoint does not have foreign key but referenced to other types
            _dataPosition[typeof(IDataItemTreeAttachmentPoint)] = int.MaxValue;
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
                return int.Parse(element.AttributeValue(OrderingAttributeName));
            }
            catch
            {
                // ignored
            }

            return 0;
        }

        public static XElement DeleteOrderingMark(XElement element)
        {
            element.SetAttributeValue(OrderingAttributeName, null);
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
            _configurationXPaths.Add(source, xpath);
        }

        public void AddConfigurationInstallTemplate(string source, XElement template)
        {
            _configurationTemplates.Add(source, template);
        }

        #endregion

        #region AddData

        internal void AddData(Type type)
        {
            AddData(type, d => true);
        }

        internal void AddData<T>() where T : class, IData
        {
            AddData(typeof(T));
        }

        public void AddData<T>(Func<T, bool> where) where T : class, IData
        {
            AddData(typeof(T), Func);
            return;

            bool Func(IData d)
            {
                return where((T)d);
            }
        }

        internal void AddData(Type type, Func<IData, bool> where)
        {
            foreach (var dataScopeIdentifier in DataFacade.GetSupportedDataScopes(type))
                AddData(type, dataScopeIdentifier, where);
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
            AddData(typeof(T), dataScopeIdentifier, Func);
            return;

            bool Func(IData d)
            {
                return where((T)d);
            }
        }

        internal void AddData(Type type, DataScopeIdentifier dataScopeIdentifier, Func<IData, bool> where)
        {
            using (var conn = new DataConnection(dataScopeIdentifier.ToPublicationScope()))
            {
                conn.DisableServices();

                foreach (var data in DataFacade.GetData(type).ToDataEnumerable().Where(where).OrderBy(d => d.GetSelfPosition())) 
                    AddData(data);
            }
        }


        internal void AddDataTypeData(Type type)
        {
            var globalDataTypeInterfaces = DataFacade
                .GetAllInterfaces(UserType.Developer)
                .Where(interfaceType => interfaceType.Assembly != typeof(IData).Assembly)
                .OrderBy(t => t.FullName)
                .Except(PageFolderFacade.GetAllFolderTypes())
                .Except(PageMetaDataFacade.GetAllMetaDataTypes());

            var pageDataTypeInterfaces = PageFolderFacade.GetAllFolderTypes();
            var pageMetaTypeInterfaces = PageMetaDataFacade.GetAllMetaDataTypes();

            var dataTypeDescriptor = DynamicTypeManager.GetDataTypeDescriptor(type);

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
                foreach (var j in DataFacade.GetData<IPageMetaDataDefinition>(d =>
                             d.MetaDataTypeId == dataTypeDescriptor.DataTypeId))
                    //Add only one MetaTypeTab
                    AddData<ICompositionContainer>(d => d.Id == j.MetaDataContainerId);
                AddData<IPageMetaDataDefinition>(d => d.MetaDataTypeId == dataTypeDescriptor.DataTypeId);
                AddData(type);
            }
        }

        public void AddData(IData data)
        {
            // #3102 Do not export ICompositionContainer with id eb210a75-be25-401f-b0d4-b3787bce36fa - considered always installed
            if (data is ICompositionContainer container && container.Id == new Guid("eb210a75-be25-401f-b0d4-b3787bce36fa")) 
                return;

            var keyProperties = data.GetKeyProperties();

            if (keyProperties.Count == 1 && keyProperties.First().Name == "Id" && keyProperties.First().PropertyType == typeof(Guid) && ExcludedIds.Contains(data.GetProperty<Guid>("Id"))) 
                return;

            var dataTypeName = TypeManager.SerializeType(data.DataSourceId.InterfaceType);
            var dataScopeIdentifier = DataScopeManager.CurrentDataScope.Name;

            var cultureName = string.Empty;
            if (data is ILocalizedControlled) 
                cultureName = "?";

            var dataTypeKey = $"{dataTypeName}{dataScopeIdentifier}{(string.IsNullOrEmpty(cultureName) ? string.Empty : $"_{cultureName}")}";
            var dataTypeFileName = $"{Regex.Replace(dataTypeName, "(.+:|,.+)", "")}s_{dataScopeIdentifier}{(string.IsNullOrEmpty(cultureName) || cultureName.Length <= 1 ? string.Empty : $"_{cultureName}")}.xml";


            //Add only one MetaTypeTab
            if (_datas.ContainsKey(dataTypeKey) && dataTypeName.Contains("Composite.Data.Types.ICompositionContainer")) 
                return;

            var shortFilePath = $"Datas\\{dataTypeFileName}";
            var filePath = Path.Combine(_packageDirectoryPath, shortFilePath);

            if (!_datas.ContainsKey(dataTypeKey))
                _datas.Add(
                    dataTypeKey,
                    new XElement("Type",
                        _installDataTypeNamesList.Contains(dataTypeName)
                            ? new XAttribute("isDynamicAdded", "true")
                            : null,
                        _allowOverwriteDataOnInstall ? new XAttribute("allowOverwrite", "true") : null,
                        new XAttribute("type", dataTypeName),
                        new XElement("Data",
                            new XAttribute("dataScopeIdentifier", dataScopeIdentifier),
                            string.IsNullOrEmpty(cultureName) ? null : new XAttribute("locale", cultureName),
                            new XAttribute("dataFilename", $"~\\{shortFilePath}")
                        )
                    )
                );

            _dataFiles.Add(filePath,
                new XElement("Add",
                    TypeManager.GetType(dataTypeName)
                        .GetPropertiesRecursively()
                        .Where(propertyInfo => propertyInfo.CanWrite && propertyInfo.CanRead)
                        .Select(propertyInfo => new { s = propertyInfo, val = propertyInfo.GetValue(data, null) })
                        .Where(f => f.val != null)
                        .Select(f => new XAttribute(f.s.Name, SerializeDataField(f.val))))
            );
        }

        private static string SerializeDataField(object o)
        {
            switch (o)
            {
                case Guid guid:
                    return GuidReplacer.Replace(guid).ToString();
                case DateTime time:
                    return time.ToString("u");
                default:
                    return o.ToString();
            }
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

            var targetFilename = Path.Combine(_packageDirectoryPath, newFilename);
            var targetDirectory = Path.GetDirectoryName(targetFilename);

            if (!Directory.Exists(targetDirectory)) 
                Directory.CreateDirectory(targetDirectory);

            File.Copy(Path.Combine(PathUtil.Resolve(PathUtil.BaseDirectory), filename), targetFilename, true);

            _files.Add(
                new XElement("File",
                    new XAttribute("sourceFilename", "~\\" + newFilename),
                    new XAttribute("targetFilename", "~\\" + newFilename),
                    new XAttribute("allowOverwrite", _allowOverwriteDataOnInstall)
                ));
        }

        #endregion

        #endregion
    }
}