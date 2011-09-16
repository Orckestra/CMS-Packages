using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.C1Console.Security;
using Composite.Plugins.Elements.ElementProviders.BaseFunctionProviderElementProvider;

namespace Composite.Forms.Renderer.FormsRendererFunctionProvider
{
#warning This class is a work around, duplicating code from C1 3.0. Remove this class when building a C1 3.0 only release of this package – instead call Composite.Functions.StandardFunctionSecurityAncestorProvider
	public class StandardFunctionSecurityAncestorProvider : ISecurityAncestorProvider
	{
		public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
		{
			string fullname = entityToken.Id;
			string providerName = entityToken.Source;

			if (fullname.Contains('.'))
			{
				fullname = fullname.Remove(fullname.LastIndexOf('.'));
			}

			string id = BaseFunctionProviderElementProvider.CreateId(fullname, "AllFunctionsElementProvider");

			yield return new BaseFunctionFolderElementEntityToken(id);
		}
	}
}
