using System;
using System.Globalization;

namespace Orckestra.Search.WebsiteSearch
{
    public class WebsiteSearchQueryFacet
    {
        public string Name { get; set; }
        public string[] Selections { get; set; }
    }

    public class WebsiteSearchQuery
    {
        public CultureInfo Culture { get; set; }

        public string[] Keywords { get; set; }

        public bool CurrentSiteOnly { get; set; }

        public int PageSize { get; set; } = 50;

        public WebsiteSearchQueryFacet[] Facets { get; set; } 


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
