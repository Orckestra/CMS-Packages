using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Composite.Core.Xml;
using DocumentFormat.OpenXml.Packaging;

namespace Composite.Media.WordDocumentViewer.Renderers
{
	internal sealed class LinkRenderer
	{

		internal static XDocument RenderLinks(XDocument document, IEnumerable<ExternalRelationship> externalRelationship)
		{
			var externalLinks = externalRelationship.ToDictionary(d => d.Id, d => d.Uri.OriginalString);
			foreach(var link in document.Descendants(Namespaces.Xhtml + "a"))
			{
				var id = link.AttributeValue("id");
				var href = link.AttributeValue("href")??string.Empty;
				var anchor = href.StartsWith("#") ? href : string.Empty;
				if(id != null)
				{
					if(externalLinks.ContainsKey(id))
					{
						link.SetAttributeValue("href", externalLinks[id] + anchor);
						link.SetAttributeValue("id", null);
					}
				}
			}
			return document;
		}
	}
}
