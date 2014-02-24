namespace Composite.Web.Css.Less
{
    public class LessHttpModule : CssCompilationHttpModule
    {
        public LessHttpModule() : base(".less", "*.less", CompressFiles.CompressLess)
        {
        }
    }
}