namespace Orckestra.Search.Commands
{
    class RebuildIndexCommand : IIndexUpdateCommand
    {
        public void Execute(ISearchIndex container)
        {
            container.RebuildAll();
        }
    }
}
