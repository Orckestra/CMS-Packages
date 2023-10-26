using BoboBrowse.Net;
using BoboBrowse.Net.Sort;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orckestra.Search.LuceneNET.BoboCustomization
{
    public class CustomBoboSubBrowser : BoboSubBrowser
    {
        public CustomBoboSubBrowser(BoboIndexReader reader) : base(reader) { }

        public override SortCollector GetSortCollector(SortField[] sort, Query query, int offset, int count, bool fetchStoredFields, IEnumerable<string> termVectorsToFetch, bool forceScoring, string[] groupBy, int maxPerGroup, bool collectDocIdCache)
        {
            return BuildSortCollector(this, query, sort, offset, count, forceScoring, fetchStoredFields, termVectorsToFetch, groupBy, maxPerGroup, collectDocIdCache);
        }

        public static SortCollector BuildSortCollector(IBrowsable browser, Query q,  SortField[] sort,  int offset,  int count,  bool forceScoring,  bool fetchStoredFields,
            IEnumerable<string> termVectorsToFetch,  string[] groupBy,  int maxPerGroup,  bool collectDocIdCache)
        {
            bool doScoring = forceScoring;
            if (sort == null || sort.Length == 0)
            {
                switch (q)
                {
                    case null:
                    case MatchAllDocsQuery _:
                        break;
                    default:
                        sort = new SortField[1] { SortField.FIELD_SCORE };
                        break;
                }
            }
            if (sort == null || sort.Length == 0)
            {
                sort = new SortField[1] { SortField.FIELD_DOC };
            }    

            foreach (var sortField in sort)
            {
                if (sortField.Type == 0)
                {
                    doScoring = true;
                    break;
                }
            }
            DocComparatorSource compSource;
            if (sort.Length == 1)
            {
                var sf = Convert(browser, sort[0]);
                compSource = GetComparatorSource(browser, sf);
            }
            else
            {
                var compSources = new DocComparatorSource[sort.Length];
                for (int index = 0; index < sort.Length; ++index)
                {
                    compSources[index] = GetComparatorSource(browser, Convert(browser, sort[index]));
                }            
                compSource = new MultiDocIdComparatorSource(compSources);
            }
            return new SortCollectorImpl(compSource, sort, browser, offset, count, doScoring, fetchStoredFields, termVectorsToFetch, groupBy, maxPerGroup, collectDocIdCache);
        }

        private static DocComparatorSource GetComparatorSource(IBrowsable browser, SortField sortField)
        {
            DocComparatorSource inner;
            if (SortField.FIELD_DOC.Equals(sortField))
            {
                inner = new DocComparatorSource.DocIdDocComparatorSource();
            }
            else if (SortField.FIELD_SCORE.Equals(sortField) || sortField.Type == 0)
            {
                inner = new ReverseDocComparatorSource(new DocComparatorSource.RelevanceDocComparatorSource());
            }
            else if (sortField is BoboCustomSortField customSortField)
            {
                inner = customSortField.GetCustomComparatorSource();
            }
            else
            {
                var facetNames = browser.FacetNames;
                string field = sortField.Field;
                if (facetNames.Contains(field))
                {
                    inner = browser.GetFacetHandler(field).GetDocComparatorSource();
                }
                else
                {
                    inner = GetNonFacetComparatorSource(sortField);
                }
            }
            bool reverse = sortField.Reverse;
            if (reverse)
            {
                inner = new ReverseDocComparatorSource(inner);
            }
            inner.IsReverse = reverse;
            return inner;
        }

        private static SortField Convert(IBrowsable browser, SortField sort)
        {
            string field = sort.Field;
            var facetHandler = browser.GetFacetHandler(field);
            return facetHandler != null ? new BoboCustomSortField(field, sort.Reverse, facetHandler.GetDocComparatorSource()) : sort;
        }

        private static DocComparatorSource GetNonFacetComparatorSource(SortField sortField)
        {
            string field = sortField.Field;
            var locale = sortField.Locale;
            if (locale != null) return new CustomStringLocaleComparatorSource(field, locale);
            int type = sortField.Type;
            switch (type)
            {
                case 3:
                    return new CustomStringComparatorSource(field);
                case 4:
                    return new DocComparatorSource.IntDocComparatorSource(field);
                case 5:
                    return new DocComparatorSource.FloatDocComparatorSource(field);
                case 6:
                    return new DocComparatorSource.LongDocComparatorSource(field);
                case 7:
                    return new DocComparatorSource.LongDocComparatorSource(field);
                case 8:
                    return new DocComparatorSource.ShortDocComparatorSource(field);
                case 9:
                    throw new InvalidOperationException("Lucene custom sort no longer supported: " + field);
                case 10:
                    return new DocComparatorSource.ByteDocComparatorSource(field);
                case 11:
                    return new CustomStringComparatorSource(field);
                default:
                    throw new InvalidOperationException("Illegal sort type: " + type + ", for field: " + field);
            }
        }
    }
}
