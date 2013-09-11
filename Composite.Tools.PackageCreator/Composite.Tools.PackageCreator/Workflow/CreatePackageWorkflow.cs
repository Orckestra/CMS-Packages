using System;
using System.Collections.Generic;
using System.Linq;
using System.Workflow.Activities;
using System.Workflow.Runtime;
using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.C1Console.Users;
using Composite.C1Console.Workflow;
using Composite.Core.Serialization;
using Composite.Tools.PackageCreator.ElementProvider.Actions;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;

namespace Composite.Tools.PackageCreator
{
    public sealed partial class CreatePackageWorkflow : Composite.C1Console.Workflow.Activities.FormsWorkflow
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
                Dictionary<string, string> dic = StringConversionServices.ParseKeyValueCollection(Payload);
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

            if (this.BindingExist("Package") == false)
                this.Bindings.Add("Package", package);

        }

        private void saveInfoCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            var package = this.GetBinding<PackageInformation>("Package");
            PackageCreatorFacade.SavePackageInformation(package);


            if (!string.IsNullOrEmpty(Payload))
            {

                var type = StringConversionServices.DeserializeValueString(StringConversionServices.ParseKeyValueCollection(Payload)["ActionToken"]);
                ActionToken actionToken = ActionTokenSerializer.Deserialize(type);
                ActionExecutorFacade.Execute(package.GetEntityToken(), actionToken, WorkflowFacade.GetFlowControllerServicesContainer(WorkflowEnvironment.WorkflowInstanceId));

            }
            else
            {
                ActionExecutorFacade.Execute(
                    package.GetEntityToken(),
                    new SetActivePackageActionToken(),
                    WorkflowFacade.GetFlowControllerServicesContainer(WorkflowEnvironment.WorkflowInstanceId));
            }

            SpecificTreeRefresher treeRefresher = this.CreateSpecificTreeRefresher();
            treeRefresher.PostRefreshMesseges(new PackageCreatorElementProviderEntityToken());
        }

        private void ValidateSave(object sender, ConditionalEventArgs e)
        {
            var package = this.GetBinding<PackageInformation>("Package");
            e.Result = true;
            if (PackageCreatorFacade.GetPackageNames().Contains(package.Name.Trim()))
            {
                this.ShowFieldMessage("Package.Name", "Package with this name already exists");
                e.Result = false;
            }
            if (string.IsNullOrEmpty(package.Description))
            {
                this.ShowFieldMessage("Package.Description", PackageCreatorFacade.GetLocalization("Required"));
                e.Result = false;
            }

        }

    }

}
