<%@ Control Language="C#" Inherits="Composite.Plugins.Forms.WebChannel.UiControlFactories.ContainerTemplateUserControlBase" %>
<%@ Import Namespace="Composite.C1Console.Forms.WebChannel" %>
<%@ Import Namespace="Composite.Plugins.Forms.WebChannel.UiControlFactories" %>

<script runat="server">
    private void Page_Init(object sender, System.EventArgs e)
    {
        this.ID = "TabPanels";

        int tabCounter = 0;
        foreach (FormControlDefinition formControlDefinition in this.FormControlDefinitions)
        {
            string uiTabStartTag = string.Format("<div class=\"tabpanel\" {0}>", CustomUiTabPanelTagParams(formControlDefinition, tabCounter));
            content.Controls.Add(new LiteralControl(uiTabStartTag));
            if (formControlDefinition.IsFullWidthControl == false)
                content.Controls.Add( new LiteralControl("<div class=\"scrollbox padded\">"));
                
            content.Controls.Add(formControlDefinition.FormControl);
            
            if (formControlDefinition.IsFullWidthControl == false)
                content.Controls.Add(new LiteralControl("</div>"));
            
            content.Controls.Add(new LiteralControl("</ui:tabpanel>"));

            tabCounter++;
        }
    }

    string CustomUiTabPanelTagParams(FormControlDefinition childControl, int listPosition)
    {
        StringBuilder tagParams = new StringBuilder();

        tagParams.AppendFormat(" id=\"{0}_tabpanel{1}\"", this.UniqueID, listPosition);

        return tagParams.ToString();
    }
</script>

<div class="tabbox">
       <asp:PlaceHolder runat="server" ID="content" />
</div>
