namespace Orckestra.Search.KeywordRedirect.Model
{
    public class RedirectKeyword
    {
        public string Keyword { get; set; }
        public string HomePage { get; set; }
        public string LandingPage { get; set; }

        public string KeywordUnpublished { get; set; }
        public string LandingPageUnpublished { get; set; }
        public string HomePageUnpublished { get; set; }

        public string PublishDate { get; set; }
        public string UnpublishDate { get; set; }
    }
}
