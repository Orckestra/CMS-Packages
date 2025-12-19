using System.Collections.Generic;
using System.Xml.Linq;
using Composite.C1Console.Security;
using Composite.Core.Types;
using Composite.Plugins.Elements.ElementProviders.GeneratedDataTypesElementProvider;

namespace Composite.Tools.PackageCreator.Types
{
    [PackCategory("DynamicDataTypesData")]
    public class PCDynamicDataTypesData : BasePackItem
    {
        public PCDynamicDataTypesData(XElement element) : base(element)
        {
        }

        public PCDynamicDataTypesData(EntityToken entityToken) : base(entityToken)
        {
        }

        public PCDynamicDataTypesData(string name) : base(name)
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

        public override string ActionLabel => PackageCreatorFacade.GetLocalization($"{CategoryName}.Add.Label");

        public override string ActionToolTip => PackageCreatorFacade.GetLocalization($"{CategoryName}.Add.ToolTip");

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
        {
            if (entityToken is GeneratedDataTypesElementProviderTypeEntityToken)
                yield return new PCDynamicDataTypesData(entityToken);
        }
    }
}