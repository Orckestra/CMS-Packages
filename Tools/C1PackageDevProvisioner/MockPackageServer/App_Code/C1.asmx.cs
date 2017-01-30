using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;

namespace Composite.Package.Server
{
    [WebService(Namespace = "http://package.composite.net/package.asmx")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class Packages : WebService
	{
		[WebMethod]
		public List<PackageDescriptor> GetPackageList(Guid InstallationId, string Culture)
		{
            List<PackageDescriptor> myPackages = new List<PackageDescriptor>();

            XDocument packagesDoc = XDocument.Load(HttpContext.Current.Request.MapPath("~/App_Data/Setup/Packages.xml"));

            foreach (var packageElement in packagesDoc.Root.Elements("Package"))
            {
                myPackages.Add(new PackageDescriptor
                {
                    Id = (Guid)packageElement.Attribute("Id"),
                    Name = (string)packageElement.Attribute("Name"),
                    GroupName = (string)packageElement.Attribute("GroupName"),
                    MinCompositeVersionSupported = (string)packageElement.Attribute("MinimumCompositeVersion"),
                    MaxCompositeVersionSupported = (string)packageElement.Attribute("MaximumCompositeVersion"),
                    PackageVersion = (string)packageElement.Attribute("Version"),
                    Author = (string)packageElement.Attribute("Author"),
                    ReadMoreUrl = (string)packageElement.Attribute("ReadMoreUrl"),
                    TechicalDetails = (string)packageElement.Attribute("TechicalDetails"),
                    Description = (string)packageElement.Attribute("Description")
                });
            }

            return myPackages;
		}

		[WebMethod]
		public bool IsOperational()
		{
			return true;
		}

		[WebMethod]
		public string GetEulaText(Guid eulaId, string userCulture)
		{
            return "Mock eula";
		}

		[WebMethod]
		public void RegisterPackageUninstall(Guid InstallationId, Guid PackageId, string localUserName, string localUserIp)
		{
		}

		[WebMethod]
		public void PackageActivity(Guid installationId, Guid packageId, string packageName, string ip, int status, string exception = null)
		{
		}


		[WebMethod]
		public bool RequestLicenseUpdate(Guid InstallationId, Guid PackageId, string localUserName, string localUserIp)
		{
            return false;
		}

		[WebMethod]
		public void RegisterPackageInstallationCompletion(Guid InstallationId, Guid PackageId, string LocalUserName, string localUserIp)
		{
		}

		[WebMethod]
		public void RegisterPackageInstallationFailure(Guid InstallationId, Guid PackageId, string LocalUserName, string localUserIp, string exceptionString)
		{
		}

	}
}
