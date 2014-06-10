using System;
using System.Collections.Generic;
using System.Linq;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Tools.PackageCreator.Types
{
    [PCCategory("PageTypes")]
    internal class PCPageType : SimplePackageCreatorItem, IPackageable
    {
        public PCPageType(string name)
            : base(name)
        {
        }

        public override ResourceHandle ItemIcon
        {
            get { return new ResourceHandle("Composite.Icons", "base-function-function"); }
        }

        public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
        {
            if (entityToken is DataEntityToken)
            {
                DataEntityToken dataEntityToken = (DataEntityToken)entityToken;
                if (dataEntityToken.Data is IPageType)
                {
                    var data = (IPageType)dataEntityToken.Data;
                    yield return new PCPageType(data.Name);
                }
            }
        }

        public void Pack(PackageCreator creator)
        {
            var pageType = DataFacade.GetData<IPageType>(d => d.Name == this.Name).FirstOrDefault();
            if (pageType == null)
                throw new InvalidOperationException(string.Format("PageType '{0}' does not exists", this.Name));
            var pageTypeId = pageType.Id;
            creator.AddData(pageType);
            creator.AddData<IPageTypeDataFolderTypeLink>(d => d.PageTypeId == pageTypeId);
            creator.AddData<IPageTypeDefaultPageContent>(d => d.PageTypeId == pageTypeId);
            creator.AddData<IPageTypeMetaDataTypeLink>(d => d.PageTypeId == pageTypeId);
            creator.AddData<IPageTypePageTemplateRestriction>(d => d.PageTypeId == pageTypeId);
            creator.AddData<IPageTypeParentRestriction>(d => d.PageTypeId == pageTypeId);
            creator.AddData<IPageTypeTreeLink>(d => d.PageTypeId == pageTypeId);
            foreach (var pageMetaDataDefinition in DataFacade.GetData<IPageMetaDataDefinition>(d => d.DefiningItemId == pageTypeId))
            {
                creator.AddData(pageMetaDataDefinition);
                creator.AddData<ICompositionContainer>(d => d.Id == pageMetaDataDefinition.MetaDataContainerId);
            }
        }
    }
}
