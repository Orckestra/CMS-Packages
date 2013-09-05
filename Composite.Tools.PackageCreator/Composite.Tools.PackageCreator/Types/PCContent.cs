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
    [PCCategory("Content")]
    internal class PCContent : SimplePackageCreatorItem, IPackageCreatorItemActionToken, IPackageable
    {
        private const string _pagesName = "Pages";
        private const string _mediasName = "Medias";
        private const string _datatypesName = "DatatypesData";
        private const string _applicationsName = "Applications";

        public PCContent(string name)
            : base(name)
        {
        }

        public override string ActionLabel
        {
            get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.{1}.Add.Label", this.CategoryName, _name)); }
        }

        public override string ActionToolTip
        {
            get { return PackageCreatorFacade.GetLocalization(string.Format("{0}.{1}.Add.ToolTip", this.CategoryName, _name)); }
        }

        public override ResourceHandle ItemIcon
        {
            get
            {
                if (Name == _pagesName)
                    return new ResourceHandle("Composite.Icons", "page-publication");
                if (Name == _mediasName)
                    return new ResourceHandle("Composite.Icons", "perspective-media");
                if (Name == _datatypesName)
                    return new ResourceHandle("Composite.Icons", "perspective-datas");
                if (Name == _applicationsName)
                    return new ResourceHandle("Composite.Icons", "perspective-developerapplication");

                return base.ItemIcon;
            }
        }

        public static IEnumerable<IPackageCreatorItem> Create(EntityToken entityToken)
        {
            if (entityToken is PackageCreatorPackageElementProviderEntityToken
                || entityToken is PageElementProviderEntityToken)
            {
                yield return new PCContent(_pagesName);
                yield return new PCContent(_mediasName);
                yield return new PCContent(_datatypesName);
                yield return new PCContent(_applicationsName);
            }
        }
        public string ActionTokenName
        {
            get { return _name; }
        }

        public void Pack(PackageCreator creator)
        {
            if (Name == _pagesName)
            {
                #region All Pages
                if (creator.LocaleAction == PackageCreator.LocaleActions.DefaultLocalesToAllLocales || creator.LocaleAction == PackageCreator.LocaleActions.DefaultLocalesToCurrentLocale)
                {
                    HashSet<Guid> pages;
                    using (var locale = new DataScope(DataLocalizationFacade.DefaultLocalizationCulture))
                    {
                        using (var scope = new DataScope(DataScopeIdentifier.Administrated))
                        {
                            pages = DataFacade.GetData<IPage>().Select(p => p.Id).ToHashSet();
                        }

                        creator.AddData(typeof(IPage), DataScopeIdentifier.Public, d => pages.Contains((d as IPage).Id));
                        creator.AddData(typeof(IPage), DataScopeIdentifier.Administrated, d => pages.Contains((d as IPage).Id));
                        creator.AddData(typeof(IPagePlaceholderContent), DataScopeIdentifier.Public, d => pages.Contains((d as IPagePlaceholderContent).PageId));
                        creator.AddData(typeof(IPagePlaceholderContent), DataScopeIdentifier.Administrated, d => pages.Contains((d as IPagePlaceholderContent).PageId));
                        creator.AddData(typeof(IPageStructure), DataScopeIdentifier.Public, d => pages.Contains((d as IPageStructure).Id));
                    }
                }
                else
                {
#warning TODO: Make for adding pages to all locales;
                    throw new NotImplementedException();
                }
                #endregion

            }
            else if (Name == _mediasName)
            {
                creator.AddData(typeof(IMediaFileData));
                creator.AddData(typeof(IMediaFolderData));
                creator.AddFilesinDirectory(@"App_Data\Media\");
            }
            else if (Name == _datatypesName)
            {
                if (creator.LocaleAction == PackageCreator.LocaleActions.DefaultLocalesToAllLocales || creator.LocaleAction == PackageCreator.LocaleActions.DefaultLocalesToCurrentLocale)
                {
                    using (new DataScope(DataLocalizationFacade.DefaultLocalizationCulture))
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
                }
            }
            else if (Name == _applicationsName)
            {
                creator.AddData<IDataItemTreeAttachmentPoint>();
            }
            return;
        }
    }
}
