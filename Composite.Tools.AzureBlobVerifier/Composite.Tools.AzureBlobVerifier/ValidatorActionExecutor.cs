using System;
using System.Web;
using Composite.C1Console.Actions;
using Composite.C1Console.Events;
using Composite.C1Console.Security;
using Composite.Core.IO;
using Composite.Core.ResourceSystem;
using Composite.Core.ResourceSystem.Icons;
using Composite.Core.WebClient;


namespace Composite.Tools.AzureBlobVerifier
{
    internal sealed class ValidatorActionExecutor : IActionExecutor
    {
        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            string currentConsoleId = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>().CurrentConsoleId;

            string path = ExtractPath(entityToken);

            string url = string.Format("{0}?Path={1}", UrlUtils.ResolveAdminUrl("content/views/azureblobverifier/AzureBlobVerifier.aspx"), HttpUtility.UrlEncode(path));

            ConsoleMessageQueueFacade.Enqueue(
                new OpenViewMessageQueueItem
                {
                    Url = url,
                    ViewId = Guid.NewGuid().ToString(),
                    ViewType = ViewType.Main,
                    Label = "Azure Blob Verifier Result",
                    IconResourceHandle = GetIconHandle("advanced")
                }, currentConsoleId);

            return null;
        }



        private static string ExtractPath(EntityToken entityToken)
        {
            string path = entityToken.Id;
            if (path.Length > 0)
            {
                path = "/" + path.Remove(0, PathUtil.BaseDirectory.Length).Replace('\\', '/');
            }
            else
            {
                path = "/";
            }
            return path;
        }



        private static ResourceHandle GetIconHandle(string name)
        {
            return new ResourceHandle(BuildInIconProviderName.ProviderName, name);
        }
    }
}
