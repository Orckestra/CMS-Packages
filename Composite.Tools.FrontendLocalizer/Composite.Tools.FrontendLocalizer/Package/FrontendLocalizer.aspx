<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FrontendLocalizer.aspx.cs" Inherits="Frontend.FrontendLocalizer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Frontend Localizer</title>
	<link href="Frontend/Composite/Tools/FrontendLocalizer/Styles.css" rel="stylesheet"
		type="text/css" />
</head>
<body>
	<form id="mainForm" runat="server">
	<table cellpadding="0" cellspacing="0" style="margin-left: auto; margin-right: auto;"
		width="560px">
		<tr>
			<td class="corner">
				<img src="Frontend/Composite/Tools/FrontendLocalizer/Images/topleft.png" alt="" />
			</td>
			<td class="top">
			</td>
			<td class="corner">
				<img src="Frontend/Composite/Tools/FrontendLocalizer/Images/topright.png" alt="" />
			</td>
		</tr>
		<tr>
			<td class="left">
			</td>
			<td class="backgroundfill">
				<div class="title">
					<img src="Frontend/Composite/Tools/FrontendLocalizer/Images/CompositeC1.png" alt="Composite C1" />
					Frontend Localizer</div>
				<h4>
					This package allows you to localize all XSLT functions and templates used on your
					website.
				</h4>
				<fieldset>
					<asp:ValidationSummary ID="Validation" runat="server" />
					<table>
						<tr>
							<td>
								<asp:Label ID="lblFileNameLabel" runat="server">Resource file name:</asp:Label>
							</td>
							<td>
								App_GlobalResources/<asp:TextBox ID="txtFileName" runat="server" MaxLength="256">Localization</asp:TextBox><asp:RegularExpressionValidator
									ID="valFileName" runat="server" ControlToValidate="txtFileName" ErrorMessage="*"
									CssClass="error" ValidationExpression="^[a-zA-Z0-9]{1,256}$" /><asp:Label ID="lblFileName"
										runat="server" />.resx
							</td>
						</tr>
						<tr>
							<td colspan="3">
								<hr size="1" />
							</td>
						</tr>
						<tr>
							<td>
								<asp:Label ID="lblResult" runat="server" />
							</td>
							<td align="right">
								<asp:Button ID="Localize" runat="server" OnClick="Localize_Click" Text="Localize" />
							</td>
						</tr>
					</table>
					<div class="error">
						Important: All the modified files will be backed up and stored in '~/App_Data/Backups/FrontendLocalizer'
					</div>
				</fieldset>
			</td>
			<td class="right">
			</td>
		</tr>
		<tr>
			<td class="corner">
				<img src="Frontend/Composite/Tools/FrontendLocalizer/Images/bottomleft.png" alt="" />
			</td>
			<td class="bottom">
			</td>
			<td class="corner">
				<img src="Frontend/Composite/Tools/FrontendLocalizer/Images/bottomright.png" alt="" />
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
