using System.Collections.Generic;
using Composite.C1Console.Elements.Plugins.ElementActionProvider;
using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Tools.PackageCreator.Types;

namespace Composite.Tools.PackageCreator.Actions
{
    public class PackageCreatorElementActionProvider : IElementActionProvider
    {
        public IEnumerable<ElementAction> GetActions(EntityToken entityToken)
        {
            if (!PackageCreatorFacade.IsHaveAccess)
                yield break;

            foreach (var item in PackageCreatorActionFacade.GetPackageCreatorItems(entityToken))
            {
	            var name = string.Empty;
	            var disabled = false;
	            var checkStatus = ActionCheckedStatus.Uncheckable;
				
				if (item is IPackItemActionToken)
                   name = (item as IPackItemActionToken).ActionTokenName;

	            if (item is IPackToggle)
	            {
		            disabled = (item as IPackToggle).Disabled;
		            checkStatus = (item as IPackToggle).CheckedStatus;
	            }


	            var  actionToken = new PackageCreatorActionToken(item.CategoryName, name);

                yield return new ElementAction(new ActionHandle(actionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = item.ActionLabel,
                        ToolTip = item.ActionToolTip,
                        Icon = item.ActionIcon,
						Disabled = disabled,
						ActionCheckedStatus = checkStatus,
                        ActionLocation = new ActionLocation
                        {
                            ActionType = PackageCreatorFacade.ActionType,
                            IsInFolder = false,
                            IsInToolbar = false,
                            ActionGroup = new ActionGroup("Develop", ActionGroupPriority.PrimaryLow)
                        }
                    }
                };
            }
        }
    }
}
