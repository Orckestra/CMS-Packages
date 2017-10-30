using Composite.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.ExperienceManagement.KeywordRedirect
{
    public class KeywordFacade
    {
        public static string GetPageUrl(Guid landingPage, CultureInfo cultureInfo) {
            
            return  Composite.Core.Routing.PageUrls.BuildUrl(new Composite.Core.Routing.PageUrlData(landingPage, PublicationScope.Published, cultureInfo)) ??
                      Composite.Core.Routing.PageUrls.BuildUrl(new Composite.Core.Routing.PageUrlData(landingPage, PublicationScope.Unpublished, cultureInfo));
        }
    }
}
