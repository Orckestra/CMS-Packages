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
	[PCCategory("VisualFunctions")]
	internal class PCVisualFunction : SimplePackageCreatorItem
	{
		public PCVisualFunction(string name)
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
				if (dataEntityToken.Data is IVisualFunction)
				{
					IVisualFunction data = (IVisualFunction)dataEntityToken.Data;
					yield return new PCVisualFunction(data.Namespace + "." + data.Name);
				}
			}
		}

	}
}
