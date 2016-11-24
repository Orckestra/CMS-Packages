using System.Globalization;
using Composite.C1Console.Search;

namespace Orckestra.Search
{
    public interface ISearchIndex
    {
        void Initialize();

        T GetCollection<T>(CultureInfo culture) where T: class;
        void RebuildAll();

        void PopulateCollection(CultureInfo culture);
        void DropCollection(CultureInfo culture);

        void DeleteDocumentsBySource(string documentSourceName);
        void PopulateDocumentsFromSource(string documentSourceName);

        void AddDocument(CultureInfo cultureInfo, SearchDocument document);
        void UpdateDocument(CultureInfo cultureInfo, SearchDocument document);
        void RemoveDocument(CultureInfo cultureInfo, string documentId);
    }
}
