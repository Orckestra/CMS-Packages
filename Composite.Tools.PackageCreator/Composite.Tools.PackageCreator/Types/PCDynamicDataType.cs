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
	[PCCategory("DynamicDataTypes")]
	public class PCDynamicDataType : SimplePackageCreatorItem
	{
		public override string Name {
			get {
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

		public PCDynamicDataType(EntityToken entityToken)
			:base(entityToken)
		{
		}

		public PCDynamicDataType(string name)
			: base(name)
		{
		}

		public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
		{
			if (entityToken is GeneratedDataTypesElementProviderTypeEntityToken)
			{
				yield return new PCDynamicDataType(entityToken);
			}
		}

	}

}
