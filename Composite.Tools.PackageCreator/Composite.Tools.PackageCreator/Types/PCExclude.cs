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
using Composite.Core.Types;

namespace Composite.Tools.PackageCreator.Types
{
	//[PCCategory("Excludes")]
#warning localization
	[PCCategory("Excludes", "Excludes")]
	[ItemManager(typeof(PCExludeManager))]
	internal class PCExclude : SimplePackageCreatorItem, IInitable
	{
		private const string _idName = "Id";

		public enum Exclude { Page, MediaFolder, DataItem};

		public Exclude Type { get; private set; }

		public string Label { get; set; }

		private static Dictionary<System.Type, bool> _isIdType = new Dictionary<Type, bool>();

		public static Guid GetId(IData data) {
			var type = data.DataSourceId.InterfaceType;
			if (!_isIdType.ContainsKey(type))
			{
				var keyProperies = data.GetKeyProperties();
				_isIdType[type] = keyProperies.Count == 1 && keyProperies.First().Name == _idName && keyProperies.First().PropertyType == typeof(Guid);
				
			}
			if(!_isIdType[type])
				return Guid.Empty;
			
			return data.GetProperty<Guid>(_idName);		
		}

		public PCExclude(string name, Exclude type, string label = "")
			: base(name)
		{
			Type = type;
			Label = label;
		}

		public override string Id
		{
			get
			{
				return this.Name + "*" + Type.ToString() + "*" + Label;
			}
		}
		public override string ActionLabel
		{
			get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.Label", this.CategoryName)); }
		}

		public override string ActionToolTip
		{
			get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.ToolTip", this.CategoryName)); }
		}

		public override string GetLabel()
		{
			string result = Name;
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
			catch
			{
			}
			return result;
		}
		public override Core.ResourceSystem.ResourceHandle ItemIcon
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

		public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
		{
			if (entityToken is DataEntityToken)
			{
				DataEntityToken dataEntityToken = (DataEntityToken)entityToken;
				if (dataEntityToken.Data is IPage)
				{
					var page = dataEntityToken.Data as IPage;
					yield return new PCExclude(page.Id.ToString(), Exclude.Page);
				}
				else if (dataEntityToken.Data is IMediaFileFolder)
				{
					var MediaFolder = dataEntityToken.Data as IMediaFileFolder;
					yield return new PCExclude(MediaFolder.Path, Exclude.MediaFolder);
				}
				else if (dataEntityToken.Data is IPageFolderData)
				{
					var data = dataEntityToken.Data as IPageFolderData;
					yield return new PCExclude(data.Id.ToString(), Exclude.DataItem, data.GetLabel());

				} else {
					var data = dataEntityToken.Data;
					var id = GetId(data);
					if(id != Guid.Empty)
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
	}

	internal class PCExludeManager : IItemManager
	{
		public IEnumerable<IPackageCreatorItem> GetItems(Type type, XElement config)
		{
			XNamespace ns = PackageCreator.pc;
			XName itemName = "Add";
			foreach (var category in typeof(PCExclude).GetCategoryAllNames())
			{
				foreach (var element in config.Elements(ns + category).Elements(itemName))
				{
					var name = element.IndexAttributeValue();
					var exludeType = PCExclude.Exclude.Page;
					Enum.TryParse<PCExclude.Exclude>(element.AttributeValue("type"), true, out exludeType);
					var label = element.Value;
					var item = new PCExclude(name, exludeType, label);
					yield return item;
				}
			}; ;
		}

		public IPackageCreatorItem GetItem(Type type, string id)
		{
			var split = id.Split('*');
			var name = split[0];
			var label = split[1];
			var excludeType = PCExclude.Exclude.Page;
			Enum.TryParse<PCExclude.Exclude>(split[1], true, out excludeType);
			return new PCExclude(name, excludeType, label);
		}
	}
}
