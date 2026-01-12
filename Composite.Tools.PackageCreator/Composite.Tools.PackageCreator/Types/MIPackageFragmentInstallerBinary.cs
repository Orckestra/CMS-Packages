using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Composite.C1Console.Security;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;

namespace Composite.Tools.PackageCreator.Types
{
    [PackCategory("PackageFragmentInstallerBinaries")]
    public class MIPackageFragmentInstallerBinary : BasePackItem
    {
        public MIPackageFragmentInstallerBinary(string name)
            : base(name)
        {
        }

        public override string ActionLabel => PackageCreatorFacade.GetLocalization($"{CategoryName}.Add.Label");

        public override string ActionToolTip => PackageCreatorFacade.GetLocalization($"{CategoryName}.Add.ToolTip");

        protected override XNamespace ns => PackageCreator.NsMi;

        protected override XName itemName => ns + "Add";

        public override void AddToConfiguration(XElement config)
        {
            var category = config.ForceElement(ns + CategoryName);
            if (category.Elements(itemName).All(x => x.AttributeValue("path") != Id))
                category.Add(
                    new XElement(itemName,
                        new XAttribute("path", Id)
                    )
                );
        }

        public new static IEnumerable<IPackItem> GetItems(Type type, XElement config)
        {
            return GetItems(type, config, PackageCreator.NsMi, PackageCreator.NsMi + "Add");
        }

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
        {
            if (!(entityToken is PackageCreatorItemElementProviderEntityToken token))
                yield break;

            if (token.Type != typeof(PCFile).GetCategoryName())
                yield break;

            if (Path.GetExtension(token.Id).ToLower() != ".dll")
                yield break;

            yield return new MIPackageFragmentInstallerBinary($"~\\{token.Id}");
        }
    }
}