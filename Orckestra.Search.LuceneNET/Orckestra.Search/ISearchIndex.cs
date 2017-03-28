using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Composite.Search;

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
        void Initialize(IEnumerable<CultureInfo> cultures, CancellationToken cancellationToken, out ICollection<CultureInfo> newlyCreatedCollections);

        /// <summary>
        /// Populates the index with the provided documents. 
        /// Once the cancellation is requested by <paramref name="cancellationToken" />,
        /// <paramref name="onCancel" /> will be invoked with a new continuation token as a parameter.
        /// </summary>
        /// <param name="cultureInfo">The collection that has to be updated.</param>
        /// <param name="documents">The documents to be indexed.</param>
        /// <param name="customFields">The custom fields.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="onCancel">The action that's invoked when the cancellation is requested.</param>
        void IndexDocuments<TContinuationToken>(CultureInfo cultureInfo,
            IEnumerable<Tuple<SearchDocument, TContinuationToken>> documents,
            IReadOnlyCollection<DocumentField> customFields,
            CancellationToken cancellationToken,
            Action<TContinuationToken> onCancel);

        /// <summary>
        /// Deletes all the documents for the given culture.
        /// To be called when a culture is removed from the cms.
        /// </summary>
        /// <param name="culture">The culture.</param>
        void DropCollection(CultureInfo culture);


        /// <summary>
        /// Removes all the documents for the given culture.
        /// </summary>
        /// <param name="cultureInfo"></param>
        void RemoveDocuments(CultureInfo cultureInfo);

        /// <summary>
        /// Removes all the document populated from the given document source.
        /// </summary>
        /// <param name="documentSourceName"></param>
        /// <param name="cultureInfo">The culture.</param>
        void RemoveDocuments(CultureInfo cultureInfo, string documentSourceName);

        /// <summary>
        /// Adds a new document to the collection.
        /// </summary>
        /// <param name="cultureInfo">The culture name.</param>
        /// <param name="document">The document.</param>
        void AddDocument(CultureInfo cultureInfo, SearchDocument document, IReadOnlyCollection<DocumentField> customFields);

        /// <summary>
        /// Updates a new document in the collection.
        /// </summary>
        /// <param name="cultureInfo">The culture name.</param>
        /// <param name="document">The document.</param>
        void UpdateDocument(CultureInfo cultureInfo, SearchDocument document, IReadOnlyCollection<DocumentField> customFields);

        /// <summary>
        /// Removes a document from the collection.
        /// </summary>
        /// <param name="cultureInfo">The culture name.</param>
        /// <param name="documentId">The document identifier.</param>
        void RemoveDocument(CultureInfo cultureInfo, string documentId);
    }
}
