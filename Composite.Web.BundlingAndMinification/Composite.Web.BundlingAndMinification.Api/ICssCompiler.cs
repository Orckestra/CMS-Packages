namespace Composite.Web.BundlingAndMinification.Api
{
    public interface ICssCompiler
    {
        bool SupportsExtension(string extension);
        string CompileCss(string sourceFilePath);
    }
}
