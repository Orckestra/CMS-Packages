using System;
using System.Linq;
using System.Web;
using System.Workflow.Activities;
using Composite.C1Console.Workflow.Activities;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;

namespace Composite.Tools.PackageCreator.Workflow
{
    public sealed partial class EditPackageWorkflow : FormsWorkflow
    {
        public EditPackageWorkflow()
        {
            InitializeComponent();
        }

        private void InitCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            var package = PackageCreatorFacade.GetPackageInformation(EntityToken.Source);
            if (!BindingExist("Package"))
                Bindings.Add("Package", package);

            if (!BindingExist("MaxCompositeVersionSupported"))
                Bindings.Add("MaxCompositeVersionSupported", package.MaxCompositeVersionSupported.ToString());

            if (!BindingExist("MinCompositeVersionSupported"))
                Bindings.Add("MinCompositeVersionSupported", package.MinCompositeVersionSupported.ToString());

            var request = HttpContext.Current.Request;
            if (!BindingExist("PackageUrl"))
            {
                var packageUrl =
                    string.Format(
                        "{0}/Composite/InstalledPackages/services/Composite.Tools.PackageCreator/GetPackage.ashx?package={1}",
                        request.ApplicationPath == "/" ? "" : request.ApplicationPath, EntityToken.Source);
                Bindings.Add("PackageUrl", packageUrl);
            }

            if (!BindingExist("ConfigUrl"))
            {
                var packageUrl =
                    string.Format(
                        "{0}/Composite/InstalledPackages/services/Composite.Tools.PackageCreator/GetPackage.ashx?config={1}",
                        request.ApplicationPath == "/" ? "" : request.ApplicationPath, EntityToken.Source);
                Bindings.Add("ConfigUrl", packageUrl);
            }
        }

        private void ValidateSave(object sender, ConditionalEventArgs e)
        {
            e.Result = true;
            var package = GetBinding<PackageInformation>("Package");
            var maxCompositeVersionSupported = GetBinding<string>("MaxCompositeVersionSupported");
            var minCompositeVersionSupported = GetBinding<string>("MinCompositeVersionSupported");
            try
            {
                package.MaxCompositeVersionSupported = new Version(maxCompositeVersionSupported);
            }
            catch
            {
                ShowFieldMessage("MaxCompositeVersionSupported",
                    PackageCreatorFacade.GetLocalization("Error.WrongVersion"));
                e.Result = false;
            }

            try
            {
                package.MinCompositeVersionSupported = new Version(minCompositeVersionSupported);
            }
            catch
            {
                ShowFieldMessage("MinCompositeVersionSupported",
                    PackageCreatorFacade.GetLocalization("Error.WrongVersion"));
                e.Result = false;
            }

            if (string.IsNullOrEmpty(package.Description))
            {
                ShowFieldMessage("Package.Description", PackageCreatorFacade.GetLocalization("Required"));
                e.Result = false;
            }

            if (string.IsNullOrEmpty(package.TechnicalDetails))
            {
                ShowFieldMessage("Package.TechnicalDetails", PackageCreatorFacade.GetLocalization("Required"));
                e.Result = false;
            }

            if (EntityToken.Source != package.Name &&
                PackageCreatorFacade.GetPackageNames().Contains(package.Name.Trim()))
            {
                ShowFieldMessage("Package.Name", PackageCreatorFacade.GetLocalization("Error.PackageAlreadyExist"));
                e.Result = false;
            }

            UpdateBinding("Package", package);
        }

        private void SaveCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            var package = GetBinding<PackageInformation>("Package");
            PackageCreatorFacade.SavePackageInformation(package, EntityToken.Source);
            var treeRefresher = CreateSpecificTreeRefresher();
            treeRefresher.PostRefreshMesseges(new PackageCreatorElementProviderEntityToken());

            UpdateBinding("PackageUrl", "123");

            SetSaveStatus(true);
        }
    }
}