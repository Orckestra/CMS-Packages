<%@ Page Language="C#" AutoEventWireup="true" Inherits="Composite.Tools.LegacyUrlHandler.StoreCurrentPaths" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Legacy Url Handler: Store Current Paths</title>
	<link href="/Frontend/Composite/Tools/LegacyUrlHandler/Styles.css" rel="stylesheet" type="text/css" />
</head>
<body>
	<form id="mainForm" runat="server">
	<table cellpadding="0" cellspacing="0" style="margin-left:auto; margin-right:auto;">
		<tr>
			<td class="corner"><img src="/Frontend/Composite/Tools/LegacyUrlHandler/Images/topleft.png" alt="" /></td>
			<td class="top"></td>
			<td class="corner"><img src="/Frontend/Composite/Tools/LegacyUrlHandler/Images/topright.png" alt="" /></td>
		</tr>
		<tr>
			<td class="left"></td>
			<td class="backgroundfill">
				<div class="title"><img src="/Frontend/Composite/Tools/LegacyUrlHandler/Images/CompositeC1.png" alt="Composite C1" /> Legacy Url Handler: Store Current Paths</div>
				<h4>Using this page, you can save all current paths to xml file.</h4>
				<fieldset>
					<div>Please press "Start" button to begin the process or <a title="Remove Redunant Paths" href="RemoveRedunantPaths.aspx">Remove Redunant Paths</a>.</div>
					<table width="100%">
					<tr>
						<td>
							<hr size="1" />
							<asp:Label id="lblResult" runat="server" />
						</td>
					</tr>
					<tr>
						<td align="right">
							<asp:Button ID="btnStoreCurrentPaths" OnClick="btnStoreCurrentPaths_Click" runat="server" Text="Start" />
						</td>
					</tr>
				</table>
				</fieldset>
			</td>
			<td class="right"></td>
		</tr>
		<tr>
			<td class="corner"><img src="/Frontend/Composite/Tools/LegacyUrlHandler/Images/bottomleft.png" alt="" /></td>
			<td class="bottom"></td>
			<td class="corner"><img src="/Frontend/Composite/Tools/LegacyUrlHandler/Images/bottomright.png" alt="" /></td>
		</tr>
	</table>
	</form>
</body>
</html>