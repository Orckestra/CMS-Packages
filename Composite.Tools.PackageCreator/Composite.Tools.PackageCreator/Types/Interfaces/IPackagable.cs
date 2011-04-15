using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Composite.Tools.PackageCreator.Types
{
	public interface IPackagable
	{
		void Pack(PackageCreator creator);
	}
}
