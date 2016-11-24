using System.Globalization;

namespace Orckestra.Search.Commands
{
    class RemoveDocumentCommand: IIndexUpdateCommand
    {
        public string Culture { get; set; }

        public string DocumentId { get; set; }

        public void Execute(ISearchIndex container)
        {
            var culture = CultureInfo.GetCultureInfo(Culture);
            container.RemoveDocument(culture, DocumentId);
        }
    }
}
