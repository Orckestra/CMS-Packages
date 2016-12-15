namespace Composite.Web.Css.Sass
{
    public class SassHttpModule : CssCompilationHttpModule
    {
        public SassHttpModule(): base(".scss", "*.scss", SassCssCompiler.CompileCss)
        {
        }
    }
}