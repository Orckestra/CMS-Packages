using System;
using System.Collections.Generic;
using Composite.C1Console.Security;
using Composite.Data;
using Composite.Data.Types;
using System.Xml.Linq;
using System.Linq;
using Composite.Core.Types;

namespace Composite.Tools.PackageCreator.Types
{
	[PCCategory("PageTrees")]
	[ItemManager(typeof(PCPageTreeManager))]
	internal class PCPageTree : SimplePackageCreatorItem, IPackageCreatorItemActionToken, IPackageable
	{
		public bool IsRoot { get; private set; }
		public bool IncludeData { get; private set; }
		public PCPageTree(string name, bool isRoot, bool includeData)
			: base(name)
		{
			this.IsRoot = isRoot;
			this.IncludeData = includeData;
		}

		public override string ActionLabel
		{
			get
			{
				return IsRoot 
					? PackageCreatorFacade.GetLocalization(string.Format("{0}.AddRoot.Label", this.CategoryName))
					: PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.Label", this.CategoryName));
			}
		}

		public override string ActionToolTip
		{
			get {
				return IsRoot
					? PackageCreatorFacade.GetLocalization(string.Format("{0}.AddRoot.ToolTip", this.CategoryName))
					: PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.ToolTip", this.CategoryName));
			}

		}

		public override string Id
		{
			get
			{
				return this.Name + ":" + this.IsRoot;
			}
		}

		public override string GetLabel()
		{
			string result;
			try
			{
				result = PageManager.GetPageById(new Guid(this.Name), true).Title;
			}
			catch
			{
				result = base.GetLabel();
			}
			return result + (IsRoot ? "(new)" : string.Empty);
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
						yield return new PCPageTree(page.Id.ToString(), true, true);
					yield return new PCPageTree(page.Id.ToString(), false, true);
				}
			}
		}

		public void Pack(PackageCreator pc)
		{
			var pageId = new Guid(this.Name);
			if (pc.ExludedIds.Contains(pageId))
				return;
			PackPageTree(pc, pageId);
			if (this.IsRoot)
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
					new XAttribute(category.IndexAttributeName(), this.Name),
					new XAttribute("root", this.IsRoot.ToString().ToLower()),
					new XAttribute("data", this.IncludeData.ToString().ToLower())
				)
			);
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

			if (IncludeData)
			{
				pc.AddData<IDataItemTreeAttachmentPoint>(d => d.KeyValue == ValueTypeConverter.Convert<string>(pageId));
				pc.AddData<IPageFolderDefinition>(d => d.PageId == pageId);
				using (new DataScope(DataScopeIdentifier.Administrated))
				{

					var page = PageManager.GetPageById(pageId, true);
					foreach (Type folderType in page.GetDefinedFolderTypes())
					{
						pc.AddData(folderType, d => (d as IPageFolderData).PageId == pageId);
					}

					//var items = PageFolderFacade.GetFolderData(page).ToList();
					//foreach (var item in items)
					//{
					//	if (!pc.ExludedIds.Contains((item as IPageFolderData).Id))
					//		pc.AddData(item);
					//}
				}
			}
		}

		public string ActionTokenName
		{
			get { return this.IsRoot.ToString(); }
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
					var item = new PCPageTree(
						element.IndexAttributeValue(),
						element.AttributeValue("root") != "false",
						element.AttributeValue("data") != "false"
						);
					yield return item;
				}
			};
		}


		public IPackageCreatorItem GetItem(Type type, string id)
		{
			var split = id.Split(':');
			var pageId = split[0];
			var isNew = split[1] == true.ToString();
			return new PCPageTree(pageId, isNew, true);
		}
	}
}
