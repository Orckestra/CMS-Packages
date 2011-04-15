using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace C1AzureBlobVerifier
{
    public partial class AzureBlobVerifier : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["Repair"] == null)
            {
                XElement html = Composite.Tools.AzureBlobVerifier.HtmlPresenter.GetHtml(Request.Url.LocalPath, Request.QueryString["Path"]);

                ResultPlaceHolder.Controls.Add(new LiteralControl(html.ToString()));
            }
            else 
            {
                XElement html = Composite.Tools.AzureBlobVerifier.RepairWorker.Repair(Request.QueryString["Repair"], Request.QueryString["Path"]);

                ResultPlaceHolder.Controls.Add(new LiteralControl(html.ToString()));
            }
        }
    }
}