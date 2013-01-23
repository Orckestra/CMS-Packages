using System.Collections.Generic;
using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementActionProvider;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Plugins.Elements.ElementProviders.GeneratedDataTypesElementProvider;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace Composite.Tools.StaticDataTypeCreator
{
	[ConfigurationElementType(typeof(NonConfigurableElementActionProvider))]
	public class StaticDataTypeCreatorActionProvider : IElementActionProvider
	{
		private static readonly ActionGroup StaticDataTypeCreatorActionGroup = new ActionGroup("StaticDataTypeCreator", ActionGroupPriority.GeneralAppendHigh);
		private static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = false, ActionGroup = StaticDataTypeCreatorActionGroup };

		public IEnumerable<ElementAction> GetActions(EntityToken entityToken)
		{
			var generatedDataTypetoken = entityToken as GeneratedDataTypesElementProviderTypeEntityToken;
			if (generatedDataTypetoken != null)
			{
				if (generatedDataTypetoken.Source == "GeneratedDataTypesElementProvider")
				{
					yield return new ElementAction(new ActionHandle(new StaticDataTypeCreatorActionToken()))
					{
						VisualData = new ActionVisualizedData
						{
							Label = "Download Datatype Source Code",
							ToolTip = "Generate a C# interface definition representing this datatype. You can add this to your code project and then remove this dynamic type definition.",
							Icon = new ResourceHandle("Composite.Icons", "media-download-file"),
							ActionLocation = ActionLocation
						}
					};
				}
			}
		}
	}
}
