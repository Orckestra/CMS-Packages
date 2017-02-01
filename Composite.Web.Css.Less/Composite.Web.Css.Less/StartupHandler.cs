using Composite.Core.Application;
using Composite.Web.BundlingAndMinification.Api;

namespace Orckestra.Web.Css.Less
{
    public static class StartupHandler
    {
        [ApplicationStartup]
        public static class StatupHandler
        {
            public static void OnBeforeInitialize() { }

            public static void OnInitialized()
            {
                CssCompilerRegistry.RegisterCssCompiler(new LessCssCompiler());
            }
        }
    }
}
