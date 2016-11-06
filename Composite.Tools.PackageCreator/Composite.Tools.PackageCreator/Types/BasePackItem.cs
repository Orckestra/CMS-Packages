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
    public abstract class BasePackItem : IPackItem
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


        protected BasePackItem() { }

        protected BasePackItem(string name)
        {
            this.Name = name;
        }

        protected BasePackItem(XElement element)
        {
            this.Name = element.IndexAttributeValue();

            var isOverwritable = this as IPackOverwriteItem;
            if (isOverwritable != null)
            {
                isOverwritable.AllowOverwrite = element.AllowOverwriteAttributeValue();
            }
        }

        protected BasePackItem(EntityToken entityToken)
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
                    new XAttribute(category.IndexAttributeName(), Id)
                )
            );

        }
        public virtual void RemoveFromConfiguration(XElement config)
        {
            foreach (var name in this.CategoryAllNames)
            {
                config.Elements(ns + name).Elements(itemName).Where(x => x.IndexAttributeValue() == Id).Remove();
            }
        }

        public virtual void ToggleAllowOverwrite(XElement config)
        {
            foreach (var name in this.CategoryAllNames)
            {
                var element = config.Elements(ns + name).Elements(itemName).FirstOrDefault(x => x.IndexAttributeValue() == Id);

                if (element == null) {continue;}

                element.SetAttributeValue("allowOverwrite", !element.AllowOverwriteAttributeValue());
            }
        }

        public static IEnumerable<IPackItem> GetItems(Type type, XElement config)
        {
            return GetItems(type, config, PackageCreator.pc, "Add");
        }

        public static IEnumerable<IPackItem> GetItems(Type type, XElement config, XNamespace ns, XName itemName)
        {
            foreach (var category in type.GetCategoryAllNames())
            {
                foreach (var element in config.Elements(ns + category).Elements(itemName).Where(d => d.IndexAttributeValue() !=null))
                {
                    object item = null;
                    try
                    {
                        item = Activator.CreateInstance(type, new object[] { element });
                    }
                    catch {}

                    if (item == null)
                    {
                        item = Activator.CreateInstance(type, new object[] {element.IndexAttributeValue()});
                    }
                    yield return (IPackItem)item;
                }
            }
        }

        public string CategoryName
        {
            get { return this.GetCategoryName(); }
        }

        public string[] CategoryAllNames
        {
            get { return this.GetCategoryAllNames(); }
        }

    }
}
