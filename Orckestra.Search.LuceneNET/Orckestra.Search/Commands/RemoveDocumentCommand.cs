using System.Globalization;

namespace Orckestra.Search.Commands
{
    class RemoveDocumentCommand: IIndexUpdateCommand
    {
        public string Culture { get; set; }

        public string DocumentId { get; set; }

        public void Execute(CommandContext context)
        {
            var culture = CultureInfo.GetCultureInfo(Culture);
            context.Index.RemoveDocument(culture, DocumentId);
        }
    }
}
