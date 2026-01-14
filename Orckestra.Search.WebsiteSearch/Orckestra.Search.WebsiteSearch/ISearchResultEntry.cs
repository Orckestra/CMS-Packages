using System.Collections.Generic;

namespace Orckestra.Search.WebsiteSearch
{
    public class SearchResultEntry
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Highlight { get; set; }
        public float Boost { get; set; }
        public float Score { get; set; }
        public string ExplanationSummary { get; set; }

        public IDictionary<string, object> FieldValues { get; set; }
    }
}
