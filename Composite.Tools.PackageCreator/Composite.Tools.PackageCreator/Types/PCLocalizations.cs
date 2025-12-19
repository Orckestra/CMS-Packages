using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Core.Types;

namespace Composite.Tools.PackageCreator.Types
{
    [PackCategory("Localizations")]
    internal sealed class PCLocalizations : BasePackItem, IPack
    {
        private static readonly XNamespace xsl = "http://www.w3.org/1999/XSL/Transform";

        public PCLocalizations(string name) : base(name)
        {
        }

        internal Type XmlStringResourceProviderType => TypeManager.TryGetType(
            "Composite.Plugins.ResourceSystem.XmlStringResourceProvider.XmlStringResourceProvider, Composite");

        public override ResourceHandle ItemIcon =>
            new ResourceHandle("Composite.Icons", "localization-element-localeitem");

        public void Pack(PackageCreator creator)
        {
            creator.AddDirectory($@"App_Data\Composite\LanguagePacks\{Id}\");
        }

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
        {
            yield break;
        }
    }
}