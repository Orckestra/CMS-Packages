namespace Orckestra.Web.Typescript.Interfaces
{
    public interface ITypescriptCompileService
    {
        bool InvokeService();
        bool ConfigureService(string taskName, string baseDirPath, int compilerTimeOutSeconds, string pathConfigFile, bool allowOverwrite, bool useMinification, string minifiedName);
        bool IsSourceChanged();
        void SetSourceChanged();
        void SetSourceLast();
    }
}