using System.Collections.Generic;
using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;

namespace Composite.Tools.PackageCreator.ElementProvider.Actions
{
    [ActionExecutor(typeof(IncludePageDataPackageCreatorElementProviderActionExecutor))]
    public sealed class IncludePageDataPackageCreatorActionToken : ActionToken
    {
        private static readonly IEnumerable<PermissionType> _permissionTypes = new[] { PermissionType.Administrate };

        public override IEnumerable<PermissionType> PermissionTypes => _permissionTypes;

        public override string Serialize()
        {
            return "IncludePageDataPackageCreator";
        }

        public static ActionToken Deserialize(string serializedData)
        {
            return new IncludePageDataPackageCreatorActionToken();
        }
    }

    public class IncludePageDataPackageCreatorElementProviderActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken,
            FlowControllerServicesContainer flowControllerServicesContainer)
        {
            PackageCreatorFacade.ToggleIncludePageData(entityToken.Source);

            var treeRefresher = new SpecificTreeRefresher(flowControllerServicesContainer);
            treeRefresher.PostRefreshMesseges(new PackageCreatorElementProviderEntityToken());

            return null;
        }
    }
}