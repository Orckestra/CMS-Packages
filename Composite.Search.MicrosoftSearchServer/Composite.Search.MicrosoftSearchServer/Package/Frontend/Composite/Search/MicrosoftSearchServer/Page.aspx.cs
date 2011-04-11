using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Composite.Search.MicrosoftSearchServer;
using Composite.Data.Types;
using Composite.Core.WebClient;
using Composite.Core.Xml;

public partial class Frontend_Composite_Search_MicrosoftSearchServer_Page : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		var output = new StringBuilder();
		using(var writer = new HtmlTextWriter(new StringWriter(output)))
		{
			Server.Execute("~/Renderers/Page.aspx", writer);
		}
		var document = XDocument.Parse(Regex.Replace(output.ToString(), @"xmlns:(\w*)=""http://www.w3.org/1999/xhtml""", ""));

		foreach (var a in document.Descendants(Namespaces.Xhtml + "a"))
		{
			if (a.Attribute("href") != null)
			{
				var href = a.Attribute("href").Value;
				var re = new Regex(@"ShowMedia.ashx\?(.*)");
				var match = re.Match(href);
				if(match.Success)
				{
					try
					{
						var querystring = HttpUtility.ParseQueryString(match.Groups[1].Value);
						a.SetAttributeValue("href", href.Replace("ShowMedia.ashx?", string.Format("ShowMedia.ashx/{0}?", Regex.Replace(MediaUrlHelper.GetFileFromQueryString(querystring).FileName, @"[^\w\d.]", ""))));
					}
					catch
					{ }
				}
			}
		};

		foreach (var el in document.Descendants().Where(d => d.Attribute("class") != null && d.Attribute("class").Value.ToLower().Contains("noindex")).Reverse())
		{
			el.Remove();
		}
		Response.Write(document.ToString());
	}
}