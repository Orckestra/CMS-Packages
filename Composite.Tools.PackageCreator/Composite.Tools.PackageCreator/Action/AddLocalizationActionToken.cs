using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.C1Console.Actions;
using Composite.C1Console.Events;
using Composite.C1Console.Security;
using Composite.Core.Serialization;
using Composite.Tools.PackageCreator.ElementProvider;
using Composite.Tools.PackageCreator.Types;

namespace Composite.Tools.PackageCreator
{
	[ActionExecutor(typeof(AddLocalizationActionExecutor))] 
	internal class AddLocalizationActionToken : ActionToken
	{
		static private IEnumerable<PermissionType> _permissionTypes = new PermissionType[] { PermissionType.Administrate};

		public override IEnumerable<PermissionType> PermissionTypes
		{
			get { return _permissionTypes; }
		}

		public AddLocalizationActionToken(string cultureName)
		{
			CultureName = cultureName;
		}


		public override string Serialize()
		{
			var sb = new StringBuilder();
			StringConversionServices.SerializeKeyValuePair<string>(sb, "_cultureName", CultureName);
			return sb.ToString();
		}



		public string CultureName { get; private set; }


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

		public FlowToken Execute(EntityToken entityToken, ActionToken actionToken, FlowControllerServicesContainer flowControllerServicesContainer)
		{
			var packageName = PackageCreatorFacade.ActivePackageName;
			if (entityToken is PackageCreatorEntityToken)
			{
				packageName = (entityToken as PackageCreatorEntityToken).Source;
			}
			if (string.IsNullOrEmpty(packageName))
			{
				flowControllerServicesContainer.GetService<IManagementConsoleMessageService>();
				var consoleServices = flowControllerServicesContainer.GetService<IManagementConsoleMessageService>();
				consoleServices.ShowMessage(DialogType.Warning, PackageCreatorFacade.GetLocalization("NoPackages.Title"),
				                            PackageCreatorFacade.GetLocalization("NoPackages.Message"));
				return null;
			}

			if (actionToken is AddLocalizationActionToken)
			{
				var token = actionToken as AddLocalizationActionToken;
				PackageCreatorFacade.AddItem(new PCLocalizations(token.CultureName), packageName);
			}

			var treeRefresher = new SpecificTreeRefresher(flowControllerServicesContainer);
			treeRefresher.PostRefreshMesseges(new PackageCreatorElementProviderEntityToken());

			return null;

		}
		#endregion

	}
}
