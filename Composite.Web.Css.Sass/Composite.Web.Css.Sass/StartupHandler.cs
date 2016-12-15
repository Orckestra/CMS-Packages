using Composite.Core.Application;
using Composite.Web.BundlingAndMinification.Api;

namespace Composite.Web.Css.Sass
{
    public static class StartupHandler
    {
        [ApplicationStartup]
        public static class StatupHandler
        {
            public static void OnBeforeInitialize() { }

            public static void OnInitialized()
            {
                CssCompilerRegistry.RegisterCssCompiler(new SassCssCompiler());
            }
        }
    }
}
