using System;
using Composite.Plugins.PageTemplates.MasterPages;
using Composite.Core.Xml;
using Composite.Core.PageTemplates;

public partial class TwoColumnsNavigationContent : MasterPagePageTemplate
{
	public override Guid TemplateId
	{
        get { return new Guid("e3851f7a-3f4b-4eda-9708-07c3b6020e08"); }
	}

	public override string TemplateTitle 
	{
        get { return "2 columns, navigation and content"; }
	}

    [Placeholder(Id = "content", IsDefault = true)]
    public XhtmlDocument Content { get; set; }

    [Placeholder(Id = "jumbotron")]
    public XhtmlDocument Jumbotron { get; set; }

	protected void Page_Load(object sender, EventArgs e)
	{
	}
}