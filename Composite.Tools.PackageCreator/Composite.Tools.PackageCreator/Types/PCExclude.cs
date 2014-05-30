using System.Collections.Generic;
using System.Text.RegularExpressions;
using Composite.C1Console.Security;
using Composite.Core.IO;
using Composite.Plugins.Elements.ElementProviders.WebsiteFileElementProvider;
using Composite.Data;
using Composite.Data.Types;
using System;

namespace Composite.Tools.PackageCreator.Types
{
	//[PCCategory("Excludes")]
#warning localization
	[PCCategory("Excludes", "Excludes")]
	internal class PCExclude : SimplePackageCreatorItem, IInitable
	{
		public PCExclude(string name)
			: base(name)
		{
		}

		public override string ActionLabel
		{
			get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.Page.Label", this.CategoryName)); }
		}

		public override string ActionToolTip
		{
			get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.Page.ToolTip", this.CategoryName)); }
		}

		public override string GetLabel()
		{
			string result;
			try
			{
				result = PageManager.GetPageById(new Guid(Id), true).Title;
			}
			catch {
				result = base.GetLabel();
			}
			return result;
		}

		public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
		{
			if (entityToken is DataEntityToken)
			{
				DataEntityToken dataEntityToken = (DataEntityToken)entityToken;
				if (dataEntityToken.Data is IPage)
				{
					var page = dataEntityToken.Data as IPage;
					yield return new PCExclude(page.Id.ToString());
				}
			}
		}

		public void Init(PackageCreator creator)
		{
			Guid id;
			if (Guid.TryParse(this.Id, out id))
			{
				creator.ExludedIds.Add(id);
			}
		}
	}
}
