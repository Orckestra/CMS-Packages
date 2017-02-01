namespace Orckestra.Web.Css.Less
{
    public class LessHttpModule : CssCompilationHttpModule
    {
        public LessHttpModule() : base(".less", "*.less", LessCssCompiler.CompileCss)
        {
        }
    }
}