using System.Globalization;
using Lucene.Net.Store;

namespace Orckestra.Search.LuceneNET
{
    public interface IIndexContainer
    {
        void Initialize();
        void SubscribeToSources();

        Directory GetDirectory(CultureInfo culture);
    }
}
