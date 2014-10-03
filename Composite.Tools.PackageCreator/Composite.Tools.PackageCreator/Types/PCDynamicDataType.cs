using System;
using System.Collections.Generic;
using Composite.C1Console.Security;
using Composite.Core.Types;
using Composite.Plugins.Elements.ElementProviders.GeneratedDataTypesElementProvider;

namespace Composite.Tools.PackageCreator.Types
{
    [PackCategory("DynamicDataTypes")]
    public class PCDynamicDataType : BasePackItem
    {
        public override string Id
        {
            get
            {
                if (Name == null)
                {
                    if (_entityToken is GeneratedDataTypesElementProviderTypeEntityToken)
                    {
                        Type type = TypeManager.GetType(((GeneratedDataTypesElementProviderTypeEntityToken)_entityToken).SerializedTypeName);
                        Name = type.FullName;
                    }
                }
                return Name;
            }
        }

        public PCDynamicDataType(EntityToken entityToken)
            : base(entityToken)
        {
        }

        public PCDynamicDataType(string name)
            : base(name)
        {
        }

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
        {
            if (entityToken is GeneratedDataTypesElementProviderTypeEntityToken)
            {
                yield return new PCDynamicDataType(entityToken);
            }
        }

    }

}
