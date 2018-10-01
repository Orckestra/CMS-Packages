using System;
using System.Collections.Generic;
using Composite.C1Console.Security;
using Composite.Core.Logging;
using Composite.Core.PackageSystem.PackageFragmentInstallers;
using Composite.Data;
using Composite.Data.Types;
using System.Linq;
using Composite.C1Console.Trees.Foundation;
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


            IUserGroupActivePerspective sitemapPerspective = DataFacade.BuildNew<IUserGroupActivePerspective>();
            var userGroup = DataFacade.GetData<IUserGroup>().FirstOrDefault(u => u.Name == "Administrator");
            if (userGroup != null)
            {
                sitemapPerspective.UserGroupId = userGroup.Id;
                sitemapPerspective.SerializedEntityToken = EntityTokenSerializer.Serialize(entityToken);
                sitemapPerspective.Id = Guid.NewGuid();
                DataFacade.AddNew(sitemapPerspective);
                LoggingService.LogInformation("Composite.Tools.PackageCreator", string.Format("Access to the {0} granted for group {1}.", perspectiveName, userGroup.Name));
            }

            if (UserValidationFacade.IsLoggedIn())
            {
                IUserActivePerspective activePerspective = DataFacade.BuildNew<IUserActivePerspective>();
                activePerspective.Username = C1Console.Users.UserSettings.Username;
                activePerspective.SerializedEntityToken = EntityTokenSerializer.Serialize(entityToken);
                activePerspective.Id = Guid.NewGuid();

                DataFacade.AddNew<IUserActivePerspective>(activePerspective);
                LoggingService.LogInformation("Composite.Tools.PackageCreator", String.Format("Access to the {0} granted for the {1}.", perspectiveName, C1Console.Users.UserSettings.Username));

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
