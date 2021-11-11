using Composite.Core;
using Composite.Core.Application;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Web.Typescript.Classes;
using Orckestra.Web.Typescript.Classes.Models;
using Orckestra.Web.Typescript.Classes.Services;
using Orckestra.Web.Typescript.Interfaces;
using System;
using System.Collections.Generic;
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
            if (!HostingEnvironment.IsHosted) return;

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
                RegisterException("No typescript tasks in assembly config file.", typeof(ArgumentException));
                return;
            }
            string baseDirPath = HostingEnvironment.MapPath("~");

            List<ITypescriptWatcherService> wsl = new List<ITypescriptWatcherService>();
            List<ITypescriptCompileService> csl = new List<ITypescriptCompileService>();
            foreach (TypescriptTask el in settings.TypescriptTasks)
            {
                ITypescriptCompileService compileService = ServiceLocator.GetService<ITypescriptCompileService>();

                bool operationResult = compileService.ConfigureService(
                    el.TaskName,
                    baseDirPath,
                    el.CompilerTimeOutSeconds,
                    el.PathTypescriptConfigFile,
                    el.AllowOverwrite,
                    el.UseMinification,
                    el.MinifiedFileName);

                if (!operationResult)
                {
                    return;
                }

                ITypescriptWatcherService watcherService = ServiceLocator.GetService<ITypescriptWatcherService>();

                operationResult = watcherService.ConfigureService(el.TaskName, () => compileService.SetSourceChanged(), el.FileMask, el.PathsToWatchForChanges);
                if (!operationResult)
                {
                    return;
                }

                operationResult = watcherService.InvokeService();
                if (!operationResult)
                {
                    return;
                }
                wsl.Add(watcherService);
                csl.Add(compileService);
            }

            TasksPool.Register(csl, wsl);
        }
    }
}