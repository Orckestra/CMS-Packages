<%@ Control Language="C#" Inherits="Composite.Plugins.Forms.WebChannel.UiControlFactories.TextInputTemplateUserControlBase"  %>
<%@ Import Namespace="Composite.Plugins.Forms.WebChannel.UiControlFactories" %>

<script runat="server">
	private string _currentStringValue = null;

	protected void Page_Init(object sender, EventArgs e)
	{
		if (_currentStringValue == null)
		{
			_currentStringValue = Request.Form[this.UniqueID];
		}
	}

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
		return this.ClientID;
	}
</script>

<textarea cols="32" rows="6" id="<%= this.ClientID %>" name="<%= this.UniqueID %>"><%= Server.HtmlEncode(_currentStringValue) %></textarea>