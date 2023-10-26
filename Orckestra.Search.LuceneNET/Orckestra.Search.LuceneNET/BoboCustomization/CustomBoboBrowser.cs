using BoboBrowse.Net;
using BoboBrowse.Net.Facets;
using BoboBrowse.Net.Sort;
using Lucene.Net.Search;
using System.Collections.Generic;
using System.Linq;

namespace Orckestra.Search.LuceneNET.BoboCustomization
{
    public class CustomBoboBrowser : MultiBoboBrowser
    {
        public CustomBoboBrowser(BoboIndexReader reader) : base(CreateBrowsables(reader)) { }

        public static void GatherSubReaders(IList<BoboIndexReader> readerList, BoboIndexReader reader)
        {
            if (reader.GetSequentialSubReaders() is BoboIndexReader[] subReaders)
            {
                foreach (var subReader in subReaders)
                {
                    GatherSubReaders(readerList, subReader);
                }
            }
            else
            {
                readerList.Add(reader);
            }
        }

        public static CustomBoboSubBrowser[] CreateSegmentedBrowsables(IEnumerable<BoboIndexReader> readerList) 
            => readerList.Select(reader => new CustomBoboSubBrowser(reader)).ToArray();


        public static IBrowsable[] CreateBrowsables(BoboIndexReader reader)
        {
            var readerList = new List<BoboIndexReader>();
            GatherSubReaders(readerList, reader);
            return CreateSegmentedBrowsables(readerList);
        }

        public static IEnumerable<BoboIndexReader> GatherSubReaders(IList<BoboIndexReader> readerList)
        {
            var readerListNew = new List<BoboIndexReader>();
            foreach (var reader in readerList)
            {
                GatherSubReaders(readerListNew, reader);
            }  
            return readerListNew;
        }

        public static IBrowsable[] CreateBrowsables(IList<BoboIndexReader> readerList) => CreateSegmentedBrowsables(GatherSubReaders(readerList));

        public override IEnumerable<string> FacetNames => _subBrowsers[0].FacetNames;

        public override IFacetHandler GetFacetHandler(string name) => _subBrowsers[0].GetFacetHandler(name);

        public override SortCollector GetSortCollector(SortField[] sort, Query query, int offset, int count, bool fetchStoredFields, IEnumerable<string> termVectorsToFetch, bool forceScoring, string[] groupBy, int maxPerGroup, bool collectDocIdCache)
        {
            return _subBrowsers.Length == 1 
                ? _subBrowsers[0].GetSortCollector(sort, query, offset, count, fetchStoredFields, termVectorsToFetch, forceScoring, groupBy, maxPerGroup, collectDocIdCache) 
                : CustomBoboSubBrowser.BuildSortCollector(this, query, sort, offset, count, forceScoring, fetchStoredFields, termVectorsToFetch, groupBy, maxPerGroup, collectDocIdCache);
        }
    }
}
