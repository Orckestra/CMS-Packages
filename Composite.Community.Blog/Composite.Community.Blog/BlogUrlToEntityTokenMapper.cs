using System;
using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Trees;
using Composite.Core.Extensions;
using Composite.Core.Routing;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Community.Blog
{
    class BlogUrlToEntityTokenMapper: IUrlToEntityTokenMapper
    {
        private const string BlogTreeSource = "Composite.Community.Blog.Entries.xml";

        public string TryGetUrl(EntityToken entityToken)
        {
            var groupingEntityToken = entityToken as TreeDataFieldGroupingElementEntityToken;
            if (groupingEntityToken != null && groupingEntityToken.Source == BlogTreeSource)
            {
                object dateTimeObject;
                if (groupingEntityToken.ChildGeneratingDataElementsReferenceType == typeof (IPage)
                    && groupingEntityToken.DeserializedGroupingValues.TryGetValue("Date", out dateTimeObject))
                {
                    var pageId = (Guid) groupingEntityToken.ChildGeneratingDataElementsReferenceValue;
                    var page = PageManager.GetPageById(pageId);
                    if (page == null)
                    {
                        return null;
                    }

                    var date = (DateTime) dateTimeObject;
                    string pathInfo = "/{0}/{1}".FormatWith(date.Year, date.Month);

                    return PageUrls.BuildUrl(new PageUrlData(page) {PathInfo = pathInfo},
                        UrlKind.Public,
                        new UrlSpace {ForceRelativeUrls = true});
                }

                return null;
            }

            var treeElementEntityToken = entityToken as TreeSimpleElementEntityToken;
            if (treeElementEntityToken != null 
                && treeElementEntityToken.Source == BlogTreeSource 
                && treeElementEntityToken.Id == "Root")
            {
                var parentEntityToken = treeElementEntityToken.ParentEntityToken;

                var dataParent = parentEntityToken as DataEntityToken;
                if (dataParent != null && dataParent.InterfaceType == typeof (IPage))
                {
                    var page = dataParent.Data as IPage;
                    if (page != null)
                    {
                        return PageUrls.BuildUrl(new PageUrlData(page),
                            UrlKind.Public,
                            new UrlSpace { ForceRelativeUrls = true });
                    }
                }
            }

            return null;
        }

        public BrowserViewSettings TryGetBrowserViewSettings(EntityToken entityToken, bool showPublishedView)
        {
            using (new DataConnection(showPublishedView ? PublicationScope.Published : PublicationScope.Unpublished))
            {
                string url = TryGetUrl(entityToken);
                return url == null ? null : new BrowserViewSettings { Url = TryGetUrl(entityToken), ToolingOn = true };
            }
        }

        public EntityToken TryGetEntityToken(string url)
        {
            return null;
        }
    }
}
