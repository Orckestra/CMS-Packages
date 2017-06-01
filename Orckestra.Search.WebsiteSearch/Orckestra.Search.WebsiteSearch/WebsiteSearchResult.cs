using System;
using System.Collections.Generic;

namespace Orckestra.Search.WebsiteSearch
{
    public class WebsiteSearchResult
    {
        public IReadOnlyCollection<SearchResultEntry> Entries { get; set; } = Array.Empty<SearchResultEntry>();
        public IReadOnlyCollection<SearchResultFacet> Facets { get; set; } = Array.Empty<SearchResultFacet>();

        public int ResultsFound { get; set; }
    }
}
