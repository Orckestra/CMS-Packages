using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Composite.Core.PackageSystem;
using Composite.C1Console.Users;
using System.Web;
using Composite.Core.PackageSystem.Foundation;
using System.IO;
using Composite.C1Console.Actions;
using Composite.Tools.PackageCreator.ElementProvider;
using System.Collections.Generic;
using Composite.Core.Serialization;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;

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
				if(dic.ContainsKey("Name"))
				{
					var name = StringConversionServices.DeserializeValueString(dic["Name"]);
					package.Name = name;
					package.GroupName = name.Substring(0, name.LastIndexOf("."));
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
