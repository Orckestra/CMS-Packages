using System;
using System.Collections.Generic;
using Composite.C1Console.Security;
using Composite.Core.Types;
using Composite.Plugins.Elements.ElementProviders.GeneratedDataTypesElementProvider;

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
            get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.Label", this.CategoryName)); }
        }

        public override string ActionToolTip
        {
            get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.ToolTip", this.CategoryName)); }
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
