using System.Globalization;

namespace Composite.Search.GoogleSiteSearch
{
	public class Functions
	{
		public static string GetRequestUrl(string SearchEngineId, string SearchTerm, int PageSize, int Page, bool SearchLanguageDetection)
		{
			string searchLanguage = SearchLanguageDetection ? string.Format("&lr=lang_{0}", CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower()) : string.Empty;
			return string.Format("http://www.google.com/search?output=xml&client=google-csbe&cx={0}&q={1}&num={2}&start={3}{4}", SearchEngineId, SearchTerm, PageSize, (Page - 1) * PageSize, searchLanguage);
		}
	}
}