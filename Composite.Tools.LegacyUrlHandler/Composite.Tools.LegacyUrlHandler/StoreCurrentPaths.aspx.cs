using System;
using System.Web.UI.WebControls;
using Composite.C1Console.Security;

namespace Composite.Tools.LegacyUrlHandler
{
	
	public partial class StoreCurrentPaths : System.Web.UI.Page
	{
		protected Button btnStoreCurrentPaths;
		protected Label lblResult;

		protected void Page_Load(object sender, EventArgs e)
		{
		    if (!UserValidationFacade.IsLoggedIn())
		    {
                LegacyUrlHandlerFacade.RedirectToLoginPage();
		    }
		}

		protected void btnStoreCurrentPaths_Click(object sender, EventArgs e)
		{
			var mappings = LegacyUrlHandlerFacade.GetMappingsFromXml().RawLinks;
			var siteMap = LegacyUrlHandlerFacade.GetMappingsFromSiteMap();

			foreach (var m in siteMap)
			{
				var pageId = m.Value;
				var oldPath = m.Key;
				var newPath = string.Format("~/page({0})", pageId);

				if (!mappings.ContainsKey(oldPath))
				{
					mappings.Add(oldPath, newPath);
				}
			}

			LegacyUrlHandlerFacade.WriteXml(mappings);
			btnStoreCurrentPaths.Visible = false;
			lblResult.Text = "Current paths are stored. Please make required changes to website and press 'Remove Redundant Paths' url above.";
		}
	}
}