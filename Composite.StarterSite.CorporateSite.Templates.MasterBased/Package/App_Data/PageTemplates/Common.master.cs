using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Composite.Data;

public partial class Common : System.Web.UI.MasterPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
		html_tag.Attributes.Add("lang", System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
		using (var conn = new DataConnection())
		{
			var homepageLink = new SitemapNavigator(conn).CurrentHomePageNode.Url;
			hlHomePage.NavigateUrl = homepageLink;
		}
	}
}
