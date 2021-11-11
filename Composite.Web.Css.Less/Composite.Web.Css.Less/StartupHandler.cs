using System.Web.Hosting;
using Composite.Core.Application;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Web.Css.CompileFoundation;

namespace Orckestra.Web.Css.Less
{
    public static class StartupHandler
    {
        [ApplicationStartup]
        public static class StatupHandler
        {
            public static void OnBeforeInitialize() { }

            public static void ConfigureServices(IServiceCollection services)
            {
                if (!HostingEnvironment.IsHosted) return;

                services.AddSingleton(typeof(ICssCompiler), typeof(LessCssCompiler));
            }
        }
    }
}
