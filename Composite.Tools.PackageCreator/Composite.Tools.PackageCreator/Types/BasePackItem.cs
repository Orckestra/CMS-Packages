using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;

namespace Composite.Tools.PackageCreator.Types
{
    /// <summary>
    ///     Represents a package creator item that has only one 'name' attribute in serialized state.
    ///     Derived classes have to define a constructor from with a single <see cref="string" /> parameter.
    /// </summary>
    public abstract class BasePackItem : IPackItem
    {
        protected EntityToken _entityToken;


        protected BasePackItem()
        {
        }

        protected BasePackItem(string name)
        {
            Name = name;
        }

        protected BasePackItem(XElement element)
        {
            Name = element.IndexAttributeValue();
        }

        protected BasePackItem(EntityToken entityToken)
        {
            _entityToken = entityToken;
        }

        protected virtual XNamespace ns => PackageCreator.NsPc;

        protected virtual XName itemName => "Add";

        public virtual string Name { get; set; }

        public virtual string Id => Name;

        public virtual string GetLabel()
        {
            return Name;
        }


        public virtual string ActionLabel => PackageCreatorFacade.GetLocalization("Add.Label");

        public virtual string ActionToolTip => PackageCreatorFacade.GetLocalization("Add.ToolTip");

        public virtual ResourceHandle ItemIcon => new ResourceHandle("Composite.Icons", "page-publication");

        public virtual ResourceHandle ActionIcon =>
            new ResourceHandle("Composite.Icons", "package-element-closed-availableitem");

        public virtual void AddToConfiguration(XElement config)
        {
            RemoveFromConfiguration(config);
            var category = config.ForceElement(ns + CategoryName);
            category.Add(
                new XElement(itemName,
                    new XAttribute(category.IndexAttributeName(), Id)
                ));
        }

        public virtual void RemoveFromConfiguration(XElement config)
        {
            foreach (var name in CategoryAllNames)
                config.Elements(ns + name).Elements(itemName).Where(x => x.IndexAttributeValue() == Id).Remove();
        }

        public string CategoryName => this.GetCategoryName();

        public string[] CategoryAllNames => this.GetCategoryAllNames();

        public static IEnumerable<IPackItem> GetItems(Type type, XElement config)
        {
            return GetItems(type, config, PackageCreator.NsPc, "Add");
        }

        public static IEnumerable<IPackItem> GetItems(Type type, XElement config, XNamespace ns, XName itemName)
        {
            foreach (var category in type.GetCategoryAllNames())
            foreach (var element in config.Elements(ns + category).Elements(itemName)
                         .Where(d => d.IndexAttributeValue() != null))
            {
                object item = null;
                try
                {
                    item = Activator.CreateInstance(type, element);
                }
                catch
                {
                    // ignored
                }

                if (item == null) item = Activator.CreateInstance(type, element.IndexAttributeValue());
                yield return (IPackItem)item;
            }
        }
    }
}