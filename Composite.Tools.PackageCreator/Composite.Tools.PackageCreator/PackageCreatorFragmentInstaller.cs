using System;
using System.Collections.Generic;
using Composite.Data;
using Composite.Data.Types;
using Composite.Core.Logging;
using Composite.Core.PackageSystem.PackageFragmentInstallers;
using Composite.C1Console.Security;
using Composite.Plugins.Elements.ElementProviders.VirtualElementProvider;

namespace Composite.Tools.PackageCreator
{
	public class PackageCreatorFragmentInstaller : BasePackageFragmentInstaller
	{
		public override IEnumerable<System.Xml.Linq.XElement> Install()
		{
			// grant Perspective permissions to the current user
			string perspectiveName = "Composite.Tools.PackageCreator";
			EntityToken entityToken = new VirtualElementProviderEntityToken("VirtualElementProvider", perspectiveName);

			IUserActivePerspective activePerspective = DataFacade.BuildNew<IUserActivePerspective>();
			string Username = Composite.C1Console.Users.UserSettings.Username;
			activePerspective.Username = Username;
			activePerspective.SerializedEntityToken = EntityTokenSerializer.Serialize(entityToken);
			activePerspective.Id = Guid.NewGuid();

			DataFacade.AddNew<IUserActivePerspective>(activePerspective);
			LoggingService.LogInformation("Composite.Tools.PackageCreator", String.Format("Access to the {0} granted for the {1}.", perspectiveName, Username));

			yield break;
		}

		public override IEnumerable<Composite.Core.PackageSystem.PackageFragmentValidationResult> Validate()
		{
			yield break;
		}
	}

	public class PackageCreatorFragmentUninstaller : BasePackageFragmentUninstaller
	{
		public override void Uninstall()
		{
			return;
		}

		public override IEnumerable<Composite.Core.PackageSystem.PackageFragmentValidationResult> Validate()
		{
			yield break;
		}
	}

}
