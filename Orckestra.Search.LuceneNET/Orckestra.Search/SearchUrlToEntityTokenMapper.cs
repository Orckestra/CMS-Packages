using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Trees;
using Composite.Core.WebClient;

namespace Orckestra.Search
{
    internal class SearchUrlToEntityTokenMapper: IUrlToEntityTokenMapper
    {
        public string TryGetUrl(EntityToken entityToken)
        {
            return null;
        }

        public BrowserViewSettings TryGetBrowserViewSettings(EntityToken entityToken, bool showPublishedView)
        {
            var castedEntityToken = entityToken as TreeSimpleElementEntityToken;
            if (castedEntityToken == null || castedEntityToken.Id != "system.content.search.simple")
            {
                return null;
            }

            return new BrowserViewSettings
            {
                Url = UrlUtils.Combine(UrlUtils.PublicRootPath, 
                                       "/Composite/content/views/simplesearch/SimpleSearch.cshtml"),
                ToolingOn = false
            };
        }

        public EntityToken TryGetEntityToken(string url)
        {
            return null;
        }
    }
}
