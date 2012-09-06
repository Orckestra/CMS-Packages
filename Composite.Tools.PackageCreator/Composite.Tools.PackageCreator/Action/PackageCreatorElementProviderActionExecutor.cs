using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.C1Console.Events;
using Composite.Core.WebClient;
using Composite.Core.ResourceSystem;
using System.Web;
using Composite.Plugins.Elements.ElementProviders.GeneratedDataTypesElementProvider;
using Composite.Core.Types;
using Composite.Data;
using System.Xml.Linq;
using Composite.Data.GeneratedTypes;
using Composite.Data.Types;
using Composite.Tools.PackageCreator.ElementProvider;
using Composite.Tools.PackageCreator.Types;


namespace Composite.Tools.PackageCreator
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
						var itemActionToken = item as IPackageCreatorItemActionToken;
						if(itemActionToken != null)
						{
							if (token.Name != itemActionToken.ActionTokenName)
							{
								continue;
							}
						}
						PackageCreatorFacade.AddItem(item, packageName);
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
