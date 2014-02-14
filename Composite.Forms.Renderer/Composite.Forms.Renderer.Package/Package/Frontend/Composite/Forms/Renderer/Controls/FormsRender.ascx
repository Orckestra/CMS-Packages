<%@ Control Language="C#" CodeFile="FormsRender.ascx.cs" AutoEventWireup="true" Inherits="FormsRenderer_FormsRender" %>
<div class="FormsRenderer">
    <asp:Panel runat="server" DefaultButton="Send">
        <asp:ValidationSummary ID="ValidationSummary" HeaderText="" CssClass="alert alert-danger" runat="server" />
        <asp:Literal ID="IntroText" runat="server"></asp:Literal>
        <asp:PlaceHolder ID="Fields" runat="server"></asp:PlaceHolder>
        <div id="Captcha" runat="server" class="form-group">
            <label for="CaptchaInput">
                <asp:Literal ID="CaptchaText" runat="server" />
            </label>
            <div class="row">
                <div class="col-xs-12 col-sm-3 col-md-3">
                     <asp:TextBox ID="CaptchaInput" runat="server" CssClass="form-control" />
                </div>
                <div class="col-xs-12 col-sm-3  col-md-3">
                    <asp:Image ID="CaptchaImage" runat="server" CssClass="CaptchaImage" /></div>
            </div>
        </div>
        <div id="FieldSet" runat="server" class="form-group">
            <input type="reset" id="Reset" runat="server" value="Reset" class="btn btn-default Reset" />
            <asp:Button ID="Send" runat="server" Text="Submit" OnClick="Send_Click" CssClass="btn btn-default" />
        </div>
    </asp:Panel>
</div>
