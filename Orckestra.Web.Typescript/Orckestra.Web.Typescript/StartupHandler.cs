using Composite.Core;
using Composite.Core.Application;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Web.Typescript.Classes;
using Orckestra.Web.Typescript.Classes.Models;
using Orckestra.Web.Typescript.Enums;
using Orckestra.Web.Typescript.Interfaces;
using Orckestra.Web.Typescript.Services;
using System.Configuration;
using System.Web.Hosting;

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
            if (ConfigurationManager.AppSettings["Orckestra.Web.Typescript.Enable"] != "true")
            {
                return;
            }

            string baseDirPath = HostingEnvironment.MapPath("~");
            Settings settings = Helper.GetSettings();
            if (settings.TypescriptTasks is null)
            {
                return;
            }

            foreach (TypescriptTask el in settings.TypescriptTasks)
            {
                ITypescriptCompileService compilerService = ServiceLocator
                    .GetService<ITypescriptCompileService>()
                    .ConfigureService(
                    baseDirPath, 
                    settings.CompilerTimeOutSeconds, 
                    el.PathTypescriptConfigFile, 
                    el.CancelIfOutFileExist,
                    el.Minification?.UseMinification, 
                    el.Minification?.MinifiedFileName)
                    .InvokeService();

                if (el.Mode == Mode.Dynamic)
                {
                    ServiceLocator
                        .GetService<ITypescriptWatcherService>()
                        .ConfigureService(() => compilerService.InvokeService(), el.FileMask, el.PathsToWatch)
                        .InvokeService();
                }
            }
        }
    }
}
