using BoboBrowse.Net.Sort;
using Lucene.Net.Index;
using Lucene.Net.Search;
using System;

namespace Orckestra.Search.LuceneNET.BoboCustomization
{
    public class CustomStringComparatorSource : DocComparatorSource
    {
        private readonly string _field;

        public CustomStringComparatorSource(string field) => _field = field;

        public override DocComparator GetComparator(IndexReader reader, int docbase) => new CustomStringValDocComparator(FieldCache_Fields.DEFAULT.GetStrings(reader, _field));

        private class CustomStringValDocComparator : DocComparator
        {
            private readonly string[] _values;

            public CustomStringValDocComparator(string[] values) => _values = values;

            public override int Compare(ScoreDoc doc1, ScoreDoc doc2)
            {
                return _values[doc1.Doc] == null 
                    ? (_values[doc2.Doc] == null ? 0 : -1) 
                    : (_values[doc2.Doc] == null ? 1 : string.Compare(_values[doc1.Doc], _values[doc2.Doc], StringComparison.InvariantCultureIgnoreCase));
            }

            public override IComparable Value(ScoreDoc doc) => _values[doc.Doc];
        }
    }
}