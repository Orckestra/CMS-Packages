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
                ActionToken actionToken;
                if (item is IPackageCreatorItemActionToken)
                    actionToken = new PackageCreatorActionToken(item.CategoryName, (item as IPackageCreatorItemActionToken).ActionTokenName);
                else
                    actionToken = new PackageCreatorActionToken(item.CategoryName);

                yield return new ElementAction(new ActionHandle(actionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = item.ActionLabel,
                        ToolTip = item.ActionToolTip,
                        Icon = item.ActionIcon,
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
