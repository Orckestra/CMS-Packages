using System;
using System.Linq;
using System.Web;
using System.Workflow.Activities;
using Composite.C1Console.Actions;
using Composite.Tools.PackageCreator.ElementProvider;

namespace Composite.Tools.PackageCreator.Workflow
{
	public sealed partial class EditPackageWorkflow : Composite.C1Console.Workflow.Activities.FormsWorkflow
	{
		public EditPackageWorkflow()
		{
			InitializeComponent();
		}

		private void InitCodeActivity_ExecuteCode(object sender, EventArgs e)
		{
			var package = PackageCreatorFacade.GetPackageInformation(this.EntityToken.Source);
			if (this.BindingExist("Package") == false)
				this.Bindings.Add("Package", package);

			if (this.BindingExist("MaxCompositeVersionSupported") == false)
				this.Bindings.Add("MaxCompositeVersionSupported", package.MaxCompositeVersionSupported.ToString());

			if (this.BindingExist("MinCompositeVersionSupported") == false)
				this.Bindings.Add("MinCompositeVersionSupported", package.MinCompositeVersionSupported.ToString());


			var request = HttpContext.Current.Request;
			if (this.BindingExist("PackageUrl") == false)
			{
				var packageUrl = string.Format("{0}/Composite/InstalledPackages/services/Composite.Tools.PackageCreator/GetPackage.ashx?package={1}", request.ApplicationPath == "/" ? "" : request.ApplicationPath, this.EntityToken.Source);
				this.Bindings.Add("PackageUrl", packageUrl);
			}
			if (this.BindingExist("ConfigUrl") == false)
			{
				var packageUrl = string.Format("{0}/Composite/InstalledPackages/services/Composite.Tools.PackageCreator/GetPackage.ashx?config={1}", request.ApplicationPath == "/" ? "" : request.ApplicationPath, this.EntityToken.Source);
				this.Bindings.Add("ConfigUrl", packageUrl);
			}

		}

		private void ValidateSave(object sender, ConditionalEventArgs e)
		{
			e.Result = true;
			var package = this.GetBinding<PackageInformation>("Package");
			var maxCompositeVersionSupported = this.GetBinding<string>("MaxCompositeVersionSupported");
			var minCompositeVersionSupported = this.GetBinding<string>("MinCompositeVersionSupported");
			try
			{
				package.MaxCompositeVersionSupported = new Version(maxCompositeVersionSupported);
			}
			catch
			{
				this.ShowFieldMessage("MaxCompositeVersionSupported", PackageCreatorFacade.GetLocalization("Error.WrongVersion"));
				e.Result = false;
			}

			try
			{
				package.MinCompositeVersionSupported = new Version(minCompositeVersionSupported);
			}
			catch
			{
				this.ShowFieldMessage("MinCompositeVersionSupported", PackageCreatorFacade.GetLocalization("Error.WrongVersion"));
				e.Result = false;
			}

			if (string.IsNullOrEmpty(package.Description))
			{
				this.ShowFieldMessage("Package.Description", PackageCreatorFacade.GetLocalization("Required"));
				e.Result = false;
			}

			if (string.IsNullOrEmpty(package.TechnicalDetails))
			{
				this.ShowFieldMessage("Package.TechicalDetails", PackageCreatorFacade.GetLocalization("Required"));
				e.Result = false;
			}

			if (this.EntityToken.Source != package.Name && PackageCreatorFacade.GetPackageNames().Contains(package.Name.Trim()))
			{
				this.ShowFieldMessage("Package.Name", PackageCreatorFacade.GetLocalization("Error.PackageAlreadyExist"));
				e.Result = false;
			}
			this.UpdateBinding("Package", package);

		}

		private void SaveCodeActivity_ExecuteCode(object sender, EventArgs e)
		{
			var package = this.GetBinding<PackageInformation>("Package");
			PackageCreatorFacade.SavePackageInformation(package, this.EntityToken.Source);
			SpecificTreeRefresher treeRefresher = this.CreateSpecificTreeRefresher();
			treeRefresher.PostRefreshMesseges(new PackageCreatorElementProviderEntityToken());

			this.UpdateBinding("PackageUrl", "123");

			this.SetSaveStatus(true);

		}
	}
}
