using System.Globalization;
using System.Threading;
using Composite.C1Console.Search;

namespace Orckestra.Search
{
    /// <summary>
    /// A search index.
    /// </summary>
    public interface ISearchIndex
    {
        /// <summary>
        /// Initializes the search index.
        /// </summary>
        void Initialize(CancellationToken cancellationToken);

        /// <summary>
        /// Reindexes all the documents for every culture.
        /// </summary>
        void RebuildAll(); 

        /// <summary>
        /// Populates the document collection for the given culture.
        /// </summary>
        /// <param name="culture"></param>
        void PopulateCollection(CultureInfo culture);

        /// <summary>
        /// Populates the documents from the given data source for every culture. To be called when a new data source is added.
        /// </summary>
        /// <param name="documentSourceName"></param>
        void PopulateDocumentsFromSource(string documentSourceName); // Replace with "Populate" method?

        /// <summary>
        /// Deletes all the documents for the given culture. To be called when a culture is removed from the cms.
        /// </summary>
        /// <param name="culture">The culture.</param>
        void DropCollection(CultureInfo culture);

        /// <summary>
        /// Deletes all the document populated from the given document source.
        /// </summary>
        /// <param name="documentSourceName"></param>
        void DeleteDocumentsBySource(string documentSourceName);

        /// <summary>
        /// Adds a new document to the collection.
        /// </summary>
        /// <param name="cultureInfo">The culture name.</param>
        /// <param name="document">The document.</param>
        void AddDocument(CultureInfo cultureInfo, SearchDocument document);

        /// <summary>
        /// Updates a new document in the collection.
        /// </summary>
        /// <param name="cultureInfo">The culture name.</param>
        /// <param name="document">The document.</param>
        void UpdateDocument(CultureInfo cultureInfo, SearchDocument document);

        /// <summary>
        /// Removes a document from the collection.
        /// </summary>
        /// <param name="cultureInfo">The culture name.</param>
        /// <param name="documentId">The document identifier.</param>
        void RemoveDocument(CultureInfo cultureInfo, string documentId);
    }
}
