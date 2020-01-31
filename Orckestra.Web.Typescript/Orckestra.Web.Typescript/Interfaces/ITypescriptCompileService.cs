namespace Orckestra.Web.Typescript.Interfaces
{
    public interface ITypescriptCompileService : ITypescriptService
    {
        void ConfigureService(
            string taskName,
            string baseDirPath, 
            int compilerTimeOutSeconds, 
            string pathConfigFile, 
            bool allowOverwrite,
            bool useMinification, 
            string minifiedName);
    }
}