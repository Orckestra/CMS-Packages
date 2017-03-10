using System;
using System.Collections.Generic;

namespace Composite.Search.SimplePageSearch
{
    public class SimpleSearchResult
    {
        public IReadOnlyCollection<SearchResultEntry> Entries { get; set; } = Array.Empty<SearchResultEntry>();
        public int ResultsFound { get; set; }
    }
}
