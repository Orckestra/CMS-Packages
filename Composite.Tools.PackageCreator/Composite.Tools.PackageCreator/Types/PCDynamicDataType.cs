using System;
using System.Collections.Generic;
using System.Linq;
using Composite.C1Console.Security;
using Composite.Core.Types;
using Composite.Data;
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
                    var castedEntityToken = _entityToken as GeneratedDataTypesElementProviderTypeEntityToken;
                    if (castedEntityToken != null)
                    {
                        Type type = TypeManager.GetType(castedEntityToken.SerializedTypeName);
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
            var castedEntityToken = entityToken as GeneratedDataTypesElementProviderTypeEntityToken;
            if (castedEntityToken == null)
            {
                return Enumerable.Empty<IPackItem>();
            }
            
            Type type = TypeManager.GetType(castedEntityToken.SerializedTypeName);
            if (!type.IsGenerated())
            {
                return Enumerable.Empty<IPackItem>();
            }
            
            return new[] { new PCDynamicDataType(entityToken) };
        }

    }

}
