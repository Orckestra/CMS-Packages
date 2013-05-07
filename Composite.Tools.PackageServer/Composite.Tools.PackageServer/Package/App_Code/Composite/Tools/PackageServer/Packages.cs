using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using Composite.Core.PackageSystem.WebServiceClient;
using Composite.Core.WebClient;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Tools.PackageServer
{
	[WebService(Namespace = "http://package.composite.net/package.asmx")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class Packages : System.Web.Services.WebService
	{

		[WebMethod]
		public List<PackageDescriptor> GetPackageList(Guid InstallationId, string Culture)
		{
			using (DataConnection connection = new DataConnection())
			{
				return (
					from package in connection.Get<Package>().AsEnumerable()
					join media in connection.Get<IMediaFile>() on package.PackageFile equals media.KeyPath
					select new PackageDescriptor()
					{
						Id = package.PackageId,
						Name = package.Name,
						GroupName = package.GroupName,
						PackageVersion = package.PackageVersion,
						MinCompositeVersionSupported = package.MinCompositeVersionSupported,
						MaxCompositeVersionSupported = package.MaxCompositeVersionSupported,
						Author = package.Author,
						Description = package.Description,
						TechicalDetails = package.TechnicalDetails,
						EulaId = package.EULA,
						ReadMoreUrl = package.ReadMoreUrl,
						IsFree = true,
						PackageFileDownloadUrl = new Uri(Context.Request.Url, UrlUtils.ResolvePublicUrl("Renderers/ShowMedia.ashx?id=" + media.Id)).OriginalString
					}
				).ToList();
			}
		}

		[WebMethod]
		public string GetEulaText(Guid eulaId, string userCulture)
		{
			using (DataConnection connection = new DataConnection())
			{
				return connection.Get<EULA>().Where(d => d.Id == eulaId).Select(d => d.Text).FirstOrDefault();
			};
		}

		[WebMethod]
		public bool IsOperational()
		{
			return true;
		}

		[WebMethod]
		public void RegisterPackageUninstall(Guid InstallationId, Guid PackageId, string localUserName, string localUserIp)
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