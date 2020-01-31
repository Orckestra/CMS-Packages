using Orckestra.Web.Typescript.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Orckestra.Web.Typescript.Classes
{
    internal static class TasksPool
    {
        private static readonly ReaderWriterLockSlim _tasksLock = new ReaderWriterLockSlim();

        private static readonly List<ITypescriptCompileService> _compilers = new List<ITypescriptCompileService>();
        private static readonly List<ITypescriptWatcherService> _watchers = new List<ITypescriptWatcherService>();
        internal static void Register(ITypescriptCompileService compileService, ITypescriptWatcherService watcherService)
        {
            _tasksLock.EnterWriteLock();
            _compilers.Add(compileService);
            _watchers.Add(watcherService);
            _tasksLock.ExitWriteLock();
        }

        internal static void CheckSourcesChanges()
        {
            List<ITypescriptCompileService> result;
            try
            {
                _tasksLock.EnterReadLock();
                result = _compilers.Where(x => x.IsSourceChanged()).ToList();
            }
            finally
            {
                _tasksLock.ExitReadLock();
            }

            if (result.Any())
            {
                try
                {
                    _tasksLock.EnterWriteLock();
                    foreach (ITypescriptCompileService el in result)
                    {
                        if (el.IsSourceChanged())
                        {
                            el.InvokeService();
                        }
                    }
                }
                finally
                {
                    _tasksLock.ExitWriteLock();
                }
            }
        }
    }
}