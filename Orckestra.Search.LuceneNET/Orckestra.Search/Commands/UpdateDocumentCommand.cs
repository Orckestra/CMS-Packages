using System.Globalization;
using Composite.Search;

namespace Orckestra.Search.Commands
{
    class UpdateDocumentCommand: IIndexUpdateCommand
    {
        public string Culture { get; set; }

        public SearchDocument Document { get; set; }

        public void Execute(ISearchIndex container)
        {
            var culture = CultureInfo.GetCultureInfo(Culture);
            container.UpdateDocument(culture, Document);
        }
    }
}
