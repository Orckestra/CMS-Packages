using System.Collections.Generic;

namespace Orckestra.Search.WebsiteSearch
{
    public class SearchResultFacet
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public IReadOnlyCollection<SearchResultFacetHit> Hits { get; set; }
    }

    public class SearchResultFacetHit
    {
        public string Value { get; set; }
        public string Label { get; set; }
        public int HitCount { get; set; }
    }
}
