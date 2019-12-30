using Composite.Core.Application;

namespace Composite.Web.Js.TypeScript
{
    [ApplicationStartup]
    public static class StartupHandler
    {
        public static void OnBeforeInitialize()
        {
        }

        public static void OnInitialized()
        {
            //TypeScriptCompilerRegistry.RegisterCssCompiler(new SassCssCompiler());
        }
    };
}
