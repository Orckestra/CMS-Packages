using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using Composite.Core;
using Composite.Core.PackageSystem;

namespace Composite.StarterSite.CorporateSite.Demo
{
	public static class Package
	{
		public static bool InstallPackage(Guid packageId)
		{
			if (!IsInstalled(packageId))
			{
				var packageDescription = GetPackageDescription(packageId);
				if (packageDescription != null)
				{
					Log.LogInformation("InstallPackage", string.Format("Going to install {0} ver.{1}", packageDescription.Name, packageDescription.PackageVersion));
					return InstallPackage(packageDescription.PackageFileDownloadUrl);
				}
			}

			return false;
		}

		public static bool InstallPackage(string packageUrl)
		{
			Log.LogInformation("InstallPackage", "downloading.." + packageUrl);
			bool result = false;
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(packageUrl);
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();

				byte[] buffer = new byte[32768];
				using (Stream inputStream = response.GetResponseStream())
				{
					using (MemoryStream outputStream = new MemoryStream())
					{
						int read;
						while ((read = inputStream.Read(buffer, 0, 32768)) > 0)
						{
							outputStream.Write(buffer, 0, read);
						}

						outputStream.Seek(0, SeekOrigin.Begin);

						PackageManagerInstallProcess packageManagerInstallProcess = PackageManager.Install(outputStream, true);
						if (packageManagerInstallProcess.PreInstallValidationResult.Count > 0)
						{
							LogValidationResults(packageManagerInstallProcess.PreInstallValidationResult);
						}
						else
						{
							List<PackageFragmentValidationResult> validationResult = packageManagerInstallProcess.Validate();

							if (validationResult.Count > 0)
							{
								LogValidationResults(validationResult);
							}
							else
							{
								List<PackageFragmentValidationResult> installResult = packageManagerInstallProcess.Install();
								if (installResult.Count > 0)
								{
									LogValidationResults(installResult);
								}
								else
								{
									result = true;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.LogCritical("InstallPackage", "Error installing package: " + packageUrl);
				Log.LogCritical("InstallPackage", ex);

				throw;
			}
			Log.LogInformation("InstallPackage", "Installed package: " + packageUrl);
			return result;
		}

		public static bool IsInstalled(Guid packageId)
		{
			return PackageManager.GetInstalledPackages().Any(p => p.Id == packageId);
		}

		public static PackageDescription GetPackageDescription(Guid packageId)
		{
			return PackageServerFacade.GetAllPackageDescriptions(Guid.Empty, CultureInfo.CreateSpecificCulture("en-us")).SingleOrDefault(p => p.Id == packageId);
		}

		private static void LogValidationResults(IEnumerable<PackageFragmentValidationResult> packageFragmentValidationResults)
		{
			foreach (PackageFragmentValidationResult packageFragmentValidationResult in packageFragmentValidationResults)
			{
				Log.LogCritical("SetupServiceFacade", packageFragmentValidationResult.Message);
				throw new InvalidOperationException(packageFragmentValidationResult.Message);
			}
		}
	}
}
