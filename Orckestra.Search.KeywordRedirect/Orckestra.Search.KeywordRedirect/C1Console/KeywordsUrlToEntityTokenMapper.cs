using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Trees;
using Composite.Core.WebClient;

namespace Orckestra.Search.KeywordRedirect.C1Console
{
    public class KeywordsUrlToEntityTokenMapper : IUrlToEntityTokenMapper
    {
        public BrowserViewSettings TryGetBrowserViewSettings(EntityToken entityToken, bool showPublishedView)
        {
            var castedEntityToken = entityToken as TreeSimpleElementEntityToken;

            if (castedEntityToken == null || castedEntityToken.Id != "OrckestraSearchKeywordRedirect")
            {
                return null;
            }

            return new BrowserViewSettings
            {
                Url = UrlUtils.Combine(UrlUtils.AdminRootPath, "/InstalledPackages/Orckestra.Search.KeywordRedirect/view/viewkeywords.html"),
                ToolingOn = false
            };
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
