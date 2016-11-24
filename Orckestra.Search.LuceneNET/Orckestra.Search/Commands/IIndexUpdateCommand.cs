namespace Orckestra.Search.Commands
{
    /// <summary>
    /// Represents a command for updating index, has to be serializable to be put into the command queue.
    /// </summary>
    interface IIndexUpdateCommand
    {
        void Execute(ISearchIndex index);
    }
}
