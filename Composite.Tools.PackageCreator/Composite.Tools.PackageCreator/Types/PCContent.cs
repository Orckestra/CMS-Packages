using System;
using System.Collections.Generic;
using System.Linq;
using Composite.C1Console.Security;
using Composite.Core.ResourceSystem;
using Composite.Data;
using Composite.Data.Types;
using Composite.Plugins.Elements.ElementProviders.PageElementProvider;
using Composite.Tools.PackageCreator.ElementProvider.EntityTokens;

namespace Composite.Tools.PackageCreator.Types
{
    [PackCategory("Content")]
    internal class PCContent : BasePackItem, IPackItemActionToken, IPack
    {
        private const string _pagesName = "Pages";
        private const string _mediasName = "Medias";
        private const string _datatypesName = "DatatypesData";
        private const string _applicationsName = "Applications";
		private const string _metatypesName = "MetatypesData";

        public PCContent(string name)
            : base(name)
        {
        }

        public override string ActionLabel
        {
            get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.{1}.Add.Label", this.CategoryName, Name)); }
        }

        public override string ActionToolTip
        {
            get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.{1}.Add.ToolTip", this.CategoryName, Name)); }
        }

        public override ResourceHandle ItemIcon
        {
            get
            {
                if (Id == _pagesName)
                    return new ResourceHandle("Composite.Icons", "page-publication");
                if (Id == _mediasName)
                    return new ResourceHandle("Composite.Icons", "perspective-media");
                if (Id == _datatypesName)
                    return new ResourceHandle("Composite.Icons", "perspective-datas");
                if (Id == _applicationsName)
                    return new ResourceHandle("Composite.Icons", "perspective-developerapplication");
				if (Id == _metatypesName)
					return new ResourceHandle("Composite.Icons", "perspective-datas");
                return base.ItemIcon;
            }
        }

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
        {
            if (entityToken is PackageCreatorPackageElementProviderEntityToken
                || entityToken is PageElementProviderEntityToken)
            {
                /*yield return new PCContent(_pagesName);*/
                //yield return new PCContent(_mediasName);
                //yield return new PCContent(_datatypesName);
                //yield return new PCContent(_applicationsName);
				yield return new PCContent(_metatypesName);
            }
        }
        public string ActionTokenName
        {
            get { return Name; }
        }

        public void Pack(PackageCreator creator)
        {
            if (Id == _pagesName)
            {
                #region All Pages
                HashSet<Guid> pages;
                using (var scope = new DataScope(DataScopeIdentifier.Administrated))
                {
                    pages = DataFacade.GetData<IPage>().Select(p => p.Id).ToHashSet();
                }

                creator.AddData(typeof(IPage), DataScopeIdentifier.Public, d => pages.Contains((d as IPage).Id));
                creator.AddData(typeof(IPage), DataScopeIdentifier.Administrated, d => pages.Contains((d as IPage).Id));
                creator.AddData(typeof(IPagePlaceholderContent), DataScopeIdentifier.Public, d => pages.Contains((d as IPagePlaceholderContent).PageId));
                creator.AddData(typeof(IPagePlaceholderContent), DataScopeIdentifier.Administrated, d => pages.Contains((d as IPagePlaceholderContent).PageId));
                creator.AddData(typeof(IPageStructure), DataScopeIdentifier.Public, d => pages.Contains((d as IPageStructure).Id));
                #endregion

            }
            else if (Id == _mediasName)
            {
                creator.AddData(typeof(IMediaFileData));
                creator.AddData(typeof(IMediaFolderData));
                creator.AddFilesinDirectory(@"App_Data\Media\");
            }
            else if (Id == _datatypesName)
            {
                IEnumerable<Type> pageDataTypeInterfaces = PageFolderFacade.GetAllFolderTypes();
                IEnumerable<Type> pageMetaTypeInterfaces = PageMetaDataFacade.GetAllMetaDataTypes();

                foreach (var pageDataType in pageDataTypeInterfaces)
                {
                    creator.AddDinamicDataTypeData(pageDataType);
                }
                foreach (var pageMetaType in pageMetaTypeInterfaces)
                {
                    creator.AddDinamicDataTypeData(pageMetaType);
                }

            }
            else if (Id == _applicationsName)
            {
                creator.AddData<IDataItemTreeAttachmentPoint>();
            }
			else if (Id == _metatypesName)
			{
				
				IEnumerable<Type> pageMetaTypeInterfaces = PageMetaDataFacade.GetAllMetaDataTypes();

				foreach (var pageMetaType in pageMetaTypeInterfaces)
				{
					creator.AddDinamicDataTypeData(pageMetaType);
				}

			}
            return;
        }
    }
}
