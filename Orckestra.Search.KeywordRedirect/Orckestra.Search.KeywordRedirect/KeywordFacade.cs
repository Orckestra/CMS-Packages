using System;
using System.Globalization;
using Composite.Data;

namespace Orckestra.Search.KeywordRedirect
{
    public class KeywordFacade
    {
        public static string GetPageUrl(Guid landingPage, CultureInfo cultureInfo) {
            
            return  Composite.Core.Routing.PageUrls.BuildUrl(new Composite.Core.Routing.PageUrlData(landingPage, PublicationScope.Published, cultureInfo)) ??
                      Composite.Core.Routing.PageUrls.BuildUrl(new Composite.Core.Routing.PageUrlData(landingPage, PublicationScope.Unpublished, cultureInfo));
        }
    }
}
