using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.Plugins.Elements.ElementProviders.WebsiteFileElementProvider;
using Composite.C1Console.Security;
using System.IO;
using Composite.Tools.PackageCreator.ElementProvider;

namespace Composite.Tools.PackageCreator.Types
{
	[PCCategory("Configuration")]
	internal class PCConfiguration : SimplePackageCreatorItem
	{
		public PCConfiguration(string name)
			: base(name)
		{
		}

		public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
		{
			if (entityToken is XmlNodeAttributeProviderEntityToken)
			{
				XmlNodeAttributeProviderEntityToken token = (XmlNodeAttributeProviderEntityToken)entityToken;
				yield return new PCConfiguration(token.XPath);
				
			};
		}

	}
}
