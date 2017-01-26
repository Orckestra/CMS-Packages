using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Composite.Community.Blog.Parameters
{
    public class GetBlogEntriesParameter
    {
        public string CultureCode { get; set; }
        public Guid BlogPageId { get; set; }
        public string Tags { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 0;
    }
}
