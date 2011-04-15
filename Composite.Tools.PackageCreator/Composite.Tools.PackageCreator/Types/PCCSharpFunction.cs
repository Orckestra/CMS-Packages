using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.C1Console.Security;
using Composite.Data.Types;
using Composite.Data;
using Composite.Core.ResourceSystem;

namespace Composite.Tools.PackageCreator.Types
{
	[PCCategory("CSharpFunctions")]
	class PCCSharpFunction: SimplePackageCreatorItem
	{

		public PCCSharpFunction(string name)
			: base(name)
		{
		}

		public override ResourceHandle ItemIcon
		{
			get { return new ResourceHandle("Composite.Icons", "base-function-function"); }
		}

		public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
		{
			if (entityToken is DataEntityToken)
			{
				DataEntityToken dataEntityToken = (DataEntityToken)entityToken;
				if (dataEntityToken.Data is IMethodBasedFunctionInfo)
				{
					IMethodBasedFunctionInfo data = (IMethodBasedFunctionInfo)dataEntityToken.Data;
					yield return new PCCSharpFunction(data.Namespace + "." + data.UserMethodName);
					yield break;
				}
			}
		}
	}
	
}
