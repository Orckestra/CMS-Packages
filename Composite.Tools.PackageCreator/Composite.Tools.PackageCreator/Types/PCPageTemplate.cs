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
	[PCCategory("PageTemplates")]
	internal class PCPageTemplate : SimplePackageCreatorItem
	{
		public PCPageTemplate(string name)
			: base(name)
		{
		}

		public override ResourceHandle ItemIcon
		{
			get { return new ResourceHandle("Composite.Icons", "page-template-template"); }
		}

		public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
		{
			if (entityToken is DataEntityToken)
			{
				DataEntityToken dataEntityToken = (DataEntityToken)entityToken;
				if (dataEntityToken.Data is IPageTemplate)
				{
					IPageTemplate data = (IPageTemplate)dataEntityToken.Data;
					yield return new PCPageTemplate(data.Title);
				}
			}
		}

	}
}
