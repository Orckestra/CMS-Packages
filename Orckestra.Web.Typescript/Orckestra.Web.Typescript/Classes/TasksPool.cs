using System;
using Orckestra.Web.Typescript.Interfaces;
using System.Collections.Generic;

namespace Orckestra.Web.Typescript.Classes
{
    internal static class TasksPool
    {
        private static ITypescriptCompileService[] _compilers = Array.Empty<ITypescriptCompileService>();
        #pragma warning disable IDE0052
        private static List<ITypescriptWatcherService> _watchers;
        #pragma warning restore IDE0052
        internal static void Register(List<ITypescriptCompileService> compileServices, List<ITypescriptWatcherService> watcherServices)
        {
            _compilers = compileServices.ToArray();
            _watchers = new List<ITypescriptWatcherService>(watcherServices);
        }

        internal static void CheckSourcesChanges()
        {
            foreach (ITypescriptCompileService el in _compilers)
            {
                if (el.IsSourceChanged())
                {
                    el.InvokeService();
                }
            }
        }
    }
}