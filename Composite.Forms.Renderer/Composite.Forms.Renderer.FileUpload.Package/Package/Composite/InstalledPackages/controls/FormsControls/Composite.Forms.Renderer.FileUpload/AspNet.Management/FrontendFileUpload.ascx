<%@ Control Language="C#" Inherits="Composite.Plugins.Forms.WebChannel.UiControlFactories.TextInputTemplateUserControlBase" %>
<%@ Import Namespace="Composite.Plugins.Forms.WebChannel.UiControlFactories" %>
<%@ Import Namespace="Composite.C1Console.Forms.CoreUiControls" %>

<script runat="server">
	private string _currentStringValue = null;

	protected override void BindStateToProperties()
	{
		this.Text = _currentStringValue;
	}

	protected override void InitializeViewState()
	{
		_currentStringValue = this.Text;
	}

	public override string GetDataFieldClientName()
	{
		return this.UniqueID;
	}
</script>
<ui:datainput name="<%= this.UniqueID  %>" value="<%= Server.HtmlEncode(_currentStringValue) %>"
	readonly="true" />
