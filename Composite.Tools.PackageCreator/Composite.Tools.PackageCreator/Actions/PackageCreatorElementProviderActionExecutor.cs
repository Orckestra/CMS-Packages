using Composite.C1Console.Actions;
using Composite.C1Console.Elements;
using Composite.C1Console.Events;
using Composite.C1Console.Security;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens.Interfaces;
using Composite.Tools.PackageCreator.Types;

namespace Composite.Tools.PackageCreator.Actions
{
    internal class PackageCreatorElementProviderActionExecutor : IActionExecutor
    {
        #region IActionExecutor Members

        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken,
            FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var packageName = PackageCreatorFacade.ActivePackageName;

            if (entityToken is PackageCreatorEntityToken creatorEntityToken) packageName = creatorEntityToken.Source;
            if (string.IsNullOrEmpty(packageName))
            {
                flowControllerServicesContainer.GetService<IManagementConsoleMessageService>();
                var consoleServices = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>();
                consoleServices.ShowMessage(DialogType.Warning,
                    PackageCreatorFacade.GetLocalization("NoPackages.Title"),
                    PackageCreatorFacade.GetLocalization("NoPackages.Message"));
                return null;
            }

            if (actionToken is PackageCreatorActionToken token)
                foreach (var item in PackageCreatorActionFacade.GetPackageCreatorItems(entityToken))
                    if (item.CategoryName == token.CategoryName)
                    {
                        //if different item for one category and entityToken
                        if (item is IPackItemActionToken itemActionToken)
                            if (token.Name != itemActionToken.ActionTokenName)
                                continue;

                        if (item is IPackToggle toggle)
                        {
                            if (toggle.CheckedStatus == ActionCheckedStatus.Checked)
                                PackageCreatorFacade.RemoveItem(item, packageName);
                            else
                                PackageCreatorFacade.AddItem(item, packageName);
                            var parentTreeRefresher = new ParentTreeRefresher(flowControllerServicesContainer);
                            parentTreeRefresher.PostRefreshMesseges(entityToken);
                        }
                        else
                        {
                            PackageCreatorFacade.AddItem(item, packageName);
                        }

                        break;
                    }

            var treeRefresher = new SpecificTreeRefresher(flowControllerServicesContainer);
            treeRefresher.PostRefreshMesseges(new PackageCreatorElementProviderEntityToken());

            return null;
        }

        #endregion
    }
}