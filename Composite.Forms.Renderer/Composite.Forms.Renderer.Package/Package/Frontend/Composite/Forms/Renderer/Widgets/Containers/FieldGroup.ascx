<%@ Control Language="C#" Inherits="Composite.Plugins.Forms.WebChannel.UiControlFactories.ContainerTemplateUserControlBase" %>

<script runat="server">
	
	private void Page_Init(object sender, System.EventArgs e)
	{
		foreach (FormControlDefinition formControlDefinition in this.FormControlDefinitions)
		{
			Control labelPlaceHolder = new PlaceHolder();

			UiFields.Controls.Add(new LiteralControl(string.Format("<div class='FormElement{0}'>", Composite.Forms.Renderer.FormsRenderer.IsRequiredControl(formControlDefinition.FormControl.ID) ? " required" : "")));
			UiFields.Controls.Add(labelPlaceHolder);
			UiFields.Controls.Add(new LiteralControl("<div class='FormElementInput'>"));
			UiFields.Controls.Add(formControlDefinition.FormControl);
			UiFields.Controls.Add(new LiteralControl("</div>"));
			UiFields.Controls.Add(new LiteralControl("</div>"));
			labelPlaceHolder.Controls.Add(new LiteralControl(string.Format("<label for='{0}'>{1}</label>", formControlDefinition.FormControl.ClientID, Server.HtmlEncode(formControlDefinition.Label))));
		}
	}
</script>

<fieldset class="Fields">
	<asp:PlaceHolder ID="UiFields" runat="server" />
</fieldset>

