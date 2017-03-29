namespace Orckestra.Search.Commands
{
    interface ILongRunningCommand
    {
        bool ShouldBePersistedOnShutdown();
    }
}
