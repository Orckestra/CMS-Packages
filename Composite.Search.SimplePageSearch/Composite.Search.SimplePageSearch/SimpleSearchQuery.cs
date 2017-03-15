using System.Globalization;

namespace Composite.Search.SimplePageSearch
{
    public class SimpleSearchQuery
    {
        public CultureInfo Culture { get; set; }
        public string[] Keywords { get; set; }
        public bool CurrentSiteOnly { get; set; }
        public int PageSize { get; set; } = 50;

        /// <summary>
        /// A zero-based page number.
        /// </summary>
        public int PageNumber { get; set; }
    }
}
