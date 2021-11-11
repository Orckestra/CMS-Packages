using Composite.Core.Application;
using Microsoft.Extensions.DependencyInjection;
using Orckestra.Web.Css.CompileFoundation;

namespace Orckestra.Web.Css.Sass
{
    public static class StartupHandler
    {
        [ApplicationStartup]
        public static class StatupHandler
        {
            public static void OnBeforeInitialize() { }

            public static void ConfigureServices(IServiceCollection services)
            {
                services.AddSingleton(typeof(ICssCompiler), typeof(SassCssCompiler));
            }
        }
    }
}
