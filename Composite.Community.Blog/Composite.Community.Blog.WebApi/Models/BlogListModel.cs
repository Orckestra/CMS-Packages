
using System.Collections.Generic;

namespace Composite.Community.Blog.Models
{
    public class BlogListModel
    {
        public string Title { get; set; }
        public List<BlogListItemModel> Items { get; set; }

        public int PageNumber { get; set; }

        /// <summary>
        /// Count Of Pages
        /// </summary>
        public int PageCount { get; set; }

    }
}
