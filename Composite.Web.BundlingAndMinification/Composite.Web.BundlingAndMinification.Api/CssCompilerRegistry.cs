using System.Collections.Generic;

namespace Composite.Web.BundlingAndMinification.Api
{
    public static class CssCompilerRegistry
    {

        public static List<ICssCompiler> _compilers = new List<ICssCompiler>();

        public static void RegisterCssCompiler(ICssCompiler compiler) => _compilers.Add(compiler);

        public static IEnumerable<ICssCompiler> GetCssCompilers() => _compilers;
    }
}
