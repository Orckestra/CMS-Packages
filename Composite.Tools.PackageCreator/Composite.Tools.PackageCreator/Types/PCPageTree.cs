using System;
using System.Collections.Generic;
using Composite.C1Console.Security;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Tools.PackageCreator.Types
{
    [PCCategory("PageTrees")]
    internal class PCPageTree : SimplePackageCreatorItem, IPackageable
    {
        public PCPageTree(string name)
            : base(name)
        {
        }

        public override string ActionLabel
        {
            get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.Label", this.CategoryName)); }
        }

        public override string ActionToolTip
        {
            get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.Add.ToolTip", this.CategoryName)); }
        }

        public override string GetLabel()
        {
            return PageManager.GetPageById(new Guid(Name), true).Title;
        }

        public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
        {
            if (entityToken is DataEntityToken)
            {
                DataEntityToken dataEntityToken = (DataEntityToken)entityToken;
                if (dataEntityToken.Data is IPage)
                {
                    var page = dataEntityToken.Data as IPage;
                    yield return new PCPageTree(page.Id.ToString());
                }
            }
        }

        public void Pack(PackageCreator pc)
        {
            var pageId = new Guid(Name);
            PackPageTree(pc, pageId);
            var pageStructure = DataFacade.BuildNew<IPageStructure>();
            pageStructure.Id = pageId;
            pageStructure.ParentId = Guid.Empty;
            pageStructure.LocalOrdering = PageManager.GetLocalOrdering(pageId);
            pc.AddData(pageStructure);
        }

        private void PackPageTree(PackageCreator pc, Guid pageId)
        {
            foreach (var childPageId in PageManager.GetChildrenIDs(pageId))
            {
                PackPageTree(pc, childPageId);
                pc.AddData<IPageStructure>(d => d.ParentId == pageId && d.Id == childPageId);
            }
            pc.AddData<IPage>(d => d.Id == pageId);
            pc.AddData<IPagePlaceholderContent>(d => d.PageId == pageId);
        }
    }
}
