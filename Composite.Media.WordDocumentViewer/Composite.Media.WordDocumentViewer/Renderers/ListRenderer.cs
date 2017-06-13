using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using Composite.Core.Xml;

namespace Composite.Media.WordDocumentViewer.Renderers
{
	internal sealed class ListRenderer
	{
		private enum Mode {
			Paragraph,
			List
		}

		public static XDocument RenderLists(XDocument document, DocumentStyles listFormats)
		{
			CreateListItems(document.Root, listFormats);
			return document;
		}

		private static XElement CreateListItems(XElement element, DocumentStyles listFormats)
		{
			element.ReplaceNodes(CreateListItems(element.Nodes(), listFormats));
			element.Clear();
			return element;
			
		}
		
		private static IEnumerable<XNode> CreateListItems(IEnumerable<XNode> nodes, DocumentStyles listFormats)
		{
			List<XNode> result = new List<XNode>();

			var node = nodes.FirstOrDefault();
			while (node != null)
			{
				var nextnode = node.NextNode;
				if (node is XElement)
				{
					if (listFormats.IsListItem(node))
					{
						var element = node as XElement;
						var pair = listFormats.GetListPair(element);
						//string numId = element.AttributeValue("numId");
						//string ilvl = element.AttributeValue("ilvl");
						var ul = listFormats.GetListElement(pair.NumId, pair.Ilvl);
						var li = new XElement(Namespaces.Xhtml + "li");
						li.SetAttributeValue("class", element.AttributeValue("class"));
						li.AddCleared(node);
						var count = node.ElementsAfterSelf().Where(e => listFormats.IsListItem(e, pair.NumId, pair.Ilvl)).Count();

						//TO:Check indent for inner items
						if (listFormats.IsNumerable(pair.NumId, pair.Ilvl))
						{
							for (int i = 0; i < count; i++)
							{
								var innerItems = new XElement("InnerItems");
								node = nextnode;
								while (node != null)
								{
									nextnode = node.NextNode;
									if (listFormats.IsListItem(node, pair.NumId, pair.Ilvl))
									{
										break;
									}
									else
									{
										innerItems.Add(node);
									}
									node = nextnode;
								}
								li.AddCleared(CreateListItems(innerItems.Nodes(), listFormats));
								ul.Add(li);
								li = new XElement(Namespaces.Xhtml + "li");
								if (node is XElement)
									li.SetAttributeValue("class", (node as XElement).AttributeValue("class"));
								li.AddCleared(node);
							}
						}
						else
						{
							node = nextnode;
							while (node != null)
							{
								nextnode = node.NextNode;
								if (listFormats.IsListItem(node, pair.NumId, pair.Ilvl))
								{
									ul.Add(li);
									li = new XElement(Namespaces.Xhtml + "li");
									if (node is XElement)
										li.SetAttributeValue("class", (node as XElement).AttributeValue("class"));
									li.AddCleared(node);
								}
								else {
									nextnode = node;
									break;
								}
								node = nextnode;
							}		
						}
						ul.AddCleared(li);
						result.Add(ul);

					}
					else
					{
						var element = CreateListItems((node as XElement), listFormats);
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

		
	}
}
