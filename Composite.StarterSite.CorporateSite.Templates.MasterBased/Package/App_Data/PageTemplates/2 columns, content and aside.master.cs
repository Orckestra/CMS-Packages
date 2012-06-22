using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Composite.Core.PageTemplates;
using Composite.Core.Xml;
using Composite.Plugins.PageTemplates.MasterPages;

public partial class _2_columns__content_and_aside : MasterPagePageTemplate
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

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
