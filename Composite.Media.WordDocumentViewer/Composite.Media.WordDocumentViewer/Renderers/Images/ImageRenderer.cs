using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Composite.Core.Xml;

namespace Composite.Media.WordDocumentViewer.Renderers.Images
{

	internal sealed class ImageRenderer
	{
		/// <summary>
		/// Resolve image path
		/// </summary>
		/// <param name="document">Html document</param>
		/// <param name="mediaFile">mediaFile path</param>
		/// <returns>Html document</returns>
		public static XDocument ResolveImagePath(XDocument document, string mediaFile)
		{
			var images = document.Descendants(Namespaces.Xhtml + "img");
			foreach (var image in images)
			{
				image.SetAttributeValue("src", string.Format("~/Frontend/Composite/Media/WordDocumentViewer/ImageHandler.ashx{0}&file={1}", image.AttributeValue("src"), mediaFile));
				var a = image.Parent;
				if (a.Name.LocalName.ToLower() == "a")
				{
					a.SetAttributeValue("href", string.Format("~/Frontend/Composite/Media/WordDocumentViewer/ImageHandler.ashx{0}&file={1}", a.AttributeValue("href"), mediaFile));
				}
			}
			return document;
		}
	}
}
