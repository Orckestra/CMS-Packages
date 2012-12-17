using System.Collections.Generic;
using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.C1Console.Events;
using Composite.Core.ResourceSystem;
using Composite.Core.WebClient;

namespace Composite.Tools.LinkChecker
{
    [ActionExecutor(typeof(LinkCheckerActionExecutor))]
    public sealed class LinkCheckerActionToken : ActionToken
    {
        private static readonly IEnumerable<PermissionType> _permissionType = new[] { PermissionType.Read };

        public override IEnumerable<PermissionType> PermissionTypes
        {
            get { return _permissionType; }
        }

        public override string Serialize()
        {
            return "LinkChecker";
        }

        public static ActionToken Deserialize(string serializedData)
        {
            return new LinkCheckerActionToken();
        }
    }

    internal sealed class LinkCheckerActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            string url = UrlUtils.ResolveAdminUrl("InstalledPackages/content/views/Composite.Tools.LinkChecker/ListBrokenLinks.aspx");

            var consoleServices = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>();
            var openViewMsg = new OpenViewMessageQueueItem
            {
                EntityToken = EntityTokenSerializer.Serialize(entityToken, true),
                ViewId = "LinkChecker",
                Label = GetResourceString("LinkCheckerActionToken.Label"),
                Url = url,
                ViewType = ViewType.Main
            };

            ConsoleMessageQueueFacade.Enqueue(openViewMsg, consoleServices.CurrentConsoleId);

            return null;
        }

        private static string GetResourceString(string key)
        {
            return StringResourceSystemFacade.GetString("Composite.Tools.LinkChecker", key);
        }
    }
}