using System.Collections.Generic;

namespace Orckestra.Search.WebsiteSearch
{
    public class SearchResultEntry
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Highlight { get; set; }

        public IDictionary<string, object> FieldValues { get; set; }
    }
}
