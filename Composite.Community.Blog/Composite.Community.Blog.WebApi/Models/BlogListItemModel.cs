
using System.Collections.Generic;

namespace Composite.Community.Blog.Models
{
    public class BlogListItemModel
    {
        public string Title { get; set; }

        public string Tags { get; set; }

        public List<BlogTagsByTypeModel> TagsByType { get; set; }

        public string Url { get; set; }

        public string ImageUrl { get; set; }
    }
}
