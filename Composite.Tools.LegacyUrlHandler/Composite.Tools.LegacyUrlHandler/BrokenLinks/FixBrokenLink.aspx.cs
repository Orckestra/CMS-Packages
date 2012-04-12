using System;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
using Composite.C1Console.Security;
using Composite.Data;
using System.Linq;

namespace Composite.Tools.LegacyUrlHandler.BrokenLinks
{

	public partial class FixBrokenLink : System.Web.UI.Page
	{
		protected Button btnFixBadURL;
		protected Label lblResult;
		protected TextBox txtBadURL;
		protected TextBox txtNewURL;

		private string _badURL;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!UserValidationFacade.IsLoggedIn())
				Response.Redirect("~/Composite/Login.aspx?ReturnUrl=" + Request.RawUrl);
		
			if (HttpContext.Current.Request.QueryString["badURL"] == null) return;
			
			_badURL = HttpContext.Current.Request.QueryString["badURL"].ToString(CultureInfo.InvariantCulture);

			if (!Page.IsPostBack && HttpContext.Current.Request.QueryString["badURL"] != null && txtBadURL != null)
			{
				var uri = new System.Uri(_badURL);
				txtBadURL.Text = uri.AbsolutePath;
			}
		}

		protected void btnFixBadURL_Click(object sender, EventArgs e)
		{
			var oldPath = txtBadURL.Text;
			var newPath = txtNewURL.Text;
			LegacyUrlHandlerFacade.WriteXmlElement(oldPath, newPath);
			LegacyUrlHandler.Cache.Clear("LegacyUrlHandler");
			using (var conn = new DataConnection())
			{
				var itemsToDelete = conn.Get<BrokenLink>().Where(link => link.BadURL == _badURL).ToList();
				if (itemsToDelete.Any())
				{
					conn.Delete<BrokenLink>(itemsToDelete);
				}
			}
			lblResult.Text = "New URL was saved.";
		}
	}
}