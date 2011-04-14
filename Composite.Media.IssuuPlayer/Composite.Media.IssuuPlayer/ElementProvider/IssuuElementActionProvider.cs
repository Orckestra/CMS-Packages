using System.Collections.Generic;
using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementActionProvider;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Data;
using Composite.Media.IssuuPlayer.Data;

namespace Composite.Media.IssuuPlayer.ElementProvider
{
#warning ConfigurationElementType(typeof(NonConfigurableElementActionProvider))]
	public sealed class IssuuElementActionProvider : IElementActionProvider
	{
		private static readonly ActionGroup _actionGroup = new ActionGroup("Default", ActionGroupPriority.PrimaryLow);

		public IEnumerable<ElementAction> GetActions(EntityToken entityToken)
		{
			if (entityToken is DataEntityToken)
			{
				var token = entityToken as DataEntityToken;
				if (token.Data.DataSourceId.InterfaceType == typeof(ApiKey))
				{
					if (!token.Data.GetProperty<bool>("Default"))
					{
						yield return new ElementAction(new ActionHandle(new SetDefaultActionToken()))
						{
							VisualData = new ActionVisualizedData
							{
								Label = "Set Default",
								ToolTip = "Set Dafault",
								Icon = new ResourceHandle("Composite.Icons", "accept"),
								ActionLocation = new ActionLocation
								{
									ActionType = ActionType.Add,
									IsInFolder = false,
									IsInToolbar = true,
									ActionGroup = _actionGroup
								}
							}
						};
					}
				}
			}
		}
	}
}
