using System;
using Composite.Plugins.PageTemplates.MasterPages;
using Composite.Core.Xml;
using Composite.Core.PageTemplates;

public partial class OneColumnFullWidth : MasterPagePageTemplate
{
	public override Guid TemplateId
	{
        get { return new Guid("0526ad34-c540-418e-8c23-0eec2a8da2ce"); }
	}

	public override string TemplateTitle 
	{
        get { return "1 column, full width"; }
	}

    [Placeholder(Id = "herounit", Title = "Heading (hero unit)")]
    public XhtmlDocument HeroUnit { get; set; }

    [Placeholder(Id = "content", Title = "Content", IsDefault = true)]
    public XhtmlDocument Content { get; set; }

	protected void Page_Load(object sender, EventArgs e)
	{
	}
}