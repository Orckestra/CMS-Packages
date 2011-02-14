using System;
using System.Web.UI.WebControls;

namespace Composite.Tools.LegacyUrlHandler
{
	public partial class RemoveRedunantPaths : System.Web.UI.Page
	{
		protected Button btnRemoveRedunantPaths;
		protected Label lblResult;

		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void btnRemoveRedunantPaths_Click(object sender, EventArgs e)
		{
			var mappings = LegacyUrlHandlerFacade.GetMappingsFromXml();
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
			btnRemoveRedunantPaths.Visible = false;
			lblResult.Text = "Redunant paths are removed.";
		}
	}

}