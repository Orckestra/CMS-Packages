using System;
using System.Collections.Generic;
using System.Workflow.Activities;
using Composite.C1Console.Forms.CoreUiControls;
using Composite.C1Console.Workflow.Activities;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;

namespace Composite.Tools.PackageCreator.Workflow
{
    public sealed partial class UploadConfigWorkflow : FormsWorkflow
    {
        public UploadConfigWorkflow()
        {
            InitializeComponent();
        }

        private void Upload_CodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            Bindings.Add("UploadedFile", new UploadedFile());
        }

        private void DidValidate(object sender, ConditionalEventArgs e)
        {
            e.Result = !BindingExist("Errors");
        }

        private void AddConfig_CodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            try
            {
                var uploadedFile = GetBinding<UploadedFile>("UploadedFile");
                PackageCreatorFacade.AddConfig(uploadedFile);
                var treeRefresher = CreateSpecificTreeRefresher();
                treeRefresher.PostRefreshMesseges(new PackageCreatorElementProviderEntityToken());
            }
            catch (Exception ex)
            {
                UpdateBinding("Errors", new List<List<string>> { new List<string> { ex.Message, "" } });
            }
        }
    }
}