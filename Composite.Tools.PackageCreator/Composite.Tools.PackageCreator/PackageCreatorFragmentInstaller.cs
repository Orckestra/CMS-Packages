using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Composite.C1Console.Security;
using Composite.C1Console.Users;
using Composite.Core;
using Composite.Core.PackageSystem;
using Composite.Core.PackageSystem.PackageFragmentInstallers;
using Composite.Data;
using Composite.Data.Types;
using Composite.Plugins.Elements.ElementProviders.VirtualElementProvider;

namespace Composite.Tools.PackageCreator
{
    public class PackageCreatorFragmentInstaller : BasePackageFragmentInstaller
    {
        public override IEnumerable<XElement> Install()
        {
            // grant Perspective permissions to the current user
            var perspectiveName = "Composite.Tools.PackageCreator";
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
                Log.LogInformation("Composite.Tools.PackageCreator",
                    $"Access to the '{perspectiveName}' granted for user group '{userGroup.Name}'.");
            }

            if (UserValidationFacade.IsLoggedIn())
            {
                var activePerspective = DataFacade.BuildNew<IUserActivePerspective>();
                activePerspective.Username = UserSettings.Username;
                activePerspective.SerializedEntityToken = serializedEntityToken;
                activePerspective.Id = Guid.NewGuid();

                DataFacade.AddNew<IUserActivePerspective>(activePerspective);
                Log.LogInformation("Composite.Tools.PackageCreator",
                    $"Access to the '{perspectiveName}' granted for user '{UserSettings.Username}'.");
            }

            yield break;
        }

        public override IEnumerable<PackageFragmentValidationResult> Validate()
        {
            yield break;
        }
    }

    public class PackageCreatorFragmentUninstaller : BasePackageFragmentUninstaller
    {
        public override void Uninstall()
        {
        }

        public override IEnumerable<PackageFragmentValidationResult> Validate()
        {
            yield break;
        }
    }
}