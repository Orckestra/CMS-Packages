<%@ Control Language="C#" Inherits="Composite.Plugins.Forms.WebChannel.UiControlFactories.DateTimeSelectorTemplateUserControlBase" %>
<%@ Import Namespace="Composite.Plugins.Forms.WebChannel.UiControlFactories" %>

<script runat="server">
	private string _currentStringValue = null;

	protected void Page_Init(object sender, EventArgs e)
	{
		_currentStringValue = Request.Form[this.UniqueID];

		if (_currentStringValue == null)
		{
		    if (this.Date.HasValue == false)
		    {
		        _currentStringValue = "";
		    }
		    else
		    {
		        DateTime dateToUse = (this.Date.Value == DateTime.MinValue ? DateTime.Now : this.Date.Value);
    			_currentStringValue = dateToUse.ToShortDateString();
		    }
		}
	}

	protected override void BindStateToProperties()
	{
		try
		{
		    if (string.IsNullOrEmpty(_currentStringValue))
		    {
		        this.Date = null;    
		    }
		    else
		    {
			    this.Date = DateTime.Parse(_currentStringValue);
			}
		}
		catch {
			this.Date = null;
		}
	}

	protected override void InitializeViewState()
	{

	}

	public override string GetDataFieldClientName()
	{
		return this.UniqueID;
	}

</script>

<input name="<%= this.UniqueID  %>" id="<%= this.UniqueID  %>" value="<%= Server.HtmlEncode(_currentStringValue) %>"  class="InputDate" />
