using System.Globalization;

namespace Orckestra.Search.Commands
{
    class PopulateCollectionCommand: IIndexUpdateCommand
    {
        public string Culture { get; set; }

        public void Execute(CommandContext context)
        {
            var culture = CultureInfo.GetCultureInfo(Culture);

            context.Index.RemoveDocuments(culture);

            foreach (var dataSource in context.DocumentSources)
            {
                CommandQueue.Queue(new PopulateFromDataSourceCommand
                {
                    DocumentSourceName = dataSource.Name,
                    CultureName = Culture,
                    ContinuationToken = null
                });
            }
        }
    }
}
