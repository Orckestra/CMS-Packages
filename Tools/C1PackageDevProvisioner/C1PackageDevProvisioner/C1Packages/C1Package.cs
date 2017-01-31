using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace C1PackageDevProvisioner.C1Packages
{
    public class C1Package
    {
        public C1Package(string installXmlPath, string projectPath)
        {
            this.InstallXmlPath = installXmlPath;
            this.ProjectFilesBasePath = Directory.GetParent(Path.GetDirectoryName(installXmlPath)).FullName;
            while (!Directory.GetFiles(this.ProjectFilesBasePath,"*.sln",SearchOption.TopDirectoryOnly).Any() && this.ProjectFilesBasePath.Length > projectPath.Length)
            {
                this.ProjectFilesBasePath = Directory.GetParent(this.ProjectFilesBasePath).FullName;
            }

            InitFromInstallXml();
        }


        private void Compile()
        {
            var nugetExePath = Configration.NugetPath;

            Process nugetProcess = new Process();
            nugetProcess.StartInfo.FileName = nugetExePath;
            nugetProcess.StartInfo.Arguments = "restore";
            nugetProcess.StartInfo.WorkingDirectory = this.ProjectFilesBasePath;
            nugetProcess.StartInfo.CreateNoWindow = true;
            nugetProcess.StartInfo.UseShellExecute = false;
            nugetProcess.StartInfo.RedirectStandardInput = false;
            nugetProcess.StartInfo.RedirectStandardOutput = true;
            nugetProcess.Start();
            nugetProcess.WaitForExit();

            if (nugetProcess.ExitCode != 0)
            {
                EventInfo.Queue.Enqueue("Nuget restore failed in " + this.ProjectFilesBasePath);
            }

            Process msBuildProcess = new Process();
            msBuildProcess.StartInfo.FileName = Configration.MsBuildPath;
            msBuildProcess.StartInfo.WorkingDirectory = this.ProjectFilesBasePath;
            nugetProcess.StartInfo.CreateNoWindow = true;
            nugetProcess.StartInfo.UseShellExecute = false;
            nugetProcess.StartInfo.RedirectStandardInput = false;
            nugetProcess.StartInfo.RedirectStandardOutput = true;
            msBuildProcess.Start();
            msBuildProcess.WaitForExit();

            if (msBuildProcess.ExitCode != 0)
            {
                EventInfo.Queue.Enqueue("MSBuild failed in " + this.ProjectFilesBasePath);
            }

        }


        public string Name { get; private set; }
        public string Author { get; private set; }
        public string GroupName { get; private set; }
        public string Version { get; private set; }
        public string MinRequiredVersion { get; private set; }
        public string MaxRequiredVersion { get; private set; }

        public string Website { get; private set; }
        public string ReadMoreUrl { get; private set; }
        public string Description { get; private set; }
        public string TechnicalDetails { get; private set; }

        public Guid Id { get; private set; }
        public string InstallXmlPath { get; private set; }
        public Dictionary<string, string> FileMappings { get; private set; }
        public string ProjectFilesBasePath { get; private set; }
        public string PackageZipPath
        {
            get
            {
                var zips = GetPackageZips();

                if (zips.Count==0)
                {
                    Compile();
                    zips = GetPackageZips();
                }

                if (!zips.Any())
                {
                    EventInfo.Queue.Enqueue(string.Format("Unable to compile and find package ZIP for project in {0}", this.ProjectFilesBasePath));
                    return null;
                }
                else
                {
                    var zipPath = zips.First();
                    var zipLastChange = (new FileInfo(zipPath)).LastWriteTime;

                    var projectLastChange = Directory.GetFiles(this.ProjectFilesBasePath, "*.*", SearchOption.AllDirectories).Select(f => (new FileInfo(f)).LastWriteTime).Max();

                    if (projectLastChange > zipLastChange.Add(TimeSpan.FromSeconds(5)))
                    {
                        Compile();
                    }

                }

                if (zips.Count() == 1)
                {
                    return zips.First(); ;
                }
                else
                {
                    return zips.Where(f => f.Contains("release") || f.Contains("Release")).FirstOrDefault();
                }
            }
        }


        private List<string> GetPackageZips()
        {
            return Directory.GetFiles(this.ProjectFilesBasePath, "*.zip", SearchOption.AllDirectories).Where(f => f.Contains("release") || f.Contains("Release") || f.ToLower().Contains(this.Name.ToLower())).ToList();
        }


        public bool IsInstalled
        {
            get
            {
                var c1PackagesFolderPath = FileUtil.MapRelative(Configration.C1SitePath, "~/App_Data/Composite/Packages");
                return Directory.Exists(Path.Combine(c1PackagesFolderPath, this.Id.ToString()));
            }
        }


        public void RefreshFileMappings()
        {
            XDocument installXml = XDocument.Load(this.InstallXmlPath);
            this.FileMappings = GetFileMappings(installXml);
        }


        private const string installerType_File = "Composite.Core.PackageSystem.PackageFragmentInstallers.FilePackageFragmentInstaller, Composite";


        private void InitFromInstallXml()
        {
            XDocument installXml = XDocument.Load(this.InstallXmlPath);

            var packageInformationElement = installXml.Root.Element(Namespaces.PackageInstallerNs + "PackageInformation");
            var packageRequirementsElement = installXml.Root.Element(Namespaces.PackageInstallerNs + "PackageRequirements");

            this.Name = (string)packageInformationElement.Attribute("name");
            this.Author = (string)packageInformationElement.Attribute("author");
            this.Website = (string)packageInformationElement.Attribute("website");
            this.ReadMoreUrl = (string)packageInformationElement.Attribute("readMoreUrl");
            this.TechnicalDetails = (packageInformationElement.Element("TechnicalDetails") == null ? "" : packageInformationElement.Element("TechnicalDetails").Value);
            this.Description = (packageInformationElement.Element("Description") == null ? "" : packageInformationElement.Element("Description").Value);
            this.GroupName = (string)packageInformationElement.Attribute("groupName");
            this.Version = (string)packageInformationElement.Attribute("version");
            this.Id = (Guid)packageInformationElement.Attribute("id");
            this.FileMappings = GetFileMappings(installXml);

            this.MinRequiredVersion = (string)packageRequirementsElement.Attribute("minimumCompositeVersion");
            this.MaxRequiredVersion = (string)packageRequirementsElement.Attribute("maximumCompositeVersion");
        }


        private Dictionary<string, string> GetFileMappings(XDocument installXml)
        {
            var mappings = new Dictionary<string, string>();
            var basePath = Path.GetDirectoryName(this.InstallXmlPath);

            var fileInstallerElements = installXml.Descendants().Elements(Namespaces.PackageInstallerNs + "Add").Where(f => (string)f.Attribute("installerType") == installerType_File);

            foreach (var fileElement in fileInstallerElements.Elements("Files").Elements("File"))
            {
                var source = (string)fileElement.Attribute("sourceFilename");
                var target = (string)fileElement.Attribute("targetFilename");
                var fullSource = FileUtil.MapRelative(basePath, source);
                if (!mappings.ContainsKey(fullSource))
                {
                    mappings.Add(
                        FileUtil.MapRelative(basePath, source),
                        FileUtil.MapRelative(Configration.C1SitePath, target));
                }
            }

            foreach (var dirElement in fileInstallerElements.Elements("Directories").Elements("Directory"))
            {
                var relSourceDir = (string)dirElement.Attribute("sourceDirectory");
                var relTargetDir = (string)dirElement.Attribute("targetDirectory");

                var mappedSourceDir = FileUtil.MapRelative(basePath, relSourceDir);

                var sourceFilePaths = Directory.GetFiles(mappedSourceDir, "*.*", SearchOption.AllDirectories);

                foreach (var sourceFilePath in sourceFilePaths)
                {
                    var relTargetFilePath = sourceFilePath.Substring(basePath.Length + 1);

                    mappings.Add(
                        sourceFilePath,
                        FileUtil.MapRelative(Configration.C1SitePath, relTargetFilePath));
                }
            }

            return mappings;
        }
    }
}
