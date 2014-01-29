using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Composite.Media.YouTube
{
    public class YoutubeChannel
    {
        public IEnumerable<YoutubePlaylist> Playlists { get; private set; }
        public IEnumerable<YoutubeVideo> Videos { get; private set; }
        public int TotalResults { get; private set; }
        public int StartIndex { get; private set; }
        public int ItemsPerPage { get; private set; }
        
       

        public YoutubeChannel(XElement feed, string feedType)
        {
            switch (feedType)
            {
                case "playlists": Playlists = feed.Elements(YoutubeChannelFacade.XML_ENTRY).Select(entry => new YoutubePlaylist(entry)).ToList(); break;
                case "videos": Videos = feed.Elements(YoutubeChannelFacade.XML_ENTRY).Select(entry => new YoutubeVideo(entry)).ToList(); break;
                default: break;
            }

            TotalResults = int.Parse(feed.Element(YoutubeChannelFacade.XML_TOTALRESULTS).Value);
            StartIndex = int.Parse(feed.Element(YoutubeChannelFacade.XML_STARTINDEX).Value);
            ItemsPerPage = int.Parse(feed.Element(YoutubeChannelFacade.XML_ITEMSPERPAGE).Value);
           
        }
    }
}
