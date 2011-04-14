using System.Collections.Generic;
using Composite.C1Console.Actions;
using Composite.C1Console.Events;
using Composite.C1Console.Security;
using Composite.Core.WebClient;

namespace Composite.Forms.FormSubmitHandler
{
	[ActionExecutor(typeof(HandleElementProviderActionExecutor))]
	public class HandleActionToken : ActionToken
	{
		static private IEnumerable<PermissionType> _permissionTypes = new PermissionType[] { PermissionType.Administrate };

		public override IEnumerable<PermissionType> PermissionTypes
		{
			get { return _permissionTypes; }
		}

		public override bool IgnoreEntityTokenLocking { get { return true; } }

		public static ActionToken Deserialize(string serializedData)
		{
			return new HandleActionToken();
		}
	}

	class HandleElementProviderActionExecutor : IActionExecutor
	{
		public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
		{
			var currentConsoleId = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>().CurrentConsoleId;
			var packageName = entityToken.Source;

			string url = UrlUtils.ResolveAdminUrl(
				string.Format(@"InstalledPackages/content/views/Composite.Forms.FormSubmitHandler/GetData.aspx?entityToken={0}", EntityTokenSerializer.Serialize(entityToken))
			);

			ConsoleMessageQueueFacade.Enqueue(new DownloadFileMessageQueueItem(url), currentConsoleId);
			return null;
		}
	}
}