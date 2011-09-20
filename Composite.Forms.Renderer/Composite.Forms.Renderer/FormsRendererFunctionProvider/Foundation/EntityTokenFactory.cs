using Composite.C1Console.Security;
using Composite.Functions;
using Composite.Core.Extensions;

namespace Composite.Forms.Renderer.FormsRendererFunctionProvider.Foundation
{
	public sealed class EntityTokenFactory
	{
		private readonly string _providerName;

		internal EntityTokenFactory(string providerName)
		{
			_providerName = providerName;
		}

		internal EntityToken CreateEntityToken(IMetaFunction function)
		{
			string id = StringExtensionMethods.CreateNamespace(function.Namespace, function.Name, '.');

			return new FormsRendererFunctionProviderEntityToken(_providerName, id );
		}
	}
}
