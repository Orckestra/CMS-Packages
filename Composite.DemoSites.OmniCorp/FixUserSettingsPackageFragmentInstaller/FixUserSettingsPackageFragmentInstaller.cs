using System.Collections.Generic;
using System.Linq;
using Composite.Core.PackageSystem.PackageFragmentInstallers;
using Composite.Core.PackageSystem;
using Composite.Data;
using Composite.Data.Types;
using System.Xml.Linq;

namespace PackageFragmentInstallers
{
	public class FixUserSettingsPackageFragmentInstaller : BasePackageFragmentInstaller
	{
		public override IEnumerable<XElement> Install()
		{
			var usersNames = new[] { "developer", "advuser", "enduser" };
			using (var conn = new DataConnection())
			{
				var userSettings = conn.Get<IUserSettings>().Where(us => usersNames.Contains(us.Username));
				var culture = DataConnection.AllLocales.First();
				foreach (var setting in userSettings)
				{
					setting.CultureName = culture.Name;
					setting.C1ConsoleUiLanguage = culture.Name;
					setting.CurrentActiveLocaleCultureName = culture.Name;
					setting.ForeignLocaleCultureName = culture.Name;
				}
				conn.Update<IUserSettings>(userSettings);
			}
			yield return new XElement("FixUserSettings", usersNames);
		}

		public override IEnumerable<PackageFragmentValidationResult> Validate()
		{
			var validationResult = new List<PackageFragmentValidationResult>();
			return validationResult;
		}
	}
}
