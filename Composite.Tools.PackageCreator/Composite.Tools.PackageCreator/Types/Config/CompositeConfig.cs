using System.Collections.Generic;
using Composite.C1Console.Security;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;

namespace Composite.Tools.PackageCreator.Types
{
    [PackCategory("Composite.config", AliasNames = new[] { "Configuration" })]
    internal class PCCompositeConfig : BasePackItem, IPack
    {
        public const string Source = "Composite.config";
        public const string Path = "~/App_Data/Composite/Composite.config";

        public PCCompositeConfig(string name) : base(name) { }

        public void Pack(PackageCreator creator)
        {
            creator.AddConfigurationXPath(Source, Id);
        }

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
        {
            if (entityToken is XmlNodeAttributeProviderEntityToken token && token.Source == Source)
            {
                yield return new PCCompositeConfig(token.XPath);
            }
        }
    }
}