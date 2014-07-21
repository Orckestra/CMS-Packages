using System.Xml.Linq;

namespace Composite.Media.ImageGallery.Flickr.Data
{
    public class PhotoSet
    {
        public PhotoSet(XElement el)
        {
            Id = el.Attribute("id").Value;
            Title = el.Element("title") != null ? el.Element("title").Value : string.Empty;
            Description = el.Element("description") != null ? el.Element("description").Value : string.Empty;
            ThumbnailUrl = string.Format("http://farm{0}.static.flickr.com/{1}/{2}_{3}_s.jpg",
                el.Attribute("farm").Value, el.Attribute("server").Value, el.Attribute("primary").Value,
                el.Attribute("secret").Value);
            OwnerName = el.Attribute("ownername") != null ? el.Attribute("ownername").Value : string.Empty;
            Owner = el.Attribute("owner") != null ? el.Attribute("owner").Value : string.Empty;
            PhotosCount = el.Attribute("photos").Value;
            CommentsCount = el.Attribute("count_comments") != null ? el.Attribute("count_comments").Value : string.Empty;
        }

        public string Id
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Description { get; set; }

        public string PhotosCount
        {
            get;
            set;
        }

        public string ThumbnailUrl
        {
            get;
            set;
        }

        public string Owner { get; set; }

        public string OwnerName { get; set; }

        public string CommentsCount { get; set; }
    }
}
