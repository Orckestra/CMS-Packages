using System.Collections.Generic;
using System.Text;
using Composite.C1Console.Actions;
using Composite.C1Console.Events;
using Composite.C1Console.Security;
using Composite.Core.Serialization;
using Composite.Core.WebClient;

namespace Composite.Tools.PackageCreator.ElementProvider.Actions
{
    [ActionExecutor(typeof(DownloadPackageElementProviderActionExecutor))]
    public sealed class DownloadPackageActionToken : ActionToken
    {
        static private readonly IEnumerable<PermissionType> _permissionTypes = new[] { PermissionType.Administrate };

        public DownloadPackageActionToken(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public override IEnumerable<PermissionType> PermissionTypes
        {
            get { return _permissionTypes; }
        }

        public override string Serialize()
        {
            var sb = new StringBuilder();

            StringConversionServices.SerializeKeyValuePair<string>(sb, "name", Name);

            return sb.ToString();
        }

        public static ActionToken Deserialize(string serializedData)
        {
            var dic = StringConversionServices.ParseKeyValueCollection(serializedData);

            var name = StringConversionServices.DeserializeValueString(dic["name"]);

            return new DownloadPackageActionToken(name);
        }
    }

    public sealed class DownloadPackageElementProviderActionExecutor : IActionExecutor
    {

        public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
        {
            var token = actionToken as DownloadPackageActionToken;
            if (token != null)
            {
                var currentConsoleId = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>().CurrentConsoleId;
                var packageName = entityToken.Source;

                var url = UrlUtils.ResolveAdminUrl(
                    string.Format(@"InstalledPackages/services/Composite.Tools.PackageCreator/GetPackage.ashx?{0}={1}&consoleId={2}", token.Name, packageName, currentConsoleId));

                ConsoleMessageQueueFacade.Enqueue(new DownloadFileMessageQueueItem(url), currentConsoleId);
            }
            return null;
        }
    }
}
