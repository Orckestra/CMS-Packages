using System;
using System.Globalization;

namespace Composite.Search.SimplePageSearch
{
    public class SimpleSearchQueryFacet
    {
        public string Name { get; set; }
        public string[] Selections { get; set; }
    }

    public class SimpleSearchQuery
    {
        public CultureInfo Culture { get; set; }

        public string[] Keywords { get; set; }

        public bool CurrentSiteOnly { get; set; }

        public int PageSize { get; set; } = 50;

        public SimpleSearchQueryFacet[] Facets { get; set; } 


        /// <summary>
        /// The data types that search results should be filtered by.
        /// </summary>
        public Type[] DataTypes { get; set; }

        /// <summary>
        /// A zero-based page number.
        /// </summary>
        public int PageNumber { get; set; }
    }
}
