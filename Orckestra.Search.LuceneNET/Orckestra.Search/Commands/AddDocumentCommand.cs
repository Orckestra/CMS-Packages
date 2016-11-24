using System.Globalization;
using Composite.C1Console.Search;

namespace Orckestra.Search.Commands
{
    class AddDocumentCommand: IIndexUpdateCommand
    {
        public string Culture { get; set; }

        public SearchDocument Document { get; set; }

        public void Execute(ISearchIndex container)
        {
            var culture = CultureInfo.GetCultureInfo(Culture);
            container.AddDocument(culture, Document);
        }
    }
}
