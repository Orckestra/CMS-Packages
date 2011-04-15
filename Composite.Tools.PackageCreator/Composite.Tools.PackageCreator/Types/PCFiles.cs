using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.Plugins.Elements.ElementProviders.WebsiteFileElementProvider;
using Composite.C1Console.Security;
using System.IO;

namespace Composite.Tools.PackageCreator.Types
{
	[PCCategory("Files")]
	internal class PCFile : SimplePackageCreatorItem
	{
		public PCFile(string name)
			: base(name)
		{
		}

		public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
		{
			if (entityToken is WebsiteFileElementProviderEntityToken)
			{
				WebsiteFileElementProviderEntityToken token = (WebsiteFileElementProviderEntityToken)entityToken;
				if (File.Exists(token.Id))
				{
					yield return new PCFile(token.Id.Replace(token.GetProperty("RootPath") + "\\", ""));
				}
			};
		}

	}
}
