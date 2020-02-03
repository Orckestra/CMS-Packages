using Orckestra.Web.Typescript.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Orckestra.Web.Typescript.Classes
{
    internal static class TasksPool
    {
        private static readonly List<ITypescriptCompileService> _compilers = new List<ITypescriptCompileService>();
        private static readonly List<ITypescriptWatcherService> _watchers = new List<ITypescriptWatcherService>();
        internal static void Register(ITypescriptCompileService compileService, ITypescriptWatcherService watcherService)
        {
            _compilers.Add(compileService);
            _watchers.Add(watcherService);
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