using Composite.Data;

namespace Orckestra.Search.Commands
{
    class RemoveDataSourceCommand : IIndexUpdateCommand
    {
        public string DocumentSourceName { get; set; }

        public void Execute(CommandContext context)
        {
            foreach (var culture in DataLocalizationFacade.ActiveLocalizationCultures)
            {
                context.Index.RemoveDocuments(culture, DocumentSourceName);
            }
        }
    }
}
