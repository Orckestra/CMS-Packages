using System.Collections.Generic;
using System.Text;
using Composite.C1Console.Actions;
using Composite.C1Console.Events;
using Composite.C1Console.Security;
using Composite.Core.Serialization;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens.Interfaces;
using Composite.Tools.PackageCreator.Types;

namespace Composite.Tools.PackageCreator.Actions
{
    [ActionExecutor(typeof(AddLocalizationActionExecutor))]
    internal class AddLocalizationActionToken : ActionToken
    {
        private static readonly IEnumerable<PermissionType> _permissionTypes = new[] { PermissionType.Administrate };

        public AddLocalizationActionToken(string cultureName)
        {
            CultureName = cultureName;
        }

        public override IEnumerable<PermissionType> PermissionTypes => _permissionTypes;

        public string CultureName { get; }

        public override string Serialize()
        {
            var sb = new StringBuilder();
            StringConversionServices.SerializeKeyValuePair<string>(sb, "_cultureName", CultureName);
            return sb.ToString();
        }

        public static ActionToken Deserialize(string serializedData)
        {
            var dic = StringConversionServices.ParseKeyValueCollection(serializedData);
            var cultureName = StringConversionServices.DeserializeValueString(dic["_cultureName"]);
            return new AddLocalizationActionToken(cultureName);
        }
    }

    internal class AddLocalizationActionExecutor : IActionExecutor
    {
        #region IActionExecutor Members

        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken,
            FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var packageName = PackageCreatorFacade.ActivePackageName;
            if (entityToken is PackageCreatorEntityToken creatorEntityToken) packageName = creatorEntityToken.Source;
            if (string.IsNullOrEmpty(packageName))
            {
                flowControllerServicesContainer.GetService<IManagementConsoleMessageService>();
                var consoleServices = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>();
                consoleServices.ShowMessage(DialogType.Warning,
                    PackageCreatorFacade.GetLocalization("NoPackages.Title"),
                    PackageCreatorFacade.GetLocalization("NoPackages.Message"));
                return null;
            }

            if (actionToken is AddLocalizationActionToken token)
                PackageCreatorFacade.AddItem(new PCLocalizations(token.CultureName), packageName);

            var treeRefresher = new SpecificTreeRefresher(flowControllerServicesContainer);
            treeRefresher.PostRefreshMesseges(new PackageCreatorElementProviderEntityToken());

            return null;
        }

        #endregion
    }
}