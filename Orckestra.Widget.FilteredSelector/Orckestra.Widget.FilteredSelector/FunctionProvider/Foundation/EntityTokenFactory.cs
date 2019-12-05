using System;
using Composite.C1Console.Security;
using Composite.Core.Extensions;
using Composite.Functions;

namespace Orckestra.Widget.FilteredSelector.FunctionProvider.Foundation
{
	public sealed class EntityTokenFactory
	{
		private readonly string _providerName;
		internal EntityTokenFactory(string providerName) => _providerName = providerName ?? throw new ArgumentNullException(nameof(providerName));
		internal EntityToken CreateEntityToken(IMetaFunction function)
		{
			string id = StringExtensionMethods.CreateNamespace(function.Namespace, function.Name, '.');
			return new FilteredSelectorFunctionProviderEntityToken(_providerName, id);
		}
	}
}
