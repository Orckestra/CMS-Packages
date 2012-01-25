using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Composite.Data;
using Composite.Data.Types;

namespace Composite.Forms.JotForm
{
    public static class InlineMethodFunction
    {
        public static string GetFormIdFromUrl(string URL)
        {
            System.Uri uri = new System.Uri(URL);
            var queryString = HttpUtility.ParseQueryString(uri.Query);
            return queryString["formID"] != null ? queryString["formID"] : String.Empty;
        }
    }
}
