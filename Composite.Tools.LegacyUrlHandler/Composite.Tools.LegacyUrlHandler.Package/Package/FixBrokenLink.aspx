<%@ Page Language="C#" AutoEventWireup="true" Inherits="Composite.Tools.LegacyUrlHandler.BrokenLinks.FixBrokenLink" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Legacy Url Handler: Fix Broken Link</title>
	<link href="/Frontend/Composite/Tools/LegacyUrlHandler/Styles.css" rel="stylesheet"
		type="text/css" />
</head>
<body>
	<form id="mainForm" runat="server">
	<table cellpadding="0" cellspacing="0" style="margin-left: auto; margin-right: auto;">
		<tr>
			<td class="corner">
				<img src="/Frontend/Composite/Tools/LegacyUrlHandler/Images/topleft.png" alt="" />
			</td>
			<td class="top">
			</td>
			<td class="corner">
				<img src="/Frontend/Composite/Tools/LegacyUrlHandler/Images/topright.png" alt="" />
			</td>
		</tr>
		<tr>
			<td class="left">
			</td>
			<td class="backgroundfill">
				<div class="title">
					<img src="/Frontend/Composite/Tools/LegacyUrlHandler/Images/CompositeC1.png" alt="Composite C1" />
					Legacy Url Handler: Fix Broken Link</div>
				<h4>
					Using this page, you can fix a broken link.</h4>
				<fieldset>
					<table width="100%">
						<tr>
							<td colspan="2">
								<hr />
								<asp:Label ID="lblResult" Font-Bold="True" ForeColor="Green" runat="server" />
							</td>
						</tr>
						<tr>
							<td>
								<b>BAD URL:</b><br />
								<asp:TextBox runat="server" Width="250" ID="txtBadURL"></asp:TextBox>
							</td>
							<td>
								<b>NEW URL:</b><br />
								<asp:TextBox runat="server" Width="250" ID="txtNewURL"></asp:TextBox>
								<asp:RequiredFieldValidator ID="rfvtxtNewURL" runat="server" ErrorMessage="*" ControlToValidate="txtNewURL" />
							</td>
						</tr>
						<tr>
							<td style="text-align: right" colspan="2">
								<asp:Button ID="btnFixBadURL" OnClick="btnFixBadURL_Click" runat="server" Text="Save" />
							</td>
						</tr>
					</table>
					<hr />
					<b>New URL examples:</b>
					<ul>
						<li>/new-page-path</li>
						<li>~/page(9bdb7b6d-4e4f-41c9-bcc3-95b20bd8d796) </li>
						<li>http://external.new.url</li>
					</ul>
				</fieldset>
			</td>
			<td class="right">
			</td>
		</tr>
		<tr>
			<td class="corner">
				<img src="/Frontend/Composite/Tools/LegacyUrlHandler/Images/bottomleft.png" alt="" />
			</td>
			<td class="bottom">
			</td>
			<td class="corner">
				<img src="/Frontend/Composite/Tools/LegacyUrlHandler/Images/bottomright.png" alt="" />
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
