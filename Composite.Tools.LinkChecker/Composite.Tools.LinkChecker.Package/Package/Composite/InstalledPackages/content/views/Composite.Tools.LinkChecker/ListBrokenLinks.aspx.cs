using System;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Composite.C1Console.Users;
using Composite.Core;
using Composite.Core.ResourceSystem;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Tools.LinkChecker;

/// <summary>
/// Linkchecker. Searches for broken links within the Composite C1 
/// Based on contribution by JamBo - nu.Faqtz.com
/// </summary>
public partial class ListBrokenLinks : System.Web.UI.Page
{
    private static readonly string XsltFileName = "ListBrokenLinks.xslt";
    private static readonly string LogTitle = "LinkChecker";


    protected void Page_Load(object sender, EventArgs e)
    {
        using (new DataScope(DataScopeIdentifier.Administrated, UserSettings.ActiveLocaleCultureInfo))
        {
            var infoDocumentRoot = new XElement("ActionItems");

            int tickCount = Environment.TickCount;

            bool noBrokenLinks;

            try
            {
                noBrokenLinks = new BrokenLinksReport(Context).BuildBrokenLinksReport(infoDocumentRoot);
            }
            catch (Exception ex)
            {
                Log.LogError("Composite.Tools.LinkChecker", ex);

                throw;
            }

            Log.LogInformation(LogTitle, "Time spent: " + (Environment.TickCount - tickCount) + " ms");

            if (noBrokenLinks)
            {
                emptyLabelPlaceHolder.Visible = true;
                return;
            }

            XDocument newTree = TransformMarkup(infoDocumentRoot);

            visualOutput.Controls.Add(new LiteralControl(newTree.ToString()));
        }
    }




    protected static string GetResourceString(string stringId)
    {
        return StringResourceSystemFacade.GetString("Composite.Tools.LinkChecker", stringId);
    }

   

    private XDocument TransformMarkup(XElement inputRoot)
    {
        var newTree = new XDocument();

        using (XmlWriter writer = newTree.CreateWriter())
        {
            var xslTransformer = new XslCompiledTransform();
            xslTransformer.LoadFromPath(this.MapPath(XsltFileName));
            xslTransformer.Transform(inputRoot.CreateReader(), writer);
        }

        return newTree;
    }
}
