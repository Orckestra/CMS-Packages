using System;

namespace Composite.Search.SimplePageSearch
{
    public class SearchResultEntry
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid? PageId { get; set; }
    }
}
