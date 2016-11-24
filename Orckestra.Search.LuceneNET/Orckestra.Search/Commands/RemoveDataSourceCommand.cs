namespace Orckestra.Search.Commands
{
    public class RemoveDataSourceCommand : IIndexUpdateCommand
    {
        public string DocumentSourceName { get; set; }

        public void Execute(ISearchIndex container)
        {
            container.DeleteDocumentsBySource(DocumentSourceName);
        }
    }
}
