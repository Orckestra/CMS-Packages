using Orckestra.Web.Typescript.Classes.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Orckestra.Web.Typescript.Classes
{
    internal static class TasksPool
    {
        private static readonly ReaderWriterLockSlim _tasksLock = new ReaderWriterLockSlim();

        private static readonly List<TypescriptTask> _tasks = new List<TypescriptTask>();
        internal static void Register(TypescriptTask task)
        {
            _tasksLock.EnterWriteLock();
             _tasks.Add(task);
            _tasksLock.ExitWriteLock();
        }
        
        internal static void Remove(TypescriptTask task)
        {
            _tasksLock.EnterWriteLock();
            _tasks.Remove(task);
            _tasksLock.ExitWriteLock();
        }

        internal static void CheckProcessTasks()
        {
            List<TypescriptTask> result;
            try
            {
                _tasksLock.EnterReadLock();
                result = _tasks.Where(x => !x.CompilerService.IsInvoked() || !x.WatcherService.IsInvoked()).ToList();
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
                    foreach (TypescriptTask el in result)
                    {
                        if (!el.CompilerService.IsInvoked())
                        {
                            el.CompilerService.InvokeService();
                        }
                        if (el.CompilerService.IsInvoked() && !el.WatcherService.IsInvoked())
                        {
                            el.WatcherService.InvokeService();
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
