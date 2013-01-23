using System.Collections.Generic;
using Composite.C1Console.Actions;
using Composite.C1Console.Security;

namespace Composite.Tools.StaticDataTypeCreator
{
	[ActionExecutor(typeof(StaticDataTypeCreatorActionExecutor))]
	public sealed class StaticDataTypeCreatorActionToken : ActionToken
	{
		static private readonly IEnumerable<PermissionType> _permissionTypes = new[] { PermissionType.Read };

		public override IEnumerable<PermissionType> PermissionTypes
		{
			get { return _permissionTypes; }
		}

		public static ActionToken Deserialize(string serializedEntityToken)
		{
			return new StaticDataTypeCreatorActionToken();
		}
	}
}
