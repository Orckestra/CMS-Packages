using System;
using Composite.Core.WebClient.Renderings.Page;

public partial class Composite_InstalledPackages_content_views_Composite_Administration_Print_Print : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		var pageId = Guid.Parse(this.Request["id"].ToString());
		var pageUrl = string.Empty;
		if (PageStructureInfo.TryGetPageUrl(pageId, out pageUrl))
		{
			lPrintFrame.Text = string.Format(@"<iframe src=""{0}"" id=""printFrame"" style=""width:100%;height:100%;background-color:white;"" onload=""PrintFrame();"" ></iframe>", pageUrl);
		}
	}
}