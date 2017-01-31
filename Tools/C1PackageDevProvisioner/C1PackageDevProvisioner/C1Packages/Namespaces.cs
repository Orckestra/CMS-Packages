using System.Xml.Linq;

namespace C1PackageDevProvisioner.C1Packages
{
    public static class Namespaces
    {
        public static XNamespace PackageInstallerNs
        {
            get
            {
                return "http://www.composite.net/ns/management/packageinstaller/1.0";
            }
        }

    }
}
