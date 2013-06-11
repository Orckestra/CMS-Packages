using System;
using Composite.Plugins.PageTemplates.MasterPages;
using Composite.Core.Xml;
using Composite.Core.PageTemplates;

public partial class ThreeColumns : MasterPagePageTemplate
{
	public override Guid TemplateId
	{
        get { return new Guid("ea2ca748-efbf-4d94-8e2a-11ce49b71cd5"); }
	}

	public override string TemplateTitle 
	{
        get { return "3 columns"; }
	}

    [Placeholder(Id = "content", IsDefault = true)]
    public XhtmlDocument Content { get; set; }

    [Placeholder(Id = "aside")]
    public XhtmlDocument Aside { get; set; }

    [Placeholder(Id = "herounit")]
    public XhtmlDocument HeroUnit { get; set; }

	protected void Page_Load(object sender, EventArgs e)
	{
	}
}