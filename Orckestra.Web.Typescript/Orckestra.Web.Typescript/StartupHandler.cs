using Composite.Core;
using Composite.Core.Application;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Web.Typescript.Classes;
using Orckestra.Web.Typescript.Classes.Models;
using Orckestra.Web.Typescript.Classes.Services;
using Orckestra.Web.Typescript.Interfaces;
using System;
using System.Linq;
using System.Web.Hosting;
using static Orckestra.Web.Typescript.Classes.Helper;

namespace Orckestra.Web.Typescript
{
    [ApplicationStartup(AbortStartupOnException = true)]
    public static class StartupHandler
    {
        public static void OnBeforeInitialize() { }

        public static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ITypescriptCompileService, TypescriptCompileService>();
            serviceCollection.AddTransient<ITypescriptWatcherService, TypescriptWatcherService>();
        }

        public static void OnInitialized()
        {
            if (!PackageEnabled)
            {
                return;
            }

            Settings settings;
            try
            {
                settings = GetSettings();
            }
            catch (Exception ex)
            {
                RegisterException(ex);
                return;
            }

            if (settings.TypescriptTasks is null || !settings.TypescriptTasks.Any())
            {
                RegisterException("No typescript tasks in assembly config file", typeof(ArgumentException));
                return;
            }

            string baseDirPath = HostingEnvironment.MapPath("~");
            foreach (TypescriptTask el in settings.TypescriptTasks)
            {
                ITypescriptCompileService compilerService = ServiceLocator.GetService<ITypescriptCompileService>();
                el.CompilerService = compilerService;

                ITypescriptWatcherService watcherService = ServiceLocator.GetService<ITypescriptWatcherService>();
                el.WatcherService = watcherService;

                TasksPool.Register(el);

                compilerService.ConfigureService(
                    el.TaskName,
                    baseDirPath,
                    el.CompilerTimeOutSeconds,
                    el.PathTypescriptConfigFile,
                    el.AllowOverwrite,
                    el.UseMinification,
                    el.MinifiedFileName);

                if (!compilerService.IsConfigured())
                {
                    TasksPool.Remove(el);
                    return;
                }

                compilerService.InvokeService();

                if (!compilerService.IsInvoked())
                {
                    return;
                }

                watcherService.ConfigureService(el.TaskName, () => el.CompilerService.ResetInvokeState(), el.FileMask, el.PathsToWatchForChanges);
                if (!watcherService.IsConfigured())
                {
                    TasksPool.Remove(el);
                    return;
                }
                watcherService.InvokeService();
                if (!watcherService.IsInvoked())
                {
                    watcherService.ResetInvokeState();
                    return;
                }   
            }
        }
    }
}