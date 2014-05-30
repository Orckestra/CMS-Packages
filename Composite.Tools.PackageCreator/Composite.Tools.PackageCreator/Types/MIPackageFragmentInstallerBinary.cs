using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Composite.C1Console.Security;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;

namespace Composite.Tools.PackageCreator.Types
{
    [PCCategory("PackageFragmentInstallerBinaries")]
    public class MIPackageFragmentInstallerBinary : SimplePackageCreatorItem
    {
        public MIPackageFragmentInstallerBinary(string name)
            : base(name)
        {
        }

        public override string ActionLabel
        {
            get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.Label", this.CategoryName)); }
        }

        public override string ActionToolTip
        {
            get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.ToolTip", this.CategoryName)); }
        }

        protected override XNamespace ns
        {
            get
            {
                return PackageCreator.mi;
            }
        }

        protected override XName itemName
        {
            get
            {
                return ns + "Add";
            }
        }

        public override void AddToConfiguration(XElement config)
        {
            var category = config.ForceElement(ns + this.CategoryName);
            if (category.Elements(itemName).Where(x => x.AttributeValue("path") == this.Id).Count() == 0)
            {
                category.Add(
                    new XElement(itemName,
                        new XAttribute("path", this.Id)
                    )
                );
            }
        }

        public static new IEnumerable<IPackageCreatorItem> GetItems(Type type, XElement config)
        {
            return GetItems(type, config, PackageCreator.mi, PackageCreator.mi + "Add");
        }

        public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
        {
            if (entityToken is PackageCreatorItemElementProviderEntityToken)
            {
                var token = (PackageCreatorItemElementProviderEntityToken)entityToken;

                if (token.Type == typeof(PCFile).GetCategoryName())
                {
                    if (Path.GetExtension(token.Id).ToLower() == ".dll")
                    {
                        yield return new MIPackageFragmentInstallerBinary(string.Format("~\\{0}", token.Id));
                        yield break;
                    }
                }
            }
        }
    }
}
