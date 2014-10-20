using System.Collections.Generic;
using System.Text.RegularExpressions;
using Composite.C1Console.Security;
using Composite.Core.IO;
using Composite.Plugins.Elements.ElementProviders.WebsiteFileElementProvider;
using Composite.Data;
using Composite.Data.Types;
using System;
using System.Xml.Linq;
using System.Linq;
using Composite.Core.ResourceSystem;
using Composite.C1Console.Elements;


namespace Composite.Tools.PackageCreator.Types
{
	//[PCCategory("Excludes")]
#warning localization
	[PackCategory("Excludes", "Excludes")]
	[ItemManager(typeof(PCExludeManager))]
	internal class PCExclude : BasePackItem, IPackInit, IPackToggle
	{
		private const string _idName = "Id";

		public enum Exclude { Page, MediaFolder, DataItem };

		public Exclude Type { get; private set; }

		public string Label { get; private set; }

		public Guid PageId { get; private set; }

		private static readonly Dictionary<Type, bool> _isIdType = new Dictionary<Type, bool>();

		public static Guid GetId(IData data)
		{
			var type = data.DataSourceId.InterfaceType;
			if (!_isIdType.ContainsKey(type))
			{
				var keyProperies = data.GetKeyProperties();
				_isIdType[type] = keyProperies.Count == 1 && keyProperies.First().Name == _idName && keyProperies.First().PropertyType == typeof(Guid);

			}
			if (!_isIdType[type])
				return Guid.Empty;

			return data.GetProperty<Guid>(_idName);
		}

		public PCExclude(string name, Exclude type, string label = "")
			: base(name)
		{
			Type = type;
			Label = label;
			Guid pageId;
			if (Type == Exclude.Page && Guid.TryParse(name, out pageId))
			{
				PageId = pageId;
			}
		}

		public override string Id
		{
			get
			{
				return Name + "*" + Type + "*" + Label;
			}
		}
		public override string ActionLabel
		{
			get
			{
				if (Type == Exclude.Page)
				{
					return PackageCreatorFacade.GetLocalization(string.Format("{0}.Page.Label", this.CategoryName));
				}
				return PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.Label", this.CategoryName));
			}
		}

		public override string ActionToolTip
		{
			get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.ToolTip", this.CategoryName)); }
		}

		public override string GetLabel()
		{
			var result = Name;
			try
			{
				switch (Type)
				{
					case Exclude.DataItem:
						result = Label;
						break;
					case Exclude.MediaFolder:
						break;
					case Exclude.Page:
					default:
						result = PageManager.GetPageById(new Guid(Name), true).Title;
						break;
				}
			}
			catch (Exception)
			{
			}
			return result;
		}
		public override ResourceHandle ItemIcon
		{
			get
			{
				switch (Type)
				{
					case Exclude.DataItem:
						return new ResourceHandle("Composite.Icons", "data-disabled");
						break;
					case Exclude.MediaFolder:
						return new ResourceHandle("Composite.Icons", "folder-disabled");
						break;
					case Exclude.Page:
					default:
						return new ResourceHandle("Composite.Icons", "page-disabled");
						break;
				}

			}
		}

		public static IEnumerable<IPackItem> Create(EntityToken entityToken)
		{
			if (entityToken is DataEntityToken)
			{
				var dataEntityToken = (DataEntityToken)entityToken;
				if (dataEntityToken.Data is IPage)
				{
					var page = dataEntityToken.Data as IPage;
					if (page.GetParentId() != Guid.Empty)
						yield return new PCExclude(page.Id.ToString(), Exclude.Page);
				}
				else if (dataEntityToken.Data is IMediaFileFolder)
				{
					var mediaFolder = dataEntityToken.Data as IMediaFileFolder;
					yield return new PCExclude(mediaFolder.Path, Exclude.MediaFolder);
				}
				else if (dataEntityToken.Data is IPageFolderData)
				{
					var data = dataEntityToken.Data as IPageFolderData;
					yield return new PCExclude(data.Id.ToString(), Exclude.DataItem, data.GetLabel());

				}
				else if (dataEntityToken.Data is IPackageServerSource)
				{
				}
				else if (PCDataItem.SkipTypes.Contains(dataEntityToken.InterfaceType))
				{
					// Nothnig
				}
				else
				{
					var data = dataEntityToken.Data;
					var id = GetId(data);
					if (id != Guid.Empty)
						yield return new PCExclude(id.ToString(), Exclude.DataItem, data.GetLabel());
				}
			}
		}

		public override void AddToConfiguration(XElement config)
		{
			RemoveFromConfiguration(config);
			var category = config.ForceElement(ns + this.CategoryName);
			category.Add(
				new XElement(itemName,
					new XAttribute(category.IndexAttributeName(), this.Name),
					new XAttribute("type", this.Type.ToString().ToLower()),
					Label
				)
			);
		}

		public override void RemoveFromConfiguration(XElement config)
		{
			base.RemoveFromConfiguration(config);
			if (Type == Exclude.Page)
			{
				Tree.Page.ClearCache();
			}
			else if (Type == Exclude.MediaFolder)
			{
				Tree.Media.ClearCache();
			}
		}

		public void Init(PackageCreator creator)
		{
			switch (Type)
			{
				case Exclude.MediaFolder:
					creator.ExludedPaths.Add(this.Name);
					break;
				case Exclude.DataItem:
				case Exclude.Page:
				default:
					Guid id;
					if (Guid.TryParse(this.Name, out id))
					{
						creator.ExludedIds.Add(id);
					}
					break;
			}

		}

		private TreeState GetState()
		{
			var state = TreeState.NotIncluded;
			switch (Type)
			{
				case Exclude.Page:
					state = Tree.Page.GetState(PageId);
					break;
				case Exclude.MediaFolder:
					state = Tree.Media.GetState(Name);
					break;
			}
			return state;
		}

		public ActionCheckedStatus CheckedStatus
		{
			get
			{
				switch (Type)
				{
					case Exclude.Page:
					case Exclude.MediaFolder:
						var state = GetState();
						return (state == TreeState.ExcludedTree) ? ActionCheckedStatus.Checked : ActionCheckedStatus.Unchecked;
						break;
				}
				return ActionCheckedStatus.Uncheckable;
			}
		}

		public bool Disabled
		{
			get
			{
				switch (Type)
				{
					case Exclude.Page:
					case Exclude.MediaFolder:
						var state = GetState();
						switch (state)
						{
							case TreeState.Included:
							case TreeState.ExcludedTree:
								return false;
								break;
						}
						return true;
						break;
				}
				return false;
			}
		}


		public EntityToken GetEntityToken()
		{
			if (Type == Exclude.Page)
			{
				var page = PageManager.GetPageById(PageId);
				if (page != null)
				{
					return page.GetDataEntityToken();
				}
			}
			return null;
		}
	}

	internal class PCExludeManager : IPackItemManager
	{
		public IEnumerable<IPackItem> GetItems(Type type, XElement config)
		{
			XNamespace ns = PackageCreator.pc;
			XName itemName = "Add";
			foreach (var category in typeof(PCExclude).GetCategoryAllNames())
			{
				foreach (var element in config.Elements(ns + category).Elements(itemName))
				{
					var name = element.IndexAttributeValue();
					PCExclude.Exclude exludeType;
					Enum.TryParse(element.AttributeValue("type"), true, out exludeType);
					var label = element.Value;
					var item = new PCExclude(name, exludeType, label);
					yield return item;
				}
			}; ;
		}

		public IPackItem GetItem(Type type, string id)
		{
			var split = id.Split('*');
			var name = split[0];
			var label = split[1];
			PCExclude.Exclude excludeType;
			Enum.TryParse(split[1], true, out excludeType);
			return new PCExclude(name, excludeType, label);
		}
	}
}
