using System;
using System.Collections.Generic;
using Composite.C1Console.Security;
using Composite.Data;
using Composite.Data.Types;
using System.Xml.Linq;
using System.Linq;

namespace Composite.Tools.PackageCreator.Types
{
	[PCCategory("PageTrees")]
	[ItemManager(typeof(PCPageTreeManager))]
	internal class PCPageTree : SimplePackageCreatorItem, IPackageCreatorItemActionToken, IPackageable
	{
		public bool IsNew { get; set; }
		public PCPageTree(string name, bool isNew)
			: base(name)
		{
			this.IsNew = isNew;
		}

		public override string ActionLabel
		{
			get
			{
				return IsNew 
					? PackageCreatorFacade.GetLocalization(string.Format("{0}.AddRoot.Label", this.CategoryName))
					: PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.Label", this.CategoryName));
			}

			//get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.Label", this.CategoryName)); }
		}

		public override string ActionToolTip
		{
			get {
				return IsNew
					? PackageCreatorFacade.GetLocalization(string.Format("{0}.AddRoot.ToolTip", this.CategoryName))
					: PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.ToolTip", this.CategoryName));
			}

		}

		public override string Id
		{
			get
			{
				return this._name + ":" + this.IsNew;
			}
		}

		public override string GetLabel()
		{
			string result;
			try
			{
				result = PageManager.GetPageById(new Guid(this._name), true).Title;
			}
			catch
			{
				result = base.GetLabel();
			}
			return result + (IsNew ? "(new)" : string.Empty);
		}

		public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
		{
			if (entityToken is DataEntityToken)
			{
				DataEntityToken dataEntityToken = (DataEntityToken)entityToken;
				if (dataEntityToken.Data is IPage)
				{
					var page = dataEntityToken.Data as IPage;
					if(page.GetParentId() != Guid.Empty)
						yield return new PCPageTree(page.Id.ToString(), true);
					yield return new PCPageTree(page.Id.ToString(), false);
				}
			}
		}

		public void Pack(PackageCreator pc)
		{
			var pageId = new Guid(this._name);
			if (pc.ExludedIds.Contains(pageId))
				return;
			PackPageTree(pc, pageId);
			if (this.IsNew)
			{
				var pageStructure = DataFacade.BuildNew<IPageStructure>();
				pageStructure.Id = pageId;
				pageStructure.ParentId = Guid.Empty;
				pageStructure.LocalOrdering = PageManager.GetLocalOrdering(pageId);
				pc.AddData(pageStructure);
			}
			else {
				pc.AddData<IPageStructure>(d => d.Id == pageId);
			}
		}

		public override void AddToConfiguration(XElement config)
		{
			RemoveFromConfiguration(config);
			var category = config.ForceElement(ns + this.CategoryName);
			category.Add(
				new XElement(itemName,
					new XAttribute(category.IndexAttributeName(), this._name),
					new XAttribute("new", this.IsNew.ToString().ToLower())
				)
			);
		}

		public override void RemoveFromConfiguration(XElement config)
		{
			foreach (var name in this.CategoryAllNames)
			{
				config.Elements(ns + name).Elements(itemName).Where(
					x => x.IndexAttributeValue() == this._name
					).Remove();
			}
		}


		private void PackPageTree(PackageCreator pc, Guid pageId)
		{
			foreach (var childPageId in PageManager.GetChildrenIDs(pageId))
			{
				if (pc.ExludedIds.Contains(childPageId))
					continue;
				PackPageTree(pc, childPageId);
				pc.AddData<IPageStructure>(d => d.ParentId == pageId && d.Id == childPageId);
			}
			pc.AddData<IPage>(d => d.Id == pageId);
			pc.AddData<IPagePlaceholderContent>(d => d.PageId == pageId);
		}

		public string ActionTokenName
		{
			get { return this.IsNew.ToString(); }
		}
	}

	internal class PCPageTreeManager : IItemManager
	{
		public IEnumerable<IPackageCreatorItem> GetItems(Type type, XElement config)
		{
			XNamespace ns = PackageCreator.pc;
			XName itemName = "Add";
			foreach (var category in typeof(PCPageTree).GetCategoryAllNames())
			{
				foreach (var element in config.Elements(ns + category).Elements(itemName))
				{
					var item = new PCPageTree(element.IndexAttributeValue(), element.AttributeValue("new") != "false");
					yield return item;
				}
			};
		}


		public IPackageCreatorItem GetItem(Type type, string id)
		{
			var split = id.Split(':');
			var pageId = split[0];
			var isNew = split[1] == true.ToString();
			return new PCPageTree(pageId, isNew);
		}
	}
}
