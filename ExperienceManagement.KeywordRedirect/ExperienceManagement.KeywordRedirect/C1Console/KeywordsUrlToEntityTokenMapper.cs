using Composite.C1Console.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Composite.C1Console.Security;
using Composite.C1Console.Trees;
using Composite.Core.WebClient;

namespace Orckestra.ExperienceManagement.KeywordRedirect.C1Console
{
    public class KeywordsUrlToEntityTokenMapper : IUrlToEntityTokenMapper
    {
        public BrowserViewSettings TryGetBrowserViewSettings(EntityToken entityToken, bool showPublishedView)
        {
            var castedEntityToken = entityToken as TreeSimpleElementEntityToken;

            if (castedEntityToken == null || castedEntityToken.Id != "ExperienceManagement.KeywordRedirect")
            {
                return null;
            }

            return new BrowserViewSettings
            {
                Url = UrlUtils.Combine(UrlUtils.AdminRootPath, "/InstalledPackages/ExperienceManagement.KeywordRedirect/view/viewkeywords.html"),
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
