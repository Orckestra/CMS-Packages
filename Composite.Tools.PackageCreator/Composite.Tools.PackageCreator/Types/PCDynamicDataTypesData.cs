using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Composite.C1Console.Security;
using Composite.Core.Types;
using Composite.Plugins.Elements.ElementProviders.GeneratedDataTypesElementProvider;

namespace Composite.Tools.PackageCreator.Types
{
    [PackCategory("DynamicDataTypesData")]
    public class PCDynamicDataTypesData : BasePackItem, IPackOverwriteItem
    {
        public PCDynamicDataTypesData(XElement element): base(element)
        {
        }

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

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
        {
            if (entityToken is GeneratedDataTypesElementProviderTypeEntityToken)
            {
                yield return new PCDynamicDataTypesData(entityToken);
            }
        }

        public bool AllowOverwrite { get; set; }
    }

}
