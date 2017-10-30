using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.ExperienceManagement.KeywordRedirect.Model
{
    public class RedirectKeyword
    {
        public string Keyword { get; set; }
        public string LandingPage { get; set; }

        public string KeywordUnpublished { get; set; }
        public string LandingPageUnpublished { get; set; }

        public string PublishDate { get; set; }
        public string UnpublishDate { get; set; }
    }
}
