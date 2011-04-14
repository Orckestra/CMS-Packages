using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Composite.C1Console.Security;

public partial class Composite_InstalledPackages_content_views_Composite_Forms_FormSubmitHandler_GetData : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
		EntityToken entityToken = EntityTokenSerializer.Deserialize(Request["entityToken"]);
		var result = XElement.Load(entityToken.Id);
		var table = new XElement("table");

		Response.ContentType = "application/vnd.ms-excel";

		Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.xls", Path.GetFileNameWithoutExtension(entityToken.Id)));

		var attributes = result.Elements().Attributes()
			.Where(a => !a.IsNamespaceDeclaration)
			.Select(a => a.Name).Distinct();
			
		table.Add(
			new XElement("tr",
				attributes.Select(
					a => new XElement("th",
						a.LocalName
					)
				)
			)
		);

		table.Add(
			result.Elements().Select(
				row => new XElement("tr",
					attributes.Select(
						a => new XElement("td",
							(row.Attribute(a) != null) ? row.Attribute(a).Value : string.Empty
						)
					)
				)
			)
		);

		TableLiteral.Text = table.ToString();
	}
}
