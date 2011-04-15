using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Composite.C1Console.Actions;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Composite.Data;
using Composite.C1Console.Security;
using Composite.Plugins.Elements.ElementProviders.GeneratedDataTypesElementProvider;
using Composite.Core.Types;
using System.Xml.Linq;
using Composite.Data.Types;

namespace Composite.Tools.PackageCreator.Types
{
	[PCCategory("DynamicDataTypesData")]
	public class PCDynamicDataTypesData : SimplePackageCreatorItem
	{
		public override string Name
		{
			get
			{
				if (_name == null)
				{
					if (_entityToken is GeneratedDataTypesElementProviderTypeEntityToken)
					{
						Type type = TypeManager.GetType(((GeneratedDataTypesElementProviderTypeEntityToken)_entityToken).SerializedTypeName);
						_name = type.FullName;
					}
				}
				return _name;
			}
		}

		public override string ActionLabel
		{
			get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.Label", typeof(PCDynamicDataTypesData).GetCategoryNameAtribute())); }
		}

		public override string ActionToolTip
		{
			get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.ToolTip", typeof(PCDynamicDataTypesData).GetCategoryNameAtribute())); }
		}

		public PCDynamicDataTypesData(EntityToken entityToken)
			: base(entityToken)
		{
		}

		public PCDynamicDataTypesData(string name)
			: base(name)
		{
		}

		public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
		{
			if (entityToken is GeneratedDataTypesElementProviderTypeEntityToken)
			{
				if (entityToken.Id == "GlobalDataTypeFolder")
					yield return new PCDynamicDataTypesData(entityToken);
			}
		}

	}

}
