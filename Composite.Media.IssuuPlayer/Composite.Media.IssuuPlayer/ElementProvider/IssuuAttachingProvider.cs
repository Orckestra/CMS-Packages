using System.Collections.Generic;
using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementAttachingProvider;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Core.Types;
using Composite.Plugins.Elements.ElementProviders.GeneratedDataTypesElementProvider;

namespace Composite.Media.IssuuPlayer.ElementProvider
{
	//[ConfigurationElementType(typeof(NonConfigurableElementAttachingProvider))]
	public sealed class IssuuAttachingProvider : IElementAttachingProvider
	{
		public ElementProviderContext Context
		{
			get;
			set;
		}

		public ElementAttachingProviderResult GetAlternateElementList(EntityToken parentEntityToken, Dictionary<string, string> piggybag)
		{
			if (parentEntityToken is GeneratedDataTypesElementProviderTypeEntityToken)
			{
				var token = parentEntityToken as GeneratedDataTypesElementProviderTypeEntityToken;
				if (token.SerializedTypeName == TypeManager.SerializeType(typeof(ApiKey)))
				{
					var label = IssuuApi.GetDefaultLabel();
					if (!string.IsNullOrEmpty(label))
					{
						ElementAttachingProviderResult result = new ElementAttachingProviderResult()
						{
							Elements = GetRootElements(label, piggybag),
							Position = ElementAttachingProviderPosition.Top,
							PositionPriority = 0
						};
						return result;
					}
				}
			}
			return null;
		}

		private IEnumerable<Element> GetRootElements(string label, Dictionary<string, string> piggybag)
		{
			yield return new Element(this.Context.CreateElementHandle(new DefaultIssuuEntityToken()))
			{
				VisualData = new ElementVisualizedData
				{
					Label = label,
					ToolTip = "Default Api Key",
					HasChildren = false,
					Icon = new ResourceHandle("Composite.Icons", "accept")
				}
			};
		}
		public IEnumerable<Element> GetChildren(EntityToken parentEntityToken, Dictionary<string, string> piggybag)
		{
			yield break;
		}

		public bool HaveCustomChildElements(EntityToken parentEntityToken, Dictionary<string, string> piggybag)
		{
			return false;
		}
	}
}
