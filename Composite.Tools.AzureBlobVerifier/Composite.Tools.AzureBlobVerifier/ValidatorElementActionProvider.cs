using System.Collections.Generic;
using System.IO;
using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementActionProvider;
using Composite.C1Console.Security;
using Composite.Core.IO;
using Composite.Core.ResourceSystem;
using Composite.Core.ResourceSystem.Icons;
using Composite.Plugins.Elements.ElementProviders.WebsiteFileElementProvider;


namespace Composite.Tools.AzureBlobVerifier
{
    internal class ValidatorElementActionProvider : IElementActionProvider
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup("Azure tasks", ActionGroupPriority.GeneralAppendMedium);


        public IEnumerable<ElementAction> GetActions(EntityToken entityToken)
        {
            if ((entityToken.GetType() != typeof(WebsiteFileElementProviderRootEntityToken)) &&
                (entityToken.GetType() != typeof(WebsiteFileElementProviderEntityToken)))
            {
                yield break;
            }

            if (!Directory.Exists(Path.Combine(PathUtil.BaseDirectory, entityToken.Id))) yield break;

            ElementAction action = new ElementAction(new ActionHandle(new ValidatorActionToken()));
            action.VisualData = new ActionVisualizedData
            {
                //Label = StringResourceSystemFacade.GetString("Composite.Plugins.PageElementProvider", "PageElementProvider.AddPageAtRoot"),
                //ToolTip = StringResourceSystemFacade.GetString("Composite.Plugins.PageElementProvider", "PageElementProvider.AddPageAtRootToolTip"),
                Label = "Validate blob files",
                ToolTip = "Validate blob files",
                Icon = GetIconHandle("advanced"),
                Disabled = false,
                ActionLocation = new ActionLocation
                    {
                        ActionType = ActionType.Add,
                        IsInFolder = false,
                        IsInToolbar = false,
                        ActionGroup = ActionGroup
                    }
            };

            yield return action;
        }



        private static ResourceHandle GetIconHandle(string name)
        {
            return new ResourceHandle(BuildInIconProviderName.ProviderName, name);
        }
    }
}
