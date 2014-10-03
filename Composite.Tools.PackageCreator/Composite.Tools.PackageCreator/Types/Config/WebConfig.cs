using System.Collections.Generic;
using Composite.C1Console.Security;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;

namespace Composite.Tools.PackageCreator.Types
{
    [PackCategory("Web.config", AliasNames = new[] { "Configuration" })]
    internal class PCWebConfig : BasePackItem, IPack
    {
        public const string Source = "Web.config";
        public const string Path = "~/Web.config";

        public PCWebConfig(string name)
            : base(name)
        {
        }

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
        {
            if (entityToken is XmlNodeAttributeProviderEntityToken && entityToken.Source == Source)
            {
                XmlNodeAttributeProviderEntityToken token = (XmlNodeAttributeProviderEntityToken)entityToken;
                yield return new PCWebConfig(token.XPath);
            };
        }


        public void Pack(PackageCreator creator)
        {
            creator.AddConfigurationXPath(PCWebConfig.Source, this.Id);
        }
    }
}
