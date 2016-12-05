using Lucene.Net.Util;

namespace Orckestra.Search.LuceneNET
{
    internal static class Constants
    {
        public const string IndexFolderRelativePath = "~/App_Data/Search/LuceneNET";

        public static readonly Version LuceneVersion = Version.LUCENE_30;

        public static readonly string PreviewFieldPrefix = "p_";
        public static readonly string FacetFieldPrefix = "f_";

        public static class FieldNames
        {
            public const string id = nameof(id);
            public const string entityToken = nameof(entityToken);
            public const string label = nameof(label);
            public const string version = nameof(version);
            public const string fulltext = nameof(fulltext);
            public const string source = nameof(source);
            public const string url = nameof(url);
        }
    }
}
