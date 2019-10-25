using System;
using System.Linq;
using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Trees;
using Composite.Core.WebClient;
using Orckestra.Search.KeywordRedirect.Data.Types;

namespace Orckestra.Search.KeywordRedirect.C1Console
{
    public class KeywordsUrlToEntityTokenMapper : IUrlToEntityTokenMapper
    {
        public BrowserViewSettings TryGetBrowserViewSettings(EntityToken entityToken, bool showPublishedView)
        {
            const string ViewKeywordsUrl = "/InstalledPackages/Orckestra.Search.KeywordRedirect/view/viewkeywords.html";

            if (entityToken is TreeSimpleElementEntityToken castedEntityToken && castedEntityToken.Id == "OrckestraSearchKeywordRedirect")
            {
                return new BrowserViewSettings
                {
                    Url = UrlUtils.Combine(UrlUtils.AdminRootPath, ViewKeywordsUrl),
                    ToolingOn = false
                };
            }

            if (entityToken is TreeDataFieldGroupingElementEntityToken groupingEntityToken 
                && groupingEntityToken.Type.StartsWith(typeof(RedirectKeyword).ToString()) 
                && groupingEntityToken.GroupingValues?.FirstOrDefault().Value is Guid homepageId)
            {
                return new BrowserViewSettings
                {
                    Url = UrlUtils.Combine(UrlUtils.AdminRootPath, $"{ViewKeywordsUrl}?homePageId={homepageId}"),
                    ToolingOn = false
                };
            }

            return null;
        }

        public EntityToken TryGetEntityToken(string url)
        {
            return null;
        }

        public string TryGetUrl(EntityToken entityToken)
        {
            return null;
        }
    }
}
