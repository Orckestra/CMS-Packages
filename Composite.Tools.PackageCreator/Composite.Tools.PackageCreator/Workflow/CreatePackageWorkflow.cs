using System;
using System.Linq;
using System.Workflow.Activities;
using System.Workflow.Runtime;
using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.C1Console.Workflow.Activities;
using Composite.Core.Serialization;
using Composite.Tools.PackageCreator.ElementProvider.Actions;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;

namespace Composite.Tools.PackageCreator
{
    public sealed partial class CreatePackageWorkflow : FormsWorkflow
    {
        public CreatePackageWorkflow()
        {
            InitializeComponent();
        }

        private void packageInfoCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            var package = PackageCreatorFacade.GetPackageInformation("New");
            if (!string.IsNullOrEmpty(Payload))
            {
                var dic = StringConversionServices.ParseKeyValueCollection(Payload);
                if (dic.ContainsKey("Name"))
                {
                    var name = StringConversionServices.DeserializeValueString(dic["Name"]);
                    package.Name = name;

                    if (dic.ContainsKey("GroupName"))
                    {
                        var readMoreUrl = StringConversionServices.DeserializeValueString(dic["ReadMoreUrl"]);
                        package.ReadMoreUrl = readMoreUrl;

                        var groupName = StringConversionServices.DeserializeValueString(dic["GroupName"]);
                        package.GroupName = groupName;
                    }
                    else
                    {
                        package.GroupName = name.Substring(0, name.LastIndexOf("."));
                    }
                }
            }

            if (!BindingExist("Package"))
                Bindings.Add("Package", package);
        }

        private void saveInfoCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            var package = GetBinding<PackageInformation>("Package");
            PackageCreatorFacade.SavePackageInformation(package);


            if (!string.IsNullOrEmpty(Payload))
            {
                var type = StringConversionServices.DeserializeValueString(
                    StringConversionServices.ParseKeyValueCollection(Payload)["ActionToken"]);
                var actionToken = ActionTokenSerializer.Deserialize(type);
                ActionExecutorFacade.Execute(package.GetEntityToken(), actionToken,
                    WorkflowFacade.GetFlowControllerServicesContainer(WorkflowEnvironment.WorkflowInstanceId));
            }
            else
            {
                ActionExecutorFacade.Execute(
                    package.GetEntityToken(),
                    new SetActivePackageActionToken(),
                    WorkflowFacade.GetFlowControllerServicesContainer(WorkflowEnvironment.WorkflowInstanceId));
            }

            var treeRefresher = CreateSpecificTreeRefresher();
            treeRefresher.PostRefreshMesseges(new PackageCreatorElementProviderEntityToken());
        }

        private void ValidateSave(object sender, ConditionalEventArgs e)
        {
            var package = GetBinding<PackageInformation>("Package");
            e.Result = true;
            if (PackageCreatorFacade.GetPackageNames().Contains(package.Name.Trim()))
            {
                ShowFieldMessage("Package.Name", "Package with this name already exists");
                e.Result = false;
            }

            if (string.IsNullOrEmpty(package.Description))
            {
                ShowFieldMessage("Package.Description", PackageCreatorFacade.GetLocalization("Required"));
                e.Result = false;
            }
        }
    }
}