using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;

using System.Xml;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LocalizationTool
{
	public static class Translator
	{
		public static string Translate(string input, CultureInfo sourceCulture, CultureInfo targetCulture)
		{
			string langpair = sourceCulture.TwoLetterISOLanguageName + "|" + targetCulture.TwoLetterISOLanguageName;
			return TranslateText(input, langpair);
		}

		public static string TranslateText(string text, string langpair)
		{
			string result = string.Empty;
			try
			{
				string url = String.Format("http://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", text, langpair);
				WebClient webClient = new WebClient();
				webClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-GB; rv:1.8.1.13) Gecko/20080311 Firefox/2.0.0.13");
				webClient.Headers.Add("Accept", "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5");
				webClient.Headers.Add("Accept-Language", "en-us");
				webClient.Headers.Add("Accept-Encoding", "deflate");
				webClient.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
				webClient.Headers.Add("Referer", "http://www.google.com/translate_t?langpair=" + langpair);
				webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
				webClient.Encoding = System.Text.Encoding.UTF8;

				var sFromGoo = webClient.DownloadString(url);

				Match match = Regex.Match(sFromGoo, @"<span id=result_box(.*?)>(.*?)</span></span>");
				string temp = Regex.Replace(match.Groups[2].ToString(), @"(\s*)<br(.*?)>\s*", "\n") + "</span>";

				MatchCollection maches = Regex.Matches(temp, @"<span title=(.*?)>(.*?)</span>");
				foreach (Match m in maches)
					result += m.Groups[2].ToString();

				result = HttpUtility.HtmlDecode(result);

			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("There were problems with connection to the  www.google.com. Error: {0}", ex.Message));
			}
			return result.Trim();
		}
	}
}
