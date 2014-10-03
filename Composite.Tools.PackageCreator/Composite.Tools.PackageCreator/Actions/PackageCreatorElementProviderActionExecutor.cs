using Composite.C1Console.Actions;
using Composite.C1Console.Elements;
using Composite.C1Console.Events;
using Composite.C1Console.Security;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens.Interfaces;
using Composite.Tools.PackageCreator.Types;

namespace Composite.Tools.PackageCreator.Actions
{
    class PackageCreatorElementProviderActionExecutor : IActionExecutor
    {
        #region IActionExecutor Members

        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var packageName = PackageCreatorFacade.ActivePackageName;
	        
            if (entityToken is PackageCreatorEntityToken)
            {
                packageName = (entityToken as PackageCreatorEntityToken).Source;
            }
            if (string.IsNullOrEmpty(packageName))
            {
                flowControllerServicesContainer.GetService<IManagementConsoleMessageService>();
                IManagementConsoleMessageService consoleServices = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>();
                consoleServices.ShowMessage(DialogType.Warning, PackageCreatorFacade.GetLocalization("NoPackages.Title"), PackageCreatorFacade.GetLocalization("NoPackages.Message"));
                return null;
            }
            if (actionToken is PackageCreatorActionToken)
            {
                var token = actionToken as PackageCreatorActionToken;

                foreach (var item in PackageCreatorActionFacade.GetPackageCreatorItems(entityToken))
                {
                    if (item.CategoryName == token.CategoryName)
                    {
                        //if diffent item for one category and entitytoken
                        var itemActionToken = item as IPackItemActionToken;
                        if (itemActionToken != null)
                        {
                            if (token.Name != itemActionToken.ActionTokenName)
                            {
                                continue;
                            }
                        }
	                    if (item is IPackToggle)
	                    {
		                    if ((item as IPackToggle).CheckedStatus == ActionCheckedStatus.Checked)
		                    {
								 PackageCreatorFacade.RemoveItem(item, packageName);
		                    }
		                    else
		                    {
								PackageCreatorFacade.AddItem(item, packageName);
							}
							var parentTreeRefresher = new ParentTreeRefresher(flowControllerServicesContainer);
							parentTreeRefresher.PostRefreshMesseges(entityToken);
	                    }
	                    else
	                    {
		                    PackageCreatorFacade.AddItem(item, packageName);
	                    }
	                    break;
                    }
                }

            }

            SpecificTreeRefresher treeRefresher = new SpecificTreeRefresher(flowControllerServicesContainer);
            treeRefresher.PostRefreshMesseges(new PackageCreatorElementProviderEntityToken());

            return null;

        }
        #endregion

    }
}
