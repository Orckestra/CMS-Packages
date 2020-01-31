namespace Orckestra.Web.Typescript.Interfaces
{
    public interface ITypescriptService
    {
        void InvokeService();

        bool IsConfigured();

        bool IsInvoked();

        void ResetInvokeState();
    }
}
