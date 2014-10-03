using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Core.Types;

namespace Composite.Tools.PackageCreator.Types
{
    [PackCategory("Localizations")]
    internal sealed class PCLocalizations : BasePackItem, IPack
    {
        private static readonly XNamespace xsl = "http://www.w3.org/1999/XSL/Transform";

        internal Type XmlStringResourceProviderType
        {
            get
            {
                return TypeManager.TryGetType("Composite.Plugins.ResourceSystem.XmlStringResourceProvider.XmlStringResourceProvider, Composite");
            }
        }

        public PCLocalizations(string name)
            : base(name)
        {
        }

        public override ResourceHandle ItemIcon
        {
            get
            {
                return new ResourceHandle("Composite.Icons", "localization-element-localeitem");
            }
        }

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
        {
            yield break;
        }

        public void Pack(PackageCreator creator)
        {
            creator.AddDirectory(string.Format(@"App_Data\Composite\LanguagePacks\{0}\", this.Id));
        }
    }
}
