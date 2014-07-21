
using System.Xml.Linq;
namespace Composite.Media.ImageGallery.Flickr.Data
{
    public class Comment
    {
        public Comment(XElement el)
        {
            Id = el.Attribute("id").Value;
            CommentText = el.Value;
            Author = el.Attribute("author") != null ? el.Attribute("author").Value : string.Empty;
            AuthorName = el.Attribute("authorname") != null ? el.Attribute("authorname").Value : string.Empty;
            DateCreate = el.Attribute("date_create") != null ? el.Attribute("date_create").Value : string.Empty;
        }

        public string Id { get; set; }
        public string CommentText { get; set; }
        public string Author { get; set; }
        public string AuthorName { get; set; }
        public string DateCreate { get; set; }
    }
}
