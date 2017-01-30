using C1PackageDevProvisioner.Packages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace C1PackageDevProvisioner.PackageServer
{
    public static class PackageServerConfigManager
    {
        public static void UpdatePackageServerConfig()
        {

            List<C1Package> knownPackages = PackageManager.GetPackages().ToList();

            UpdateSetupDescription(knownPackages);
            UpdatePackageList(knownPackages);
        }

        private static void UpdatePackageList(List<C1Package> knownPackages)
        {
            XDocument packageDocument = new XDocument();
            packageDocument.Add(new XElement("Packages"));
            foreach (var package in knownPackages.Where(f=>f.PackageZipPath != null))
            {
                packageDocument.Root.Add(new XElement("Package"
                    , new XAttribute("Id", package.Id)
                    , new XAttribute("Name", package.Name ?? "***MISSING***")
                    , new XAttribute("GroupName", package.GroupName ?? "***MISSING***")
                    , new XAttribute("Version", package.Version ?? "0.0.0.0")
                    , new XAttribute("MinimumCompositeVersion", package.MinRequiredVersion ?? "0.0.0.0")
                    , new XAttribute("MaximumCompositeVersion", package.MaxRequiredVersion ?? "0.0.0.0")
                    , new XAttribute("PackagePath", package.PackageZipPath)
                    , new XAttribute("Author", package.Author ?? "")
                    , new XAttribute("ReadMoreUrl", package.ReadMoreUrl ?? "")
                    , new XAttribute("TechicalDetails", package.TechnicalDetails ?? "")
                    , new XAttribute("Description", package.Description ?? "")
                    ));
            }

            foreach (var package in knownPackages.Where(f => f.PackageZipPath == null))
            {
                EventInfo.Queue.Enqueue(string.Format("{0} has no Package ZIP, not exposed on PackageList", package.Name));
            }

            var packageDocumentPath = Path.Combine(Configration.MockPackageServerPath, "App_Data/Setup/Packages.xml");

            packageDocument.Save(packageDocumentPath);
        }

        private static void UpdateSetupDescription(List<C1Package> knownPackages)
        {
            var setupDescriptionDoc = XDocument.Load(Configration.SetupDescriptionPath);

            foreach (var packageElement in setupDescriptionDoc.Descendants().Where(e => e.Name.LocalName == "package" && e.Attribute("id") != null))
            {
                var matchingPackages = knownPackages.Where(p => p.Id.Equals((Guid)packageElement.Attribute("id")));

                int matchCount = matchingPackages.Count();

                if (matchCount == 0)
                {
                    try
                    {
                        Uri packageUri = new Uri(packageElement.Attribute("url").Value);
                    }
                    catch (Exception)
                    {
                        throw new InvalidOperationException("SetupDescription has unknown package id " + packageElement.Attribute("id") + " with a non valid URL " + packageElement.Attribute("url").Value);
                    }
                }
                if (matchCount > 1) throw new InvalidOperationException("Multiple packages in project repo has id " + packageElement.Attribute("id") + " and that make me a sad and confused program. Packages are " + string.Join(", ", matchingPackages.Select(f => f.InstallXmlPath)));

                if (matchCount == 1)
                {
                    var matchingPackage = matchingPackages.First();
                    var zipPath = matchingPackage.PackageZipPath;
                    if (zipPath == null) throw new InvalidOperationException("Packages in project repo did not build (using MSBuild from command line): " + matchingPackage.Name);

                    packageElement.Attribute("url").Value = "/Setup/Download.ashx?path=" + zipPath;
                }
            }

            var mockServerSetupDescriptionPath = Path.Combine(Configration.MockPackageServerPath, "App_Data/Setup/SetupDescription.xml");

            setupDescriptionDoc.Save(mockServerSetupDescriptionPath);
        }
    }
}
