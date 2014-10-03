using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Composite.C1Console.Actions;
using Composite.C1Console.Elements;
using Composite.C1Console.Security;


namespace Composite.Tools.PackageCreator.Types
{
	interface IPackToggle
	{
		ActionCheckedStatus CheckedStatus { get; }

		bool Disabled { get; }

		EntityToken GetEntityToken();
	}
}
