using Composite.C1Console.Actions;
using Composite.C1Console.Events;
using Composite.C1Console.Security;
using Composite.Core.Extensions;
using Composite.Plugins.Elements.ElementProviders.GeneratedDataTypesElementProvider;

namespace Composite.Tools.StaticDataTypeCreator
{
	public sealed class StaticDataTypeCreatorActionExecutor : IActionExecutor
	{
		private static readonly string DownloadUrl = "/Composite/InstalledPackages/content/views/Composite.Tools.StaticDataTypeCreator/StaticDataTypeCreator.ashx?typeName={0}";

		public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
		{
			var generatedDataTypetoken = entityToken as GeneratedDataTypesElementProviderTypeEntityToken;
			string typeName = generatedDataTypetoken.SerializedTypeName;
			string currentConsoleId = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>().CurrentConsoleId;
			string url = DownloadUrl.FormatWith(typeName);

			ConsoleMessageQueueFacade.Enqueue(new DownloadFileMessageQueueItem(url), currentConsoleId);

			return null;
		}
	}
}