using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Composite.Plugins.PageTemplates.MasterPages;
using Composite.Core.Xml;
using Composite.Core.PageTemplates;

public partial class $safeitemrootname$ : MasterPagePageTemplate
{
    public override Guid TemplateId
    {
        get { return new Guid("$guid1$"); }
    }

    public override string TemplateTitle
    {
        get { return "$itemname$"; }
    }

    [Placeholder(Id = "content", Title = "Content", IsDefault = true)]
    public XhtmlDocument Content { get; set; }

    [Placeholder(Id = "bottom", Title = "Bottom")]
    public XhtmlDocument Bottom { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}