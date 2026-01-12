using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Tools.PackageCreator.Types
{
    //[PCCategory("Excludes")]
#warning localization
    [PackCategory("Excludes", "Excludes")]
    [ItemManager(typeof(PCExcludeManager))]
    internal class PCExclude : BasePackItem, IPackInit, IPackToggle
    {
        public enum Exclude
        {
            Page,
            MediaFolder,
            DataItem
        }

        private const string _idName = "Id";

        private static readonly Dictionary<Type, bool> _isIdType = new Dictionary<Type, bool>();

        public PCExclude(string name, Exclude type, string label = "") : base(name)
        {
            Type = type;
            Label = label;
            if (Type == Exclude.Page && Guid.TryParse(name, out var pageId)) PageId = pageId;
        }

        public Exclude Type { get; }

        public string Label { get; }

        public Guid PageId { get; }

        public override string Id => $"{Name}*{Type}*{Label}";

        public override string ActionLabel =>
            PackageCreatorFacade.GetLocalization(Type == Exclude.Page
                ? $"{CategoryName}.Page.Label"
                : $"{CategoryName}.Add.Label");

        public override string ActionToolTip => PackageCreatorFacade.GetLocalization($"{CategoryName}.Add.ToolTip");

        public override ResourceHandle ItemIcon
        {
            get
            {
                switch (Type)
                {
                    case Exclude.DataItem:
                        return new ResourceHandle("Composite.Icons", "data-disabled");
                    case Exclude.MediaFolder:
                        return new ResourceHandle("Composite.Icons", "folder-disabled");
                    case Exclude.Page:
                    default:
                        return new ResourceHandle("Composite.Icons", "page-disabled");
                }
            }
        }

        public void Init(PackageCreator creator)
        {
            switch (Type)
            {
                case Exclude.MediaFolder:
                    creator.ExcludedPaths.Add(Name);
                    break;
                case Exclude.DataItem:
                case Exclude.Page:
                default:
                    if (Guid.TryParse(Name, out var id)) creator.ExcludedIds.Add(id);
                    break;
            }
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
                        return state == TreeState.ExcludedTree
                            ? ActionCheckedStatus.Checked
                            : ActionCheckedStatus.Unchecked;
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
                        }

                        return true;
                }

                return false;
            }
        }


        public EntityToken GetEntityToken()
        {
            if (Type != Exclude.Page)
                return null;

            var page = PageManager.GetPageById(PageId);
            return page?.GetDataEntityToken();
        }

        public static Guid GetId(IData data)
        {
            var type = data.DataSourceId.InterfaceType;
            if (!_isIdType.ContainsKey(type))
            {
                var keyProperties = data.GetKeyProperties();
                _isIdType[type] = keyProperties.Count == 1 && keyProperties.First().Name == _idName &&
                                  keyProperties.First().PropertyType == typeof(Guid);
            }

            if (!_isIdType[type])
                return Guid.Empty;

            return data.GetProperty<Guid>(_idName);
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
                // ignored
            }

            return result;
        }

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
        {
            if (!(entityToken is DataEntityToken dataEntityToken)) yield break;

            switch (dataEntityToken.Data)
            {
                case IPage page:
                {
                    if (page.GetParentId() != Guid.Empty)
                        yield return new PCExclude(page.Id.ToString(), Exclude.Page);

                    break;
                }
                case IMediaFileFolder mediaFolder:
                    yield return new PCExclude(mediaFolder.Path, Exclude.MediaFolder);
                    break;
                case IPageDataFolder dataFolder:
                    yield return new PCExclude(dataFolder.PageId.ToString(), Exclude.DataItem, dataFolder.GetLabel());
                    break;
                case IPackageServerSource _:
                    break;
                default:
                {
                    if (PCDataItem.SkipTypes.Contains(dataEntityToken.InterfaceType))
                    {
                        // Nothing
                    }
                    else
                    {
                        var data = dataEntityToken.Data;
                        var id = GetId(data);
                        if (id != Guid.Empty)
                            yield return new PCExclude(id.ToString(), Exclude.DataItem, data.GetLabel());
                    }

                    break;
                }
            }
        }

        public override void AddToConfiguration(XElement config)
        {
            RemoveFromConfiguration(config);
            var category = config.ForceElement(ns + CategoryName);
            category.Add(
                new XElement(itemName,
                    new XAttribute(category.IndexAttributeName(), Name),
                    new XAttribute("type", Type.ToString().ToLower()),
                    Label
                )
            );
        }

        public override void RemoveFromConfiguration(XElement config)
        {
            foreach (var name in CategoryAllNames)
                config.Elements(ns + name).Elements(itemName).Where(x => x.IndexAttributeValue() == Name).Remove();
            if (Type == Exclude.Page)
                Tree.Page.ClearCache();
            else if (Type == Exclude.MediaFolder) Tree.Media.ClearCache();
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
    }

    internal class PCExcludeManager : IPackItemManager
    {
        public IEnumerable<IPackItem> GetItems(Type type, XElement config)
        {
            var ns = PackageCreator.NsPc;
            XName itemName = "Add";
            foreach (var category in typeof(PCExclude).GetCategoryAllNames())
            foreach (var element in config.Elements(ns + category).Elements(itemName))
            {
                var name = element.IndexAttributeValue();
                Enum.TryParse(element.AttributeValue("type"), true, out PCExclude.Exclude excludeType);
                var label = element.Value;
                var item = new PCExclude(name, excludeType, label);
                yield return item;
            }

            ;
            ;
        }

        public IPackItem GetItem(Type type, string id)
        {
            var split = id.Split('*');
            var name = split[0];
            var label = split[1];
            Enum.TryParse(split[1], true, out PCExclude.Exclude excludeType);
            return new PCExclude(name, excludeType, label);
        }
    }
}