using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.Core.ResourceSystem;
using Composite.C1Console.Security;
using System.Xml.Linq;
using Composite.Tools.PackageCreator.Types;
using Composite.Plugins.Elements.ElementProviders.PageElementProvider;

namespace Composite.Tools.PackageCreator.Types
{
	public abstract class SimplePackageCreatorItem : IPackageCreatorItem
	{
		protected string _name;
		protected EntityToken _entityToken;
		protected virtual XNamespace ns
		{
			get{
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


		public SimplePackageCreatorItem() { }

		protected SimplePackageCreatorItem(string name)
		{
			this._name = name;
		}

		public SimplePackageCreatorItem(EntityToken entityToken)
		{
			this._entityToken = entityToken;
		}

		public virtual string Name
		{
			get{
				return _name;
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
			var category = config.ForceElement(ns + this.GetCategoryNameAtribute());
			if (category.Elements(itemName).Where(x => x.IndexAttributeValue() == this.Name).Count() == 0)
			{
				category.Add(
					new XElement(itemName,
						new XAttribute(category.IndexAttributeName(), this.Name)
					)
				);
			}
		}
		public void RemoveFromConfiguration(XElement config)
		{
			var category = config.ForceElement(ns + this.GetCategoryNameAtribute());
			category.Elements(itemName).Where(x => x.IndexAttributeValue() == this.Name).Remove();
		}

		public static IEnumerable<IPackageCreatorItem> GetItems(Type type, XElement config)
		{
			return GetItems(type, config, PackageCreator.pc, "Add");
		}

		public static IEnumerable<IPackageCreatorItem> GetItems(Type type, XElement config, XNamespace ns, XName itemName)
		{
			var category = type.GetCategoryNameAtribute();
			
			if (category != null)
			{
				var categoryXml = config.ForceElement(ns+ category);
				foreach (var add in categoryXml.Elements(itemName))
				{
					var name = add.IndexAttributeValue();
					if(name != null)
					{
						object item = Activator.CreateInstance(type, new object[]{ name });
						yield return (IPackageCreatorItem)item;
					}
				}
			}
		}
	}
}
