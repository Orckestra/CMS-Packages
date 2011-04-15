using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.C1Console.Security;
using Composite.Data;
using Composite.Data.Types;
using System.Xml.Linq;
using Composite.Core.ResourceSystem;

namespace Composite.Tools.PackageCreator.Types
{
	[PCCategory("XsltFunctions")]
	internal class PCXsltFunction : SimplePackageCreatorItem
	{
		public PCXsltFunction(string name)
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
				if (dataEntityToken.Data is IXsltFunction)
				{
					IXsltFunction data = (IXsltFunction)dataEntityToken.Data;
					yield return new PCXsltFunction(data.Namespace + "." + data.Name);
					yield break;
				}
			}
		}

	}
}
