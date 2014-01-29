using Composite.Core.Routing.Foundation.PluginFacades;
using System;
using System.Xml.Linq;

namespace Composite.Media.YouTube
{
    public class GoogleXmlFeedEntry
    {
        private static readonly XName XML_TITLE_ENTRY = XName.Get("title", "http://www.w3.org/2005/Atom");
        private static readonly XName XML_PUBLISHED_ENTRY = XName.Get("published", "http://www.w3.org/2005/Atom");
        private static readonly XName XML_UPDATED_ENTRY = XName.Get("updated", "http://www.w3.org/2005/Atom");

        public GoogleXmlFeedEntry(XElement entry)
        {
            Published = (DateTime)entry.Element(XML_PUBLISHED_ENTRY);
            Updated = (DateTime)entry.Element(XML_UPDATED_ENTRY);
            Title = (string)entry.Element(XML_TITLE_ENTRY);
            UrlFriendlyName = UrlFormattersPluginFacade.FormatUrl(Title, true);
        }
        public string Title { get; set; }
        public string UrlFriendlyName { get; set; }

        public DateTime Published { get; set; }
        public DateTime Updated { get; set; }
    }
}
