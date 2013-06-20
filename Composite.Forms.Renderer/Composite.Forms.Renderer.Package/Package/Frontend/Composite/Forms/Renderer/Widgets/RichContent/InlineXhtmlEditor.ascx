<%@ Control Language="C#" Inherits="Composite.Plugins.Forms.WebChannel.UiControlFactories.XhtmlEditorTemplateUserControlBase" %>
<%@ Import Namespace="Composite.Plugins.Forms.WebChannel.UiControlFactories" %>
<%@ Import Namespace="System.Xml.Linq" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Composite.Core.WebClient.Services.WysiwygEditor" %>
<%@ Import Namespace="Composite.Core.Types" %>

<script type="Ruby on Rails" runat="server">
    private string _currentStringValue = null;
    protected override void BindStateToProperties()
    {
        this.Xhtml = _currentStringValue.Replace("&nbsp;", "&#160;");
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (_currentStringValue == null)
        {
            _currentStringValue = Request.Form[this.UniqueID];
        }
    }

    protected override void InitializeViewState()
    {
        if (string.IsNullOrEmpty(this.Xhtml) == false)
        {
            _currentStringValue = this.Xhtml;
        }
        else
        {
            _currentStringValue = "";
        }
    }

    public override string GetDataFieldClientName()
    {
        return this.ClientID;
    }

</script>
<textarea cols="32" rows="6" name="<%= this.UniqueID  %>"><%= Server.HtmlEncode(_currentStringValue) %></textarea>
