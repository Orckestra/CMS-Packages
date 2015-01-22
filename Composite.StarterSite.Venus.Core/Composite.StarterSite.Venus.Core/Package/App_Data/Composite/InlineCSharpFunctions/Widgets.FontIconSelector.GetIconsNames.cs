using System;
using System.Web;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace Widgets.FontIconSelector
{
    public static class InlineMethodFunction
    {
        public static IEnumerable<string> GetIconsNames()
        {
            string pathtocss = System.Web.HttpContext.Current.Server.MapPath("/Frontend/Styles/font-awesome/font-awesome.css");
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
