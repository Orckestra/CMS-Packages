using Lucene.Net.Analysis;
using Lucene.Net.Search.Highlight;

namespace Orckestra.Search.LuceneNET
{
    internal class HTMLEncoder: IEncoder
    {
        private readonly IEncoder _defaultEncoder = new SimpleHTMLEncoder();

        public string EncodeText(string originalText)
        {
            var result = _defaultEncoder.EncodeText(originalText);

            // Fixing the default encoder not handling surrogate pairs correctly
            if (result.Contains("&#"))
            {
                for (int i = 0; i < originalText.Length - 1; i++)
                {
                    if (char.IsHighSurrogate(originalText[i]) && char.IsLowSurrogate(originalText[i + 1]))
                    {
                        int high = originalText[i];
                        int low = originalText[i + 1];

                        result = result.Replace(
                            Encode(high) + Encode(low), 
                            Encode(CombineSurrogateChar(low, high)));
                    }
                }
            }

            return result;
        }

        string Encode(int @char) => $"&#{@char};";

        int CombineSurrogateChar(int low, int high) => low - 56320 | (high - 55296 << 10) + 65536;
    }
}
