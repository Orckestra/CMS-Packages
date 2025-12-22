using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Composite.C1Console.Security;
using Composite.Core.Types;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Tools.PackageCreator.Types
{
    [PackCategory("DataItems", "Data Items")]
    [ItemManager(typeof(PcDataPackItemManager))]
    public class PCDataItem : BasePackItem
    {
        public static HashSet<Type> SkipTypes = new HashSet<Type>
        {
            typeof(IPage),
            typeof(IMethodBasedFunctionInfo),
            typeof(IInlineFunction),
            typeof(IVisualFunction),
            typeof(IXsltFunction),
            typeof(IPageType),
            typeof(IXmlPageTemplate),
            typeof(ISystemActiveLocale),
            typeof(IPage),
            typeof(IHostnameBinding),
            typeof(IUser),
            typeof(IUserGroup),
            typeof(IPageTypeMetaDataTypeLink)
        };

        private static Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();

        public PCDataItem(string name, string type, string label) : base(name)
        {
            DataType = type;
            Label = label;
        }

        public PCDataItem(IData data)
        {
            if (data is IPackageServerSource source)
                Label = source.Url;
            else
                Label = data.GetLabel();

            var dataId = data.DataSourceId.DataId;
            Name = new XElement(itemName, dataId.GetType().GetProperties().Select(p => new XAttribute(p.Name, dataId.GetProperty(p.Name))))
                .ToString();

            DataType = data.DataSourceId.InterfaceType.ToString();
        }

        public string DataType { get; set; }
        public string Params { get; set; }
        public string Label { get; set; }

        public override string Id => $"{Name}*{DataType}";

        public override string GetLabel()
        {
            return string.IsNullOrWhiteSpace(Label) ? Name : Label;
        }

        public override void AddToConfiguration(XElement config)
        {
            RemoveFromConfiguration(config);
            var category = config.ForceElement(ns + CategoryName);
            var typeElement = ForceTypeElement(category, DataType);

            try
            {
                var element = XElement.Parse(Name);
                element.Value = Label;
                typeElement.Add(element);
            }
            catch
            {
                // ignored
            }
        }

        public override void RemoveFromConfiguration(XElement config)
        {
            var category = config.ForceElement(ns + CategoryName);
            var typeElement = ForceTypeElement(category, DataType);
            try
            {
                var element = XElement.Parse(Name);
                var attributes = GetAttributeArray(element);
                foreach (var item in typeElement.Elements(itemName))
                {
                    var itemAttributes = GetAttributeArray(item);

                    var isEqual = attributes.All(pair =>
                        itemAttributes.ContainsKey(pair.Key) && itemAttributes[pair.Key] == pair.Value);

                    if (isEqual)
                    {
                        item.Remove();
                        break; // Don't enumerate after deleting
                    }
                }

                if (!typeElement.Elements().Any()) typeElement.Remove();
            }
            catch
            {
                // ignored
            }
        }

        private static Dictionary<string, string> GetAttributeArray(XElement element)
        {
            return element.Attributes().Where(x => x.Name.Namespace == XNamespace.None)
                .ToDictionary(a => a.Name.LocalName, a => a.Value);
        }

        private static XElement ForceTypeElement(XElement category, string type)
        {
            foreach (var typeElement in category.Elements("Type"))
            {
                var typeElementName = typeElement.AttributeValue("type");
                if (TypeEquals(typeElementName, type))
                    return typeElement;
            }

            var element = new XElement("Type", new XAttribute("type", type));
            category.Add(element);
            return element;
        }

        private static bool TypeEquals(string type1, string type2)
        {
            return TypeManager.TryGetType(type1) == TypeManager.TryGetType(type2);
        }

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
        {
            if (!(entityToken is DataEntityToken dataEntityToken)) 
                yield break;

            if (typeof(IPageDataFolder).IsAssignableFrom(dataEntityToken.InterfaceType))
            {
                yield return new PCDataItem(dataEntityToken.Data);
            }
            else if (SkipTypes.Contains(dataEntityToken.InterfaceType))
            {
                // Nothing
            }
            else
            {
                var data = dataEntityToken.Data;
                var id = PCExclude.GetId(data);

                if (id != Guid.Empty)
                    yield return new PCDataItem(data);
            }
        }
    }

    internal class PcDataPackItemManager : IPackItemManager
    {
        public IEnumerable<IPackItem> GetItems(Type type, XElement config)
        {
            var ns = PackageCreator.Pc;
            XName itemName = "Add";
            foreach (var element in config.Elements(ns + typeof(PCDataItem).GetCategoryName()).Elements("Type"))
            {
                var dataType = element.AttributeValue("type");
                foreach (var item in element.Elements(itemName))
                    yield return new PCDataItem(item.ToString(), dataType, item.Value);
            }
        }

        public IPackItem GetItem(Type type, string id)
        {
            var split = id.Split('*');
            var name = split[0];
            var dataType = split[1];
            return new PCDataItem(name, dataType, null);
        }
    }
}