using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Composite.Core.Xml;
using System.Text.RegularExpressions;

namespace Composite.Media.WordDocumentViewer.Renderers
{
	public class CleanRenderer
	{
		public static XDocument Render(XDocument document)
		{
			foreach(var span in document.Root.Descendants(Namespaces.Xhtml + "span").Reverse())
			{
				
				var style = span.AttributeValue("style") ?? string.Empty;

				if (style.Contains("font-weight:bold"))
					span.ReplaceNodes(new XElement(Namespaces.Xhtml + "b", span.Nodes()));
				if (style.Contains("font-style:italic"))
					span.ReplaceNodes(new XElement(Namespaces.Xhtml + "i", span.Nodes()));
				if (style.Contains("text-decoration:underline"))
					span.ReplaceNodes(new XElement(Namespaces.Xhtml + "u", span.Nodes()));
				if (style.Contains("text-decoration:line-through"))
					span.ReplaceNodes(new XElement(Namespaces.Xhtml + "s", span.Nodes()));
				if (style.Contains("vertical-align:sub"))
					span.ReplaceNodes(new XElement(Namespaces.Xhtml + "sub", span.Nodes()));
				if (style.Contains("vertical-align:super"))
					span.ReplaceNodes(new XElement(Namespaces.Xhtml + "sup", span.Nodes()));

				if (style.Contains("display:none"))
					span.RemoveNodes();

				span.ReplaceWith(span.Nodes());
			}

			SkipEmptyElements(document, "b");
			SkipEmptyElements(document, "i");
			SkipEmptyElements(document, "u");
			SkipEmptyElements(document, "s");
			SkipEmptyElements(document, "sub");
			SkipEmptyElements(document, "sup");

			SkipEmptyElements(document, "p");
			SkipEmptyElements(document, "tr");

			foreach (var el in document.Root.Descendants(Namespaces.Xhtml + "tr").Reverse())
			{
				SkipEmptyTR(el);
			}

			return document;
		}

		private static void SkipEmptyElements(XDocument document, string name)
		{
			foreach (var el in document.Root.Descendants(Namespaces.Xhtml + name).Reverse())
			{
				if (IsEmpty(el))
					el.ReplaceWith(new XText(el.Value));
			}
		}

		private static void SkipEmptyTR(XElement el)
		{
			foreach (var td in el.Elements(Namespaces.Xhtml + "td"))
			{
				if (!IsEmpty(td))
					return;
			}
			el.Remove();
		}

		private static bool IsEmpty(XElement el)
		{
			return string.IsNullOrEmpty(el.Value.Trim()) && !el.Elements().Where(e => e.Name.LocalName != "br").Any();
		}
	}
}
