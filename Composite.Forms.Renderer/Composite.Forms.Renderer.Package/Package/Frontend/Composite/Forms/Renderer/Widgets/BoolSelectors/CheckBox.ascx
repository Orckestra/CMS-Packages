<%@ Control Language="C#" Inherits="Composite.Plugins.Forms.WebChannel.UiControlFactories.CheckBoxTemplateUserControlBase"  %>
<%@ Import Namespace="Composite.Plugins.Forms.WebChannel.UiControlFactories" %>

<script runat="server">
	bool _isChecked = false;

	protected void Page_Init(object sender, EventArgs e)
	{
		_isChecked = (string.IsNullOrEmpty(HttpContext.Current.Request.Form[this.ClientID]) == false);
	}

	protected override void BindStateToProperties()
	{
		this.Checked = _isChecked;
	}

	protected override void InitializeViewState()
	{
		_isChecked = this.Checked;
	}

	public override string GetDataFieldClientName()
	{
		return this.ClientID;
	}
</script>

<input type="checkbox" name="<%= this.ClientID %>" id="<%= this.ClientID %>" <%= this.Checked?(" checked=\"checked\" "):"" %> class="Checkbox" />
<label for="<%= this.ClientID %>" class="Checkbox"><%= this.ItemLabel %></label>