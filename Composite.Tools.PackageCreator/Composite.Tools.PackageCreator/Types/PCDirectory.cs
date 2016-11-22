using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Composite.C1Console.Security;
using Composite.Core.IO;
using Composite.Plugins.Elements.ElementProviders.WebsiteFileElementProvider;

namespace Composite.Tools.PackageCreator.Types
{
    [PackCategory("Directories")]
    internal class PCDirectory : BasePackItem
    {
        public PCDirectory(string name)
            : base(name)
        {
        }

        public PCDirectory(XElement element): base(element)
        {
        }

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
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
