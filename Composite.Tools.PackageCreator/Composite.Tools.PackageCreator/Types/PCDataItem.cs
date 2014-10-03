using Composite.C1Console.Security;
using Composite.Core.Types;
using Composite.Data;
using Composite.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Composite.Tools.PackageCreator.Types
{
	[PackCategory("DataItems", "Data Items")]
	[ItemManager(typeof(PcDataPackItemManager))]
	public class PCDataItem : BasePackItem
	{
		public string DataType { get; set; }
		public string Params { get; set; }
		public string Label { get; set; }

		public static HashSet<Type> SkipTypes = new HashSet<Type>() { 
			typeof(IPage),
			typeof(IMethodBasedFunctionInfo),
			typeof(IInlineFunction),
			typeof(IVisualFunction),
			typeof(IXsltFunction),
			typeof(IPageType),
			typeof(IXmlPageTemplate),
			typeof(ISystemActiveLocale),
			typeof(IPage),
			
		};

		private static Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();
		
		public PCDataItem(string name, string type, string label)
			:base(name)
		{
			DataType = type;
			Label = label;
		}
		public PCDataItem(IData data)
		{
			Label = data.GetLabel();
			var dataId = data.DataSourceId.DataId;
			Name = new XElement(itemName,
				dataId.GetType().GetProperties()
				.Select(p => new XAttribute(p.Name, dataId.GetProperty(p.Name)))
				).ToString();
			DataType = data.DataSourceId.InterfaceType.ToString();
			
		}

		public override string Id
		{
			get
			{
				return Name + "*" + DataType;
			}
		}

		public override string GetLabel()
		{
			return string.IsNullOrWhiteSpace(Label) ? Name : Label;
		}

		public override void AddToConfiguration(XElement config)
		{
			RemoveFromConfiguration(config);
			var category = config.ForceElement(ns + this.CategoryName);
			var typeElement = ForceTypeElement(category, DataType);

			try
			{
				XElement element = XElement.Parse(Name);
				element.Value = Label;
				typeElement.Add(element);
			}
			catch { }
		}

		public override void RemoveFromConfiguration(XElement config)
		{
			var category = config.ForceElement(ns + this.CategoryName);
			var typeElement = ForceTypeElement(category, DataType);
			try
			{
				XElement element = XElement.Parse(Name);
				var attributes = GetAttrbuteArray(element);
				foreach (var item in typeElement.Elements(itemName))
				{
					var itemAttributes = GetAttrbuteArray(item);
					if (attributes.Count != attributes.Count)
						continue;
					var isEqual = true;
					foreach (var pair in attributes)
					{
						if (!itemAttributes.ContainsKey(pair.Key) || itemAttributes[pair.Key] != pair.Value)
						{
							isEqual = false;
							break;
						}
					}
					if (isEqual)
					{
						item.Remove();
						break;// Don't enumerate after deleting
					}
				}
				if (!typeElement.Elements().Any())
				{
					typeElement.Remove();
				}
			}
			catch { }

		}

		private Dictionary<string, string> GetAttrbuteArray(XElement element) {
			return element.Attributes().Where(x => x.Name.Namespace == XNamespace.None)
				.ToDictionary(a => a.Name.LocalName, a => a.Value);
		}

		private XElement ForceTypeElement(XElement category, string type) {
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

		private bool TypeEquals(string type1, string type2)
		{
			return TypeManager.TryGetType(type1) == TypeManager.TryGetType(type2);
		}

		public static IEnumerable<IPackItem> Create(EntityToken entityToken)
		{
			if (entityToken is DataEntityToken)
			{
				DataEntityToken dataEntityToken = (DataEntityToken)entityToken;
				if (SkipTypes.Contains(dataEntityToken.InterfaceType)){

				}
				else if (dataEntityToken.Data is IPageFolderData)
				{
					var data = dataEntityToken.Data as IPageFolderData;
					yield return new PCDataItem(data);
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
	}

	internal class PcDataPackItemManager : IPackItemManager
	{
		public IEnumerable<IPackItem> GetItems(Type type, XElement config)
		{
			XNamespace ns = PackageCreator.pc;
			XName itemName = "Add";
			foreach (var element in config.Elements(ns + typeof(PCDataItem).GetCategoryName()).Elements("Type"))
			{
				var dataType = element.AttributeValue("type");
				foreach (var item in element.Elements(itemName))
				{
					yield return new PCDataItem(item.ToString(), dataType, element.Value);
				}
			}
			yield break;
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
