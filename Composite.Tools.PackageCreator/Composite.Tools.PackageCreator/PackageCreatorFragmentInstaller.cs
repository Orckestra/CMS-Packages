using Composite.C1Console.Security;
using Composite.Core;
using Composite.Core.PackageSystem.PackageFragmentInstallers;
using Composite.Data;
using Composite.Data.Types;
using Composite.Plugins.Elements.ElementProviders.VirtualElementProvider;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Composite.Tools.PackageCreator
{
    public class PackageCreatorFragmentInstaller : BasePackageFragmentInstaller
    {
        public override IEnumerable<System.Xml.Linq.XElement> Install()
        {
            // grant Perspective permissions to the current user
            string perspectiveName = "Composite.Tools.PackageCreator";
            var entityToken = new VirtualElementProviderEntityToken("VirtualElementProvider", perspectiveName);
            var serializedEntityToken = EntityTokenSerializer.Serialize(entityToken);

            var sitemapPerspective = DataFacade.BuildNew<IUserGroupActivePerspective>();
            var userGroup = DataFacade.GetData<IUserGroup>().FirstOrDefault(u => u.Name == "Administrator");
            if (userGroup != null)
            {
                sitemapPerspective.UserGroupId = userGroup.Id;
                sitemapPerspective.SerializedEntityToken = serializedEntityToken;
                sitemapPerspective.Id = Guid.NewGuid();
                DataFacade.AddNew(sitemapPerspective);
                Log.LogInformation("Composite.Tools.PackageCreator", $"Access to the '{perspectiveName}' granted for user group '{userGroup.Name}'.");
            }

            if (UserValidationFacade.IsLoggedIn())
            {
                var activePerspective = DataFacade.BuildNew<IUserActivePerspective>();
                activePerspective.Username = C1Console.Users.UserSettings.Username;
                activePerspective.SerializedEntityToken = serializedEntityToken;
                activePerspective.Id = Guid.NewGuid();

                DataFacade.AddNew<IUserActivePerspective>(activePerspective);
                Log.LogInformation("Composite.Tools.PackageCreator", $"Access to the '{perspectiveName}' granted for user '{C1Console.Users.UserSettings.Username}'.");

            }
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
