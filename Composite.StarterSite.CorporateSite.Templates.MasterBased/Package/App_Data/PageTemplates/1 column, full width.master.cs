using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Composite.Core.PageTemplates;
using Composite.Core.Xml;
using Composite.Plugins.PageTemplates.MasterPages;

public partial class _1_column__full_width : MasterPagePageTemplate
{
    public override Guid TemplateId
    {
        get { return new Guid("0526ad34-c540-418e-8c23-0eec2a8da2ce"); }
    }

    public override string TemplateTitle
    {
        get { return "1 column, full width"; }
    }

    [Placeholder(Id = "content", IsDefault = true)]
    public XhtmlDocument Content { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
