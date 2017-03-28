using System.Globalization;

namespace Orckestra.Search.Commands
{
    /// <summary>
    /// Command that's executed when a culture is removed.
    /// </summary>
    class DropCollectionCommand: IIndexUpdateCommand
    {
        public string Culture { get; set; }

        public void Execute(CommandContext context)
        {
            var culture = CultureInfo.GetCultureInfo(Culture);
            context.Index.DropCollection(culture);
        }
    }
}
