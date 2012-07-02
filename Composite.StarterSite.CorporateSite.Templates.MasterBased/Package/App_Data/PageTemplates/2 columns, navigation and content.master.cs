using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Composite.Core.PageTemplates;
using Composite.Core.Xml;
using Composite.Plugins.PageTemplates.MasterPages;

public partial class _2_columns__navigation_and_content : MasterPagePageTemplate
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

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
