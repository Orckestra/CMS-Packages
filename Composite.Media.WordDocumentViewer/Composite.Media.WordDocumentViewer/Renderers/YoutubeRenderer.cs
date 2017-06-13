using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Composite.Core.Xml;

namespace Composite.Media.WordDocumentViewer.Renderers
{
	internal sealed class YoutubeRenderer
	{
		static XElement GetFunction(string videoId)
		{
			return new XElement(Namespaces.Function10 + "function",
				new XAttribute("name", "Composite.Media.YouTube"),
				GetParam("VideoId", videoId),
				GetParam("Height", "315"),
				GetParam("Width","420"),
				GetParam("FullScreen", "True")
			);
		}

		static XElement GetParam(string name, string value)
		{
			return new XElement(Namespaces.Function10 + "param",
			                    new XAttribute("name", name),
			                    new XAttribute("value", value)
				);
		}

		public static XDocument RenderYoutubeLinks(XDocument document)
		{
			foreach (var link in document.Descendants(Namespaces.Xhtml + "a").Reverse())
			{
				var href = link.AttributeValue("href");
				if (href == null) continue;

				var match = Regex.Match(href, @"^http://www.youtube.com/v/(\w*)$");
				if(match.Success)
				{
					var videoId = match.Groups[1].Value;
					link.ReplaceWith(GetFunction(videoId));
				}
			}
			return document;
		}

	}
}
