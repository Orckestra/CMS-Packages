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
		using (var conn = new DataConnection())
		{
			var homepageLink = new SitemapNavigator(conn).CurrentHomePageNode.Url;
			hlHomePage.NavigateUrl = homepageLink;
		}
	}
}
