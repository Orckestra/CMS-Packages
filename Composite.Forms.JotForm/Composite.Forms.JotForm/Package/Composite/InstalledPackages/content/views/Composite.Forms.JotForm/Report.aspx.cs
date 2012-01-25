using System;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Data;
using System.Linq;

public partial class Composite_InstalledPackages_content_views_Composite_Forms_JotForm_Report : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		var reportId = Guid.Parse(this.Request["id"].ToString());
		var reportType = this.Request["type"].ToString();
		var pageUrl = string.Empty;
		using (DataConnection con = new DataConnection())
		{
			var report = con.Get<Composite.Forms.JotFormReport>().Where(r => r.Id == reportId).SingleOrDefault();
			if (report != null)
			{
				switch (reportType)
				{
					case "grid": pageUrl = report.GridReportURL; break;
					case "table": pageUrl = report.TableReportURL; break;
					case "visual": pageUrl = report.VisualReportURL; break;
					case "excel": pageUrl = report.ExcelReportURL; break;
					case "csv": pageUrl = report.CsvReportURL; break;
					default: break;
				}

			}
		}
		if (!string.IsNullOrWhiteSpace(pageUrl))
		{
			lPrintFrame.Text = string.Format(@"<iframe src=""{0}"" frameborder=""0"" style=""width:100%; height:100%; min-height:450px; border:none;"" ></iframe>", pageUrl);
		}
		else
		{
			lPrintFrame.Text = "<div style='padding: 20px'><h2>Report is not available</h2><p>The URL for this report has not been specified. To remedy this, edit this report and specify the JotForm Report URL.</p></div>";
		}
	}
}