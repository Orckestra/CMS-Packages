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
        private const string PagesName = "Pages";
        private const string MediasName = "Medias";
        private const string DatatypesName = "DatatypesData";
        private const string ApplicationsName = "Applications";
        private const string MetatypesName = "MetatypesData";

        public PCContent(string name) : base(name) { }

        public override string ActionLabel => PackageCreatorFacade.GetLocalization($"{CategoryName}.{Name}.Add.Label");

        public override string ActionToolTip => PackageCreatorFacade.GetLocalization($"{CategoryName}.{Name}.Add.ToolTip");

        public override ResourceHandle ItemIcon
        {
            get
            {
                switch (Id)
                {
                    case PagesName:
                        return new ResourceHandle("Composite.Icons", "page-publication");
                    case MediasName:
                        return new ResourceHandle("Composite.Icons", "perspective-media");
                    case DatatypesName:
                        return new ResourceHandle("Composite.Icons", "perspective-datas");
                    case ApplicationsName:
                        return new ResourceHandle("Composite.Icons", "perspective-developerapplication");
                    case MetatypesName:
                        return new ResourceHandle("Composite.Icons", "perspective-datas");
                    default:
                        return base.ItemIcon;
                }
            }
        }

        public void Pack(PackageCreator creator)
        {
            switch (Id)
            {
                case PagesName:
                {
                    #region All Pages

                    HashSet<Guid> pages;
                    using (var scope = new DataScope(DataScopeIdentifier.Administrated))
                    {
                        pages = DataFacade.GetData<IPage>().Select(p => p.Id).ToHashSet();
                    }

                    creator.AddData(typeof(IPage), DataScopeIdentifier.Public, d => pages.Contains(((IPage)d).Id));
                    creator.AddData(typeof(IPage), DataScopeIdentifier.Administrated,
                        d => pages.Contains(((IPage)d).Id));
                    creator.AddData(typeof(IPagePlaceholderContent), DataScopeIdentifier.Public,
                        d => pages.Contains(((IPagePlaceholderContent)d).PageId));
                    creator.AddData(typeof(IPagePlaceholderContent), DataScopeIdentifier.Administrated,
                        d => pages.Contains(((IPagePlaceholderContent)d).PageId));
                    creator.AddData(typeof(IPageStructure), DataScopeIdentifier.Public,
                        d => pages.Contains(((IPageStructure)d).Id));

                    #endregion

                    break;
                }
                case MediasName:
                    creator.AddData(typeof(IMediaFileData));
                    creator.AddData(typeof(IMediaFolderData));
                    creator.AddFilesInDirectory(@"App_Data\Media\");
                    break;
                case DatatypesName:
                {
                    var pageDataTypeInterfaces = PageFolderFacade.GetAllFolderTypes();
                    var pageMetaTypeInterfaces = PageMetaDataFacade.GetAllMetaDataTypes();

                    foreach (var pageDataType in pageDataTypeInterfaces) creator.AddDataTypeData(pageDataType);
                    foreach (var pageMetaType in pageMetaTypeInterfaces) creator.AddDataTypeData(pageMetaType);

                    break;
                }
                case ApplicationsName:
                    creator.AddData<IDataItemTreeAttachmentPoint>();
                    break;
                case MetatypesName:
                {
                    var pageMetaTypeInterfaces = PageMetaDataFacade.GetAllMetaDataTypes();

                    foreach (var pageMetaType in pageMetaTypeInterfaces) creator.AddDataTypeData(pageMetaType);

                    break;
                }
            }
        }

        public string ActionTokenName => Name;

        public static IEnumerable<IPackItem> Create(EntityToken entityToken)
        {
            if (entityToken is PackageCreatorPackageElementProviderEntityToken
                || entityToken is PageElementProviderEntityToken)
                /*yield return new PCContent(_pagesName);*/
                //yield return new PCContent(_mediasName);
                //yield return new PCContent(_datatypesName);
                //yield return new PCContent(_applicationsName);
                yield return new PCContent(MetatypesName);
        }
    }
}