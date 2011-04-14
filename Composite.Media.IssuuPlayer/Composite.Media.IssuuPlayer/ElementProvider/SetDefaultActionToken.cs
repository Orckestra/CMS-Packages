using System.Collections.Generic;
using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.Data;

namespace Composite.Media.IssuuPlayer.ElementProvider
{
	[ActionExecutor(typeof(SetDefaultActionExecutor))]
	public sealed class SetDefaultActionToken : ActionToken
	{
		public override IEnumerable<PermissionType> PermissionTypes
		{
			get
			{
				yield return PermissionType.Edit;
				yield return PermissionType.Administrate;
			}
		}

		public override bool IgnoreEntityTokenLocking
		{
			get
			{
				return false;
			}
		}

		public override string Serialize()
		{
			return "";
		}

		public static ActionToken Deserialize(string serialiedWorkflowActionToken)
		{
			return new SetDefaultActionToken();
		}
	}

	public sealed class SetDefaultActionExecutor : IActionExecutor
	{
		public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
		{
			var token = entityToken as DataEntityToken;
			var apiKey = token.Data as ApiKey;

			IssuuApi.SetDefault(apiKey);

			var treeRefresher = new ParentTreeRefresher(flowControllerServicesContainer);
			treeRefresher.PostRefreshMesseges(token.Data.GetDataEntityToken());

			return null;
		}
	}
}
