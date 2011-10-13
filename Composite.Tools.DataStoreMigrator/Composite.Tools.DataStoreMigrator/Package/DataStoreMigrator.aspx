<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DataStoreMigrator.aspx.cs"
	Inherits="DataStoreMigrator" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Data Store Migrator</title>
	<link href="~/Frontend/Composite/Tools/DataStoreMigrator/Styles.css" rel="stylesheet"
		type="text/css" />
</head>
<body>
	<form id="mainForm" runat="server">
	<table cellpadding="0" cellspacing="0" style="margin-left: auto; margin-right: auto;">
		<tr>
			<td class="corner">
				<img src="Frontend/Composite/Tools/DataStoreMigrator/Images/topleft.png" alt="" />
			</td>
			<td class="top">
			</td>
			<td class="corner">
				<img src="Frontend/Composite/Tools/DataStoreMigrator/Images/topright.png" alt="" />
			</td>
		</tr>
		<tr>
			<td class="left">
			</td>
			<td class="backgroundfill">
				<div class="title">
					<img src="Frontend/Composite/Tools/DataStoreMigrator/Images/CompositeC1.png" alt="Composite C1" />
					Data Store Migrator</div>
				<h4>
					Using this page, you can transfer all types and data from one data provider to another.</h4>
				<fieldset>
					<div>
						Select the source and target data providers:</div>
					<table>
						<tr>
							<td>
								Source
							</td>
							<td>
								<asp:DropDownList ID="ddlSourceDataProvider" runat="server" ToolTip="Source data provider"
									OnSelectedIndexChanged="ddlSourceDataProvider_SelectedIndexChanged" AutoPostBack="true" />
							</td>
							<td>
								<asp:Button ID="btnTestOutOfBoundStringFields" OnClick="btnTestOutOfBoundStringFields_Click"
									runat="server" Text="Test Out-of-bound String Fields" Visible="False" />
								<asp:Button ID="btnTestSourceSQLConnection" OnClick="btnTestSourceSQLConnection_Click"
									runat="server" Text="Test Source SQL Connection" Visible="False" />
							</td>
						</tr>
						<tr>
							<td>
								Target
							</td>
							<td>
								<asp:DropDownList ID="ddlTargetDataProvider" runat="server" ToolTip="Target data provider"
									OnSelectedIndexChanged="ddlTargetDataProvider_SelectedIndexChanged" AutoPostBack="true" />
							</td>
							<td>
								<asp:Button ID="btnTestTargetSQLConnection" OnClick="btnTestTargetSQLConnection_Click"
									runat="server" Text="Test Target SQL Connection " Visible="False" />
							</td>
						</tr>
						<tr>
							<td colspan="3">
								<hr size="1" />
							</td>
						</tr>
						<tr>
							<td colspan="3">
								<asp:Label ID="lblResult" runat="server" />
							</td>
						</tr>
						<tr>
							<td colspan="3" align="right">
								<asp:Button ID="btnMigrate" OnClick="btnMigrate_Click" runat="server" Text="Migrate" />
							</td>
						</tr>
					</table>
				</fieldset>
			</td>
			<td class="right">
			</td>
		</tr>
		<tr>
			<td class="corner">
				<img src="Frontend/Composite/Tools/DataStoreMigrator/Images/bottomleft.png" alt="" />
			</td>
			<td class="bottom">
			</td>
			<td class="corner">
				<img src="Frontend/Composite/Tools/DataStoreMigrator/Images/bottomright.png" alt="" />
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
