using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.Core.ResourceSystem;
using Composite.C1Console.Security;
using Composite.Plugins.Elements.ElementProviders.WebsiteFileElementProvider;
using Composite.Data;
using System.IO;
using Composite.Core.IO;

namespace Composite.Tools.PackageCreator.Types
{
	[PCCategory("Directories")]
	internal class PCDirectory : SimplePackageCreatorItem
	{
		public PCDirectory(string name)
			: base(name)
		{
		}


		public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
		{
			if (entityToken is WebsiteFileElementProviderEntityToken)
			{
				WebsiteFileElementProviderEntityToken token = (WebsiteFileElementProviderEntityToken)entityToken;
				if (Directory.Exists(token.Id))
				{
					yield return new PCDirectory(token.Id.Replace(token.GetProperty("RootPath")+"\\","")+"\\");
					yield break;
				}
			}
		}

	}
}
