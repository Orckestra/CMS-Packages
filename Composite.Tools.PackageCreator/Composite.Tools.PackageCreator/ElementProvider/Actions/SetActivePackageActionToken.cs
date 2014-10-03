using System.Collections.Generic;
using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.Plugins.Elements.ElementProviders.PageElementProvider;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;
using Composite.Tools.PackageCreator.Types;

namespace Composite.Tools.PackageCreator.ElementProvider.Actions
{
    [ActionExecutor(typeof(SetActivePackageActionExecutor))]
    public sealed class SetActivePackageActionToken : ActionToken
    {
        static private readonly IEnumerable<PermissionType> _permissionTypes = new PermissionType[] { PermissionType.Administrate };

        public override IEnumerable<PermissionType> PermissionTypes
        {
            get { return _permissionTypes; }
        }

        public override string Serialize()
        {
            return "SetActivePackage";
        }

        public static ActionToken Deserialize(string serializedData)
        {
            return new SetActivePackageActionToken();
        }
    }

    public class SetActivePackageActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            PackageCreatorFacade.ActivePackageName = entityToken.Source;
			Tree.Page.ClearCache();
			Tree.Media.ClearCache();
            var specificTreeRefresher = new SpecificTreeRefresher(flowControllerServicesContainer);
			var parentTreeRefresher = new ParentTreeRefresher(flowControllerServicesContainer);
            specificTreeRefresher.PostRefreshMesseges(new PackageCreatorElementProviderEntityToken());
			specificTreeRefresher.PostRefreshMesseges(new PageElementProviderEntityToken("PageElementProvider"));
			parentTreeRefresher.PostRefreshMesseges(PCMediaFolder.GetRootEntityToken());


            return null;
        }
    }
}
