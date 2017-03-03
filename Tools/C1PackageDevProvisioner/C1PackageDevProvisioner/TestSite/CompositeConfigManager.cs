using System;
using System.IO;
using System.Xml.Linq;

namespace C1PackageDevProvisioner.TestSite
{
    public static class CompositeConfigManager
    {
        public static void RegisterMockPackageServer()
        {
            string conpositeConfigPath = Path.Combine(Configration.C1SitePath, "App_Data\\Composite\\Composite.config");

            var configDoc = XDocument.Load(conpositeConfigPath);
            XName elementName = "Composite.SetupConfiguration";
            XElement setupUrlElement = configDoc.Root.Element(elementName);
            XAttribute urlAttribute = setupUrlElement.Attribute("PackageServerUrl");
            if (urlAttribute == null) throw new InvalidOperationException("Composite.config structure is confusing me, sorry");

            urlAttribute.Value = string.Format("http://localhost:{0}", Configration.MockPackageServerPort);
            configDoc.Save(conpositeConfigPath);

            var rootWebConfigPath = Path.Combine(Configration.C1SitePath, "web.config");
            File.SetLastWriteTimeUtc(rootWebConfigPath, DateTime.UtcNow);

        }
    }
}
