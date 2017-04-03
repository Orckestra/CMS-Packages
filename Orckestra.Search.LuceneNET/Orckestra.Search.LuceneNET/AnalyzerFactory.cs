using System.Globalization;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.AR;
using Lucene.Net.Analysis.CJK;
using Lucene.Net.Analysis.Cn;
using Lucene.Net.Analysis.Cz;
using Lucene.Net.Analysis.El;
using Lucene.Net.Analysis.Fa;
using Lucene.Net.Analysis.Snowball;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Th;

namespace Orckestra.Search.LuceneNET
{
    internal class AnalyzerFactory
    {
        public Analyzer GetAnalyzer(CultureInfo culture)
        {
            string cultureName = culture.Name;
            var stemmerName = GetSnowBallAnalyzerStemmerName(cultureName);

            if (stemmerName != null)
            {
                return new SnowballAnalyzer(Constants.LuceneVersion, stemmerName);
            }

            return GetContribAnalyzer(cultureName) 
                   ?? new StandardAnalyzer(Constants.LuceneVersion);
        }

        private string GetSnowBallAnalyzerStemmerName(string name)
        {
            if (name.StartsWith("da-")) return "Danish";
            if (name.StartsWith("nl-")) return "Dutch";
            if (name.StartsWith("en-")) return "English";
            if (name.StartsWith("fi-")) return "Finnish";
            if (name.StartsWith("fr-")) return "French";
            if (name.StartsWith("de-")) return "German";
            if (name.StartsWith("hu-")) return "Hungarian";
            if (name.StartsWith("it-")) return "Italian";
            if (name.StartsWith("nb-") || name.StartsWith("nn-")) return "Norwegian";
            if (name.StartsWith("pt-")) return "Portuguese";
            if (name.StartsWith("ro-")) return "Romanian";
            if (name.StartsWith("ru-")) return "Russian";
            if (name.StartsWith("es-")) return "Spanish";
            if (name.StartsWith("sv-")) return "Swedish";
            if (name.StartsWith("tr-")) return "Turkish";

            return null;
        }

        private Analyzer GetContribAnalyzer(string name)
        {
            if (name.StartsWith("ar-")) return new ArabicAnalyzer(Constants.LuceneVersion);
            if (name.StartsWith("ja-") || name.StartsWith("ko-"))
                return new CJKAnalyzer(Constants.LuceneVersion);
            if (name.StartsWith("zh-")) return new ChineseAnalyzer();
            if (name.StartsWith("cs-")) return new CzechAnalyzer(Constants.LuceneVersion);
            if (name.StartsWith("el-")) return new GreekAnalyzer(Constants.LuceneVersion);
            if (name.StartsWith("fa-")) return new PersianAnalyzer(Constants.LuceneVersion);
            if (name.StartsWith("th-")) return new ThaiAnalyzer(Constants.LuceneVersion);

            return null;
        }
    }
}
