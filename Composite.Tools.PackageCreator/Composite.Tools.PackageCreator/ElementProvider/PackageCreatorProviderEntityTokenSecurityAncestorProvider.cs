using System.Collections.Generic;
using Composite.C1Console.Security;

namespace Composite.Tools.PackageCreator.ElementProvider
{
	class PackageCreatorProviderEntityTokenSecurityAncestorProvider : ISecurityAncestorProvider
	{
		public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
		{
			return new EntityToken[] { new PackageCreatorElementProviderEntityToken() };
		}
	}
}
