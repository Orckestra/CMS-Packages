using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Composite.Plugins.PageTemplates.MasterPages;
using Composite.Core.PageTemplates;
using Composite.Core.Xml;

public partial class Frontpage : MasterPagePageTemplate
{
    public override Guid TemplateId
    {
        get { return new Guid("a270f819-0b5c-4f7e-9194-4b554043e4ab"); }
    }

    [Placeholder(Id = "content", IsDefault = true)]
    public XhtmlDocument Content { get; set; }

    [Placeholder(Id = "aside")]
    public XhtmlDocument Aside { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
