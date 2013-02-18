using System.Collections.Generic;
using Composite.C1Console.Actions;
using Composite.C1Console.Security;

namespace Composite.Tools.AzureBlobVerifier
{
	[ActionExecutor(typeof(ValidatorActionExecutor))]
	internal sealed class ValidatorActionToken : ActionToken
	{
		public override IEnumerable<PermissionType> PermissionTypes
		{
			get { return new[] { PermissionType.Administrate }; }
		}


		public static ActionToken Deserialize(string serializedActionToken)
		{
			return new ValidatorActionToken();
		}
	}
}
