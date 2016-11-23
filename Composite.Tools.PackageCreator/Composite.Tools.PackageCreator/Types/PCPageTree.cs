using System;
using System.Collections.Generic;
using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Core.Collections;
using Composite.Data;
using Composite.Data.Types;
using System.Linq;
using System.Xml.Linq;
using Composite.Core.Types;

namespace Composite.Tools.PackageCreator.Types
{
	[PackCategory("PageTrees")]
	[ItemManager(typeof(PCPageTreeManager))]
	internal class PCPageTree : BasePackItem, IPackItemActionToken, IPack, IPackToggle
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
			get
			{
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

		public Guid PageId
		{
			get
			{
				return new Guid(Name);
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

		public static IEnumerable<IPackItem> Create(EntityToken entityToken)
		{
			if (entityToken is DataEntityToken)
			{
				var dataEntityToken = (DataEntityToken)entityToken;
				if (dataEntityToken.Data is IPage)
				{
					var page = dataEntityToken.Data as IPage;
					//if (page.GetParentId() != Guid.Empty)
					//	yield return new PCPageTree(page.Id.ToString(), true, true);
					yield return new PCPageTree(page.Id.ToString(), false, true);
				}
			}
		}

		public void Pack(PackageCreator pc)
		{
			using (new DataScope(DataScopeIdentifier.Administrated))
			{
				var pageId = new Guid(this.Name);
				if (pc.ExcludedIds.Contains(pageId) || pc.AlreadyAddedIds.Contains(pageId))
					return;

                pc.AlreadyAddedIds.Add(pageId); // avoid adding duplicate data for pages

				PackPageTree(pc, pageId, IsRoot);

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

		public override void RemoveFromConfiguration(XElement config)
		{
            foreach (var name in this.CategoryAllNames)
            {
                config.Elements(ns + name).Elements(itemName).Where(x => x.IndexAttributeValue() == Name).Remove();
            }
            Tree.Page.ClearCache();
		}

		private void PackPageTree(PackageCreator pc, Guid pageId, bool isRoot = false)
		{
			var page = PageManager.GetPageById(pageId, true);
			if (page == null) // Page does not exists in current locale
				return;

			foreach (var childPageId in PageManager.GetChildrenIDs(pageId))
			{
				if (pc.ExcludedIds.Contains(childPageId) || pc.AlreadyAddedIds.Contains(childPageId))
					continue;

				PackPageTree(pc, childPageId);
			}

			foreach (var dataScopeIdentifier in DataFacade.GetSupportedDataScopes(typeof(IPage)))
			{
				using (new DataScope(dataScopeIdentifier))
				{
					var pageinScope = PageManager.GetPageById(pageId, true);
					if (pageinScope != null)
					{
						pc.AddData(pageinScope);
						pc.AddData<IPagePlaceholderContent>(dataScopeIdentifier, d => d.PageId == pageId);
					}
				}
			}

			if (isRoot)
			{
				using (new DataScope(DataScopeIdentifier.Public))
				{
					var pageStructure = DataFacade.BuildNew<IPageStructure>();
					pageStructure.Id = pageId;
					pageStructure.ParentId = Guid.Empty;
					pageStructure.LocalOrdering = PageManager.GetLocalOrdering(pageId);
					pc.AddData(pageStructure);
				}
			}
			else
			{
				pc.AddData<IPageStructure>(d => d.Id == pageId);
			}

			if (IncludeData)
			{
				pc.AddData<IDataItemTreeAttachmentPoint>(d => d.KeyValue == ValueTypeConverter.Convert<string>(pageId));
				pc.AddData<IPageFolderDefinition>(d => d.PageId == pageId);

				foreach (Type folderType in page.GetDefinedFolderTypes())
				{
					pc.AddData(folderType, d => (d as IPageRelatedData).PageId == pageId);
				}
			}
		}

		public string ActionTokenName
		{
			get { return this.IsRoot.ToString(); }
		}

		public ActionCheckedStatus CheckedStatus
		{
			get
			{
				var status = Tree.Page.GetState(PageId);
				return (status == TreeState.IncludedTree) ? ActionCheckedStatus.Checked : ActionCheckedStatus.Unchecked;
			}
		}

		public bool Disabled
		{
			get
			{
				var status = Tree.Page.GetState(PageId);
				switch (status)
				{
					case TreeState.NotIncluded:
					case TreeState.IncludedTree:
						return false;
				}
				return true;
			}
		}


		public EntityToken GetEntityToken()
		{
			var page = PageManager.GetPageById(PageId);
			if (page != null)
			{
				return page.GetDataEntityToken();
			}
			return null;
		}
	}

	internal class PCPageTreeManager : IPackItemManager
	{
		public IEnumerable<IPackItem> GetItems(Type type, XElement config)
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

		public IPackItem GetItem(Type type, string id)
		{
			var split = id.Split(':');
			var pageId = split[0];
			var isNew = split[1] == true.ToString();
			return new PCPageTree(pageId, isNew, true);
		}
	}
}
