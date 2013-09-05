using System.Collections.Generic;
using System.Text.RegularExpressions;
using Composite.C1Console.Security;
using Composite.Core.IO;
using Composite.Plugins.Elements.ElementProviders.WebsiteFileElementProvider;

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
                if (C1Directory.Exists(token.Path))
                {
                    yield return new PCDirectory(Regex.Replace(token.Id, @"^\\", "") + "\\");
                    yield break;
                }
            }
        }

    }
}
