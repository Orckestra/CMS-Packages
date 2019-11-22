using System;
using System.Collections.Generic;
using System.Linq;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Tools.PackageCreator.Types
{
    [PackCategory("PageTypes")]
    internal class PCPageType : BasePackItem, IPack
    {
        public PCPageType(string name)
            : base(name)
        {
        }

        public override ResourceHandle ItemIcon
        {
            get { return new ResourceHandle("Composite.Icons", "base-function-function"); }
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
