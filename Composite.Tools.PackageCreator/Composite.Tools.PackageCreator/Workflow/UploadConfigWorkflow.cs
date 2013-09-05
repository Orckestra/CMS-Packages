using System;
using System.Collections.Generic;
using Composite.C1Console.Actions;
using Composite.C1Console.Forms.CoreUiControls;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;

namespace Composite.Tools.PackageCreator.Workflow
{
    public sealed partial class UploadConfigWorkflow : Composite.C1Console.Workflow.Activities.FormsWorkflow
    {
        public UploadConfigWorkflow()
        {
            InitializeComponent();
        }

        private void Upload_CodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            this.Bindings.Add("UploadedFile", new UploadedFile());
        }

        private void DidValidate(object sender, System.Workflow.Activities.ConditionalEventArgs e)
        {
            e.Result = this.BindingExist("Errors") == false;
        }

        private void AddConfig_CodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            try
            {
                UploadedFile uploadedFile = this.GetBinding<UploadedFile>("UploadedFile");
                PackageCreatorFacade.AddConfig(uploadedFile);
                SpecificTreeRefresher treeRefresher = this.CreateSpecificTreeRefresher();
                treeRefresher.PostRefreshMesseges(new PackageCreatorElementProviderEntityToken());
            }
            catch (Exception ex)
            {
                this.UpdateBinding("Errors", new List<List<string>> { new List<string> { ex.Message, "" } });
            }
        }
    }
}
