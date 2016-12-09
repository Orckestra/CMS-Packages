using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Orckestra.Web.Html.FontAwesome
{
	public static class InlineMethodFunction
	{
		public static IEnumerable<string> GetIconNames()
		{
			string pathtocss = System.Web.HttpContext.Current.Server.MapPath("/Frontend/Orckestra/Web/Html/FontAwesome/css/font-awesome.css");
            if (File.Exists(pathtocss))
            {
                string text = File.ReadAllText(pathtocss);
                var matches = Regex.Matches(text, @".fa-([A-Za-z0-9\-]+):before");
                foreach (Match m in matches)
                {
                    yield return m.Groups[1].Value;
                }
            }
		}
	}
}
