using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;

namespace Composite.Tools.PackageCreator.Types
{
    /// <summary>
    /// Represents a package creator item that has only one 'name' attribute in serialized state.
    /// Derived classes have to define a constructor from with a single <see cref="string"/> parameter.
    /// </summary>
    public abstract class SimplePackageCreatorItem : IPackageCreatorItem
    {
       
        protected EntityToken _entityToken;
        protected virtual XNamespace ns
        {
            get
            {
                return PackageCreator.pc;
            }
        }

        protected virtual XName itemName
        {
            get
            {
                return "Add";
            }
        }


        protected SimplePackageCreatorItem() { }

        protected SimplePackageCreatorItem(string name)
        {
            this.Name = name;
        }

        protected SimplePackageCreatorItem(EntityToken entityToken)
        {
            this._entityToken = entityToken;
        }

		public virtual string Name { get; set; }

        public virtual string Id
        {
            get
            {
                return Name;
            }
        }

        public virtual string GetLabel()
        {
            return Name;
        }


        public virtual string ActionLabel
        {
            get { return PackageCreatorFacade.GetLocalization(string.Format("Add.Label")); }
        }

        public virtual string ActionToolTip
        {
            get { return PackageCreatorFacade.GetLocalization(string.Format("Add.ToolTip")); }
        }

        public virtual ResourceHandle ItemIcon
        {
            get { return new ResourceHandle("Composite.Icons", "page-publication"); }
        }

        public virtual ResourceHandle ActionIcon
        {
            get { return new ResourceHandle("Composite.Icons", "package-element-closed-availableitem"); }
        }

        public virtual void AddToConfiguration(XElement config)
        {
            RemoveFromConfiguration(config);
            var category = config.ForceElement(ns + this.CategoryName);
            category.Add(
                new XElement(itemName,
                    new XAttribute(category.IndexAttributeName(), this.Id)
                )
            );

        }
        public virtual void RemoveFromConfiguration(XElement config)
        {
            foreach (var name in this.CategoryAllNames)
            {
                config.Elements(ns + name).Elements(itemName).Where(x => x.IndexAttributeValue() == this.Name).Remove();
            }
        }

        public static IEnumerable<IPackageCreatorItem> GetItems(Type type, XElement config)
        {
            return GetItems(type, config, PackageCreator.pc, "Add");
        }

        public static IEnumerable<IPackageCreatorItem> GetItems(Type type, XElement config, XNamespace ns, XName itemName)
        {
            foreach (var category in type.GetCategoryAllNames())
            {
                foreach (var name in config.Elements(ns + category).Elements(itemName).Select(d => d.IndexAttributeValue()).Where(d => d != null))
                {
                    object item = Activator.CreateInstance(type, new object[] { name });
                    yield return (IPackageCreatorItem)item;
                }
            }
        }


        public string CategoryName
        {
            get { return this.GetCategoryName2(); }
        }

        public string[] CategoryAllNames
        {
            get { return this.GetCategoryAllNames2(); }
        }

    }
}
