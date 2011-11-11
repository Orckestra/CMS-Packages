<%@ Control Language="C#" Inherits="Composite.Plugins.Forms.WebChannel.UiControlFactories.TextInputTemplateUserControlBase" %>
<%@ Import Namespace="System.Linq" %>
<script runat="server">
	protected override void BindStateToProperties()
	{
		if (fileUpload.FileBytes.Count() > 0)
		{
			this.Text = string.Format("{0}:{1} bytes", fileUpload.FileName, fileUpload.FileBytes.Count());
		}
	}

	protected override void InitializeViewState()
	{
		
	}


	public override string GetDataFieldClientName()
	{
		return null;
	}
</script>
<asp:FileUpload runat="server" ID="fileUpload" CssClass="FileBox" onkeydown="return false;" />