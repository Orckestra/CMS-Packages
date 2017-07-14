using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Composite.Core.Xml;

namespace Composite.Media.WordDocumentViewer.Renderers
{
	public class MarkupRenderer
	{
		private static readonly string MarkupClassName = "Code";
		public delegate XElement MarkupFunc(string text);


		static XElement CodeMarkupFunc(string text)
		{
			return new XElement(Namespaces.Function10 + "function",
				new XAttribute("name", "Composite.Web.Html.SyntaxHighlighter"),
				new XElement(Namespaces.Function10 + "param",
					new XAttribute("name", "SourceCode"),
					text
				),
				new XElement(Namespaces.Function10 + "param",
					new XAttribute("name", "CodeType"),
					"c#"
				)
			);
		}

		static XElement XmlMarkupFunc(string text)
		{
			return new XElement(Namespaces.Function10 + "function",
				new XAttribute("name", "Composite.Web.Html.SyntaxHighlighter"),
				new XElement(Namespaces.Function10 + "param",
					new XAttribute("name", "SourceCode"),
					text
				), 
				new XElement(Namespaces.Function10 + "param",
					new XAttribute("name", "CodeType"),
					"xml"
				)
			);
		}

		public static XDocument Render(XDocument document)
		{
			document.Root.Add(
				new XAttribute(XNamespace.Xmlns + "f", Namespaces.Function10));
			CreateItems(document.Root);
			return document;
		}

		private static XElement CreateItems(XElement element)
		{
			element.ReplaceNodes(CreateItems(element.Nodes()));
			element.Clear();
			return element;

		}


		private static IEnumerable<XNode> CreateItems(IEnumerable<XNode> nodes)
		{
			List<XNode> result = new List<XNode>();

			var node = nodes.FirstOrDefault();
			while (node != null)
			{
				var nextnode = node.NextNode;
				if (node is XElement)
				{
					if (IsMarkup(node))
					{
						var text = ((XElement)node).Value;
						node = nextnode;

						while (IsMarkup(node))
						{
							nextnode = node.NextNode;
							text += "\n" +((XElement)node).Value;
							node = nextnode;
						}

						if (text.TrimStart().StartsWith("<") && !text.TrimStart().StartsWith("<%"))
						{
							result.Add(XmlMarkupFunc(text));
						}
						else
						{
							result.Add(CodeMarkupFunc(text));
						}
					}
					else
					{
						var element = CreateItems((node as XElement));
						result.Add(element);
					}
				}
				else
				{
					result.Add(node);

				}
				node = nextnode;
			}
			return result;
		}

		private static bool IsMarkup(XNode node)
		{
			if (!(node is XElement))
				return false;
			if (((XElement)node).Name.LocalName != "p")
				return false;

			return ((XElement)node).AttributeValue("class") == MarkupClassName;
		}
	




	}
}
