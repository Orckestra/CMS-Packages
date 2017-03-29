namespace Orckestra.Search.Commands
{
    interface ILongRunningCommand
    {
        void Cancel();
        bool ShouldBePersistedOnShutdown();
    }
}
