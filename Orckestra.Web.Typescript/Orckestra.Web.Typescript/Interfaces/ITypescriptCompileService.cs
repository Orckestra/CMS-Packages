
namespace Orckestra.Web.Typescript.Interfaces
{
    public interface ITypescriptCompileService
    {
        ITypescriptCompileService ConfigureService(
            string baseDirPath, 
            int compilerTimeOutSeconds, 
            string pathConfigFile, 
            bool cancelIfOutFileExist,
            bool? useMinification, 
            string minifiedName);
        ITypescriptCompileService InvokeService();
    }
}