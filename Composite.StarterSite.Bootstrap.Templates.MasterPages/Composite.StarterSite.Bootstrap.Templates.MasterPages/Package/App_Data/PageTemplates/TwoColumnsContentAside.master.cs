using System;
using Composite.Plugins.PageTemplates.MasterPages;
using Composite.Core.Xml;
using Composite.Core.PageTemplates;

public partial class TwoColumnsContentAside : MasterPagePageTemplate
{
	public override Guid TemplateId
	{
        get { return new Guid("9f096519-d21c-435e-b334-62224fde2ab3"); }
	}

	public override string TemplateTitle 
	{
        get { return "2 columns, content and aside"; }
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