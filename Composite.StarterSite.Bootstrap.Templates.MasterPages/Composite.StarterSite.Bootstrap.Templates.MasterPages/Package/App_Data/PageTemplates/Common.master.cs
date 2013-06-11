using System;
using System.Web.UI;

public partial class Common : MasterPage
{
	protected void Page_Load(object sender, EventArgs e)
	{
        html_tag.Attributes.Add("lang", System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
    }
}
