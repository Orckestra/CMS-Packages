<%@ Control Language="C#" CodeFile="FormsRender.ascx.cs" AutoEventWireup="true" Inherits="FormsRenderer_FormsRender" %>
<div class="FormsRenderer">
	<asp:Panel runat="server" DefaultButton="Send">
		<asp:ValidationSummary ID="ValidationSummary" HeaderText="" runat="server" />
		<asp:Literal ID="IntroText" runat="server"></asp:Literal>
		<asp:PlaceHolder ID="Fields" runat="server"></asp:PlaceHolder>
		<fieldset id="Captcha" runat="server" class="Fields">
			<label>
				<asp:Literal ID="CaptchaText" runat="server" />
			</label>
			<asp:TextBox ID="CaptchaInput" runat="server" />
			<asp:Image ID="CaptchaImage" runat="server" CssClass="CaptchaImage" />
		</fieldset>
		<fieldset id="FieldSet" runat="server" class="Buttons">
			<input type="reset" id="Reset" runat="server" value="Reset" class="Reset" />
			<asp:Button ID="Send" runat="server" Text="Submit" OnClick="Send_Click" CssClass="Submit" />
		</fieldset>
	</asp:Panel>
</div>
