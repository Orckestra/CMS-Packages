using System;
using System.Web.UI.WebControls;
using Composite.C1Console.Security;

namespace Composite.Tools.LegacyUrlHandler
{
	public partial class RemoveRedundantPaths : System.Web.UI.Page
	{
		protected Button btnRemoveRedundantPaths;
		protected Label lblResult;

		protected void Page_Load(object sender, EventArgs e)
		{
            if (!UserValidationFacade.IsLoggedIn())
            {
                LegacyUrlHandlerFacade.RedirectToLoginPage();
            }
		}

		protected void btnRemoveRedundantPaths_Click(object sender, EventArgs e)
		{
			var mappings = LegacyUrlHandlerFacade.GetMappingsFromXml().RawLinks;
			var siteMap = LegacyUrlHandlerFacade.GetMappingsFromSiteMap();      

			foreach (var m in siteMap)
			{
				var oldPath = m.Key;

				if (mappings.ContainsKey(oldPath))
				{
					mappings.Remove(oldPath);
				}
			}

			LegacyUrlHandlerFacade.WriteXml(mappings);
			btnRemoveRedundantPaths.Visible = false;
			lblResult.Text = "Redundant paths are removed.";
		}
	}

}