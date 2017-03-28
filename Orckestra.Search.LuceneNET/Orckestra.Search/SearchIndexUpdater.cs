using System.Globalization;
using Composite.Core;
using Composite.Data;
using Composite.Search;
using Orckestra.Search.Commands;

namespace Orckestra.Search
{
    internal class SearchIndexUpdater : ISearchIndexUpdater, IDocumentSourceListener
    {
        private const string LogTitle = nameof(SearchIndexUpdater);

        public void Rebuild()
        {
            CommandQueue.CancelCurrentTasks();

            CommandQueue.ClearCommands();

            Log.LogInformation(LogTitle, "Rebuilding the search index");

            foreach (var culture in DataLocalizationFacade.ActiveLocalizationCultures)
            {
                CommandQueue.Queue(new PopulateCollectionCommand
                {
                    Culture = culture.Name
                });
            }
        }

        public void Populate(string dataSource)
        {
            CommandQueue.Queue(new PopulateFromDataSourceCommand { DocumentSourceName = dataSource });
        }

        public void Remove(string dataSource)
        {
            CommandQueue.Queue(new RemoveDataSourceCommand { DocumentSourceName = dataSource });
        }

        public void CreateCollection(CultureInfo cultureInfo)
        {
            CommandQueue.Queue(new PopulateCollectionCommand { Culture = cultureInfo.Name});
        }

        public void DropCollection(CultureInfo cultureInfo)
        {
            CommandQueue.Queue(new DropCollectionCommand { Culture = cultureInfo.Name });
        }

        public void Create(CultureInfo cultureInfo, SearchDocument document)
        {
            CommandQueue.Queue(new AddDocumentCommand {Culture = cultureInfo.Name, Document = document});
        }

        public void Update(CultureInfo cultureInfo, SearchDocument document)
        {
            CommandQueue.Queue(new UpdateDocumentCommand { Culture = cultureInfo.Name, Document = document });
        }

        public void Delete(CultureInfo cultureInfo, string documentId)
        {
            CommandQueue.Queue(new RemoveDocumentCommand { Culture = cultureInfo.Name, DocumentId = documentId });
        }

        public void Rebuild(CultureInfo cultureInfo, string source)
        {
            Remove(source);
            Populate(source);
        }

        public void StopProcessingUpdates()
        {
            CommandQueue.StopProcessingUpdates();
        }
    }
}
