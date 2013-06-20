<%@ Control Language="C#" Inherits="Composite.Plugins.Forms.WebChannel.UiControlFactories.BoolSelectorTemplateUserControlBase" %>
<%@ Import Namespace="Composite.Plugins.Forms.WebChannel.UiControlFactories" %>

<script runat="server">
    bool _isTrue = false;

    protected void Page_Init(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(HttpContext.Current.Request.Form[this.ClientID]) == false)
        {
            _isTrue = ("true" == HttpContext.Current.Request.Form[this.ClientID]);
        }
    }

    protected override void BindStateToProperties()
    {
        this.IsTrue = _isTrue;
    }

    protected override void InitializeViewState()
    {
        _isTrue = this.IsTrue;
    }

    public override string GetDataFieldClientName()
    {
        return this.ClientID;
    }
</script>

<fieldset>
    <input type="radio" name="<%= this.ClientID %>" value="true" <%= this.IsTrue?"checked=\"checked\"":"" %> /><%= this.TrueLabel %><br />
    <input type="radio" name="<%= this.ClientID %>" value="false" <%= this.IsTrue?"":"checked=\"checked\"" %> /><%= this.FalseLabel %>
</fieldset>
