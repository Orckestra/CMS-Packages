using BoboBrowse.Net.Sort;
using Lucene.Net.Index;
using Lucene.Net.Search;
using System;
using System.Globalization;

namespace Orckestra.Search.LuceneNET.BoboCustomization
{
    public class CustomStringLocaleComparatorSource : DocComparatorSource
    {
        private readonly string _field;
        private readonly CultureInfo _cultureInfo;

        public CustomStringLocaleComparatorSource(string field, CultureInfo cultureInfo)
        {
            _field = field;
            _cultureInfo = cultureInfo;
        }

        public override DocComparator GetComparator(IndexReader reader, int docbase) => new CustomStringLocaleDocComparator(FieldCache_Fields.DEFAULT.GetStrings(reader, _field), _cultureInfo);

        private class CustomStringLocaleDocComparator : DocComparator
        {
            private readonly string[] _values;
            private readonly CultureInfo _cultureInfo;

            public CustomStringLocaleDocComparator(string[] values, CultureInfo cultureInfo)
            {
                _values = values;
                _cultureInfo = cultureInfo;
            }

            public override int Compare(ScoreDoc doc1, ScoreDoc doc2) 
            {
                return _values[doc1.Doc] == null ? (_values[doc2.Doc] == null ? 0 : -1) : (_values[doc2.Doc] == null ? 1 : string.Compare(_values[doc1.Doc], _values[doc2.Doc], true, _cultureInfo));
            }

            public override IComparable Value(ScoreDoc doc) => _values[doc.Doc];
        }
    }
}