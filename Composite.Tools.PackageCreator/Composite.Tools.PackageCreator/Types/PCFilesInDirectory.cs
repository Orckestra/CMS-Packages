using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.C1Console.Security;

namespace Composite.Tools.PackageCreator.Types
{

	[PCCategory("FilesInDirectories")]
	internal class PCFilesInDirectory : SimplePackageCreatorItem
	{
		public PCFilesInDirectory(string name)
			: base(name)
		{
		}

		public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
		{
			yield break;
		}

	}
}
