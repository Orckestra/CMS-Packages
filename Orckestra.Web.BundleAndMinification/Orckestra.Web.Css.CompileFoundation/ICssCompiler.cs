namespace Orckestra.Web.Css.CompileFoundation
{
    /* Can we put this interface to C1? It used in 3 diff.modules: LessCompiler, SassCompiler, BundlingAndMinification */
    public interface ICssCompiler
    {
        bool SupportsExtension(string extension);
        string CompileCss(string sourceFilePath);
    }
}