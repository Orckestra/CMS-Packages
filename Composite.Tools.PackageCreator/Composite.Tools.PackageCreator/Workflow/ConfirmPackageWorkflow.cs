using System;
using System.Workflow.Runtime;
using Composite.C1Console.Actions;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.C1Console.Workflow.Activities;
using Composite.Core.Serialization;

namespace Composite.Tools.PackageCreator.Workflow
{
    public sealed partial class ConfirmPackageWorkflow : FormsWorkflow
    {
        public ConfirmPackageWorkflow()
        {
            InitializeComponent();
        }

        private void codeActivity_ExecuteCode(object sender, EventArgs e)
        {
            var type = StringConversionServices.DeserializeValueType(
                StringConversionServices.ParseKeyValueCollection(Payload)["ActionToken"]);
            var actionToken = (ActionToken)Activator.CreateInstance(type);
            ActionExecutorFacade.Execute(EntityToken, actionToken,
                WorkflowFacade.GetFlowControllerServicesContainer(WorkflowEnvironment.WorkflowInstanceId));
        }

        private void intiCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            Bindings.Add("ConfirmMessage",
                StringConversionServices.DeserializeValueString(
                    StringConversionServices.ParseKeyValueCollection(Payload)["ConfirmMessage"]));
        }
    }
}