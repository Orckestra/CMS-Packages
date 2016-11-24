namespace Orckestra.Search.Commands
{
    public class PopulateFromDataSourceCommand : IIndexUpdateCommand
    {
        public string DocumentSourceName { get; set; }

        public void Execute(ISearchIndex container)
        {
            container.PopulateDocumentsFromSource(DocumentSourceName);
        }
    }
}
