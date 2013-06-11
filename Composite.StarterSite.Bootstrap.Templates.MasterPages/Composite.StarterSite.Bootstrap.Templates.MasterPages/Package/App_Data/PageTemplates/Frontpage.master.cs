using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Composite.Plugins.PageTemplates.MasterPages;
using Composite.Core.Xml;
using Composite.Core.PageTemplates;

public partial class Frontpage : MasterPagePageTemplate
{
	public override Guid TemplateId
	{
        get { return new Guid("a270f819-0b5c-4f7e-9194-4b554043e4ab"); }
	}

	public override string TemplateTitle 
	{
		get { return "Front page"; }
	}

    [Placeholder(Id = "herounit", Title = "Heading (hero unit)")]
    public XhtmlDocument HeroUnit { get; set; }

    [Placeholder(Id = "content", Title = "Content", IsDefault = true)]
    public XhtmlDocument Content { get; set; }

    [Placeholder(Id = "aside", Title = "Aside column")]
    public XhtmlDocument Aside { get; set; }

	protected void Page_Load(object sender, EventArgs e)
	{
        foreach (Control control in this.Master.FindControl("html_tag").Controls)
        {
            var htmlContainerControl = control as HtmlContainerControl;
            if (htmlContainerControl != null && htmlContainerControl.TagName == "body")
	        {
                htmlContainerControl.Attributes.Add("id", "frontpage");
                break;
	        }
	    }
	}
}