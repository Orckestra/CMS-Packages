using System.Collections.Generic;
using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.Tools.PackageCreator.Actions;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;
using Composite.Tools.PackageCreator.Types;

namespace Composite.Tools.PackageCreator.ElementProvider.Actions
{
    [ActionExecutor(typeof(OverwriteOnInstallPackageCreatorElementProviderActionExecutor))]
    public sealed class OverwriteOnInstallPackageCreatorActionToken : ActionToken
    {
        static private IEnumerable<PermissionType> _permissionTypes = new PermissionType[] { PermissionType.Administrate };

        public override IEnumerable<PermissionType> PermissionTypes
        {
            get { return _permissionTypes; }
        }

        public override string Serialize()
        {
            return "OverwriteOnInstallPackageCreator";
        }

        public static ActionToken Deserialize(string serializedData)
        {
            return new OverwriteOnInstallPackageCreatorActionToken();
        }
    }

    public class OverwriteOnInstallPackageCreatorElementProviderActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
			PackageCreatorFacade.ToggleAllowOverwrite(entityToken.Source);

           SpecificTreeRefresher treeRefresher = new SpecificTreeRefresher(flowControllerServicesContainer);
           treeRefresher.PostRefreshMesseges(new PackageCreatorElementProviderEntityToken());

            return null;
        }
    }
}
