using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Composite.Core.PageTemplates;
using Composite.Core.Xml;
using Composite.Plugins.PageTemplates.MasterPages;

public partial class _3_columns : MasterPagePageTemplate
{
    public override Guid TemplateId
    {
        get { return new Guid("ea2ca748-efbf-4d94-8e2a-11ce49b71cd5"); }
    }

    [Placeholder(Id = "content", IsDefault = true)]
    public XhtmlDocument Content { get; set; }

    [Placeholder(Id = "aside")]
    public XhtmlDocument Aside { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
