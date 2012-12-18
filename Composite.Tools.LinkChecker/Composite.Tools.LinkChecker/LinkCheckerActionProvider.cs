using System.Collections.Generic;
using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementActionProvider;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Core.ResourceSystem.Icons;
using Composite.Plugins.Elements.ElementProviders.PageElementProvider;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using ActionType = Composite.C1Console.Elements.ActionType;

namespace Composite.Tools.LinkChecker
{
    /// <summary>
    /// Adds "Link Checker" action to the page root node
    /// </summary>
    [ConfigurationElementType(typeof(NonConfigurableElementActionProvider))]
    public class LinkCheckerActionProvider : IElementActionProvider
    {

        private static readonly List<ElementAction> EmptyActionList = new List<ElementAction>();
        private static readonly ActionGroup ViewActionGroup = new ActionGroup("View", ActionGroupPriority.PrimaryLow);
        public static ResourceHandle LinkIcon = GetIconHandle("link");

        public IEnumerable<ElementAction> GetActions(EntityToken entityToken)
        {
            if (entityToken == null || !(entityToken is PageElementProviderEntityToken))
            {
                return EmptyActionList;
            }
            
            return new[] {new ElementAction(new ActionHandle(new LinkCheckerActionToken()))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = GetResourceString("LinkCheckerActionToken.Label"),
                    ToolTip = GetResourceString("LinkCheckerActionToken.ToolTip"),
                    Icon = LinkIcon,
                    Disabled = false,
                    ActionLocation = new ActionLocation
                    {
                        ActionType = ActionType.Other,
                        IsInFolder = false,
                        IsInToolbar = true,
                        ActionGroup = ViewActionGroup
                    }
                }
            }};
        }

        private static ResourceHandle GetIconHandle(string name)
        {
            return new ResourceHandle(BuildInIconProviderName.ProviderName, name);
        }

        private static string GetResourceString(string key)
        {
            return StringResourceSystemFacade.GetString("Composite.Tools.LinkChecker", key);
        }
    }
}