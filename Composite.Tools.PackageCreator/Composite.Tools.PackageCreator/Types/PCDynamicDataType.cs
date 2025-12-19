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
        public PCDynamicDataType(EntityToken entityToken) : base(entityToken)
        {
        }

        public PCDynamicDataType(string name) : base(name)
        {
        }

        public override string Id
        {
            get
            {
                if (Name != null ||
                    !(_entityToken is GeneratedDataTypesElementProviderTypeEntityToken castedEntityToken))
                    return Name;

                var type = TypeManager.GetType(castedEntityToken.SerializedTypeName);
                Name = type.FullName;
                return Name;
            }
        }

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
        {
            var castedEntityToken = entityToken as GeneratedDataTypesElementProviderTypeEntityToken;
            if (castedEntityToken == null) return Enumerable.Empty<IPackItem>();

            var type = TypeManager.GetType(castedEntityToken.SerializedTypeName);
            return !type.IsGenerated() ? Enumerable.Empty<IPackItem>() : new[] { new PCDynamicDataType(entityToken) };
        }
    }
}