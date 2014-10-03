using System.Collections.Generic;
using System.Text.RegularExpressions;
using Composite.C1Console.Security;
using Composite.Core.IO;
using Composite.Plugins.Elements.ElementProviders.WebsiteFileElementProvider;

namespace Composite.Tools.PackageCreator.Types
{
    [PackCategory("Files")]
    internal class PCFile : BasePackItem
    {
        public PCFile(string name)
            : base(name)
        {
        }

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
        {
            if (entityToken is WebsiteFileElementProviderEntityToken)
            {
                WebsiteFileElementProviderEntityToken token = (WebsiteFileElementProviderEntityToken)entityToken;
                if (C1File.Exists(token.Path))
                {
                    yield return new PCFile(Regex.Replace(token.Id, @"^\\", ""));
                }
            };
        }

    }
}
