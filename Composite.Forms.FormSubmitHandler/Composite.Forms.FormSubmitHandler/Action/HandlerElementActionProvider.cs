using System.Collections.Generic;
using System.IO;
using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementActionProvider;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Plugins.Elements.ElementProviders.WebsiteFileElementProvider;

namespace Composite.Forms.FormSubmitHandler
{
#warning ConfigurationElementType(typeof(NonConfigurableElementActionProvider))]
	public class HandlerElementActionProvider : IElementActionProvider
	{
		public IEnumerable<ElementAction> GetActions(EntityToken entityToken)
		{
			if (entityToken is WebsiteFileElementProviderEntityToken)
			{
				if (Path.GetExtension(entityToken.Id).ToLower() == ".xml")
				{
					if (entityToken.Id.Replace("/", "\\").Contains(StoreHelper.StoringPath.Replace("/", "\\")))
					{
						yield return new ElementAction(new ActionHandle(new HandleActionToken()))
						{
							VisualData = new ActionVisualizedData
							{
								Label = "Export to Excel",
								ToolTip = "Export to Excel",
								Icon = new ResourceHandle("Composite.Icons", "mimetype-xls"),
								ActionLocation = new ActionLocation
								{
									ActionType = ActionType.Other,
									IsInFolder = false,
									IsInToolbar = false,
									ActionGroup = new ActionGroup("Export", ActionGroupPriority.PrimaryLow)
								}
							}
						};
					}
				}
			}
			yield break;
		}
	}
}