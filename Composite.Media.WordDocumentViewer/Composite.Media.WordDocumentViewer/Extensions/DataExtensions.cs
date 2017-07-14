using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Composite.Data;
using DocumentFormat.OpenXml.Packaging;

namespace Composite.Media.WordDocumentViewer
{
	internal static class DataExtensions
	{
		public static T GetProperty<T>(this IData data, string name)
		{
			var propertyInfo = data.GetType().GetProperty(name);
			if (propertyInfo == null)
			{
				return default(T);
			}
			MethodInfo getMethodInfo = propertyInfo.GetGetMethod();
			return (T)getMethodInfo.Invoke(data, null);
		}

		public static void SetProperty<T>(this IData data, string name, T value)
		{
			var propertyInfo = data.GetType().GetProperty(name);
			if (propertyInfo == null)
			{
				return;
			}
			MethodInfo setMethodInfo = propertyInfo.GetSetMethod();
			setMethodInfo.Invoke(data, new object[] { value });
		}

		public static XElement GetXElement(this OpenXmlPart part)
		{
			return XElement.Load(
				XmlReader.Create(
					new StreamReader(part.GetStream())
				)
			);
		}

		public static XDocument GetXDocument(this MainDocumentPart mainPart)
		{
			var document = XDocument.Load(
				XmlReader.Create(
					new StreamReader(mainPart.GetStream())
				)
			);
			foreach(var part in mainPart.Parts)
			{
				if(part.OpenXmlPart is ImagePart)
					continue;
				document.Root.Add(part.OpenXmlPart.GetXElement());
			}
			return document;
		}

		public static void AddCleared(this XElement element, object content)
		{
			if (content is XElement el1 && el1.Name.LocalName == "p")
			{
				content = el1.Nodes();
			}
			if (content is XElement el2)
			{
			    el2.Clear();
			}
			element.Add(content);
		}

		public static void Clear(this XElement content)
		{
			content.SetAttributeValue("numId", null);
			content.SetAttributeValue("ilvl", null);
		}


		public static string XPathSelectAttributeValue(this XNode node, string expression, IXmlNamespaceResolver resolver)
		{
			try
			{
				return ((IEnumerable)node.XPathEvaluate(expression, resolver)).Cast<XAttribute>().Select(a => a.Value).FirstOrDefault();
			}
			catch
			{
				throw;
			}
		}

		public static string AttributeValue(this XElement element, XName name)
		{
			return element.Attributes(name).Select(d => d.Value).FirstOrDefault();
		}
	}
}
