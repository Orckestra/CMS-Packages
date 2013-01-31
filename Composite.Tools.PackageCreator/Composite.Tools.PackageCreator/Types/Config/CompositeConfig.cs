using System.Collections.Generic;
using Composite.C1Console.Security;
using Composite.Tools.PackageCreator.ElementProvider;

namespace Composite.Tools.PackageCreator.Types
{
	[PCCategory("Composite.config", AliasNames = new[] {"Configuration"})]
	internal class PCCompositeConfig : SimplePackageCreatorItem, IPackageable
	{
		public const string Source = "Composite.config";
		public const string Path = "~/App_Data/Composite/Composite.config"; 

		public PCCompositeConfig(string name)
			: base(name)
		{
		}

		public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
		{
			if (entityToken is XmlNodeAttributeProviderEntityToken && entityToken.Source == Source)
			{
				XmlNodeAttributeProviderEntityToken token = (XmlNodeAttributeProviderEntityToken)entityToken;
				yield return new PCCompositeConfig(token.XPath);
			};
		}

		public void Pack(PackageCreator creator)
		{
			creator.AddConfigurationXPath(PCCompositeConfig.Source, this.Name);
		}
	}
}
