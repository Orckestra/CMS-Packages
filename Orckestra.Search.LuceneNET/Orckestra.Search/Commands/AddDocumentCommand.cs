using System.Globalization;
using System.Linq;
using Composite.Search;

namespace Orckestra.Search.Commands
{
    class AddDocumentCommand: IIndexUpdateCommand
    {
        public string Culture { get; set; }

        public SearchDocument Document { get; set; }

        public void Execute(CommandContext context)
        {
            var culture = CultureInfo.GetCultureInfo(Culture);

            var sourceName = Document.Source;
            var documentSource = context.DocumentSources.FirstOrDefault(ds => ds.Name == sourceName);
            if (documentSource == null) return;

            context.Index.AddDocument(culture, Document, documentSource.CustomFields);
        }
    }
}
