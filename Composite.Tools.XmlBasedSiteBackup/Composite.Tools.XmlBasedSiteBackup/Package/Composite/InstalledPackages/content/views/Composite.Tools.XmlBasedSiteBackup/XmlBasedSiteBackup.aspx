<%@ Page Language="C#" AutoEventWireup="true" CodeFile="XmlBasedSiteBackup.aspx.cs"
	Inherits="XmlBasedSiteBackup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>XmlBasedSiteBackup</title>
	<link href="Styles.css" rel="stylesheet"
		type="text/css" />
		
</head>
<body>	
	<form id="form1" runat="server">
	<table cellpadding="0" cellspacing="0" style="margin-left: auto; margin-right: auto;"
		width="560px">
		<tr>
			<td class="corner">
				<img src="Images/topleft.png" alt="" />
			</td>
			<td class="top">
			</td>
			<td class="corner">
				<img src="Images/topright.png" alt="" />
			</td>
		</tr>
		<tr>
			<td class="left">
			</td>
			<td class="backgroundfill">
				<div class="title">
					<img src="Images/CompositeC1.png" alt="Composite C1" />
					XmlBasedSiteBackup</div>
				<h4>
					This package allows you to back up your XML-based website.</h4>
				<fieldset>
					<div>
						<asp:Button ID="CreateBackupButton" runat="server" Text="Create Backup Now" 
							OnClick="CreateBackup_Click" 
							onclientclick="this.value='Backup may take several minutes...'; self = this; window.setTimeout( function (){self.disabled = true;}, 10 ); " />
					</div>
					<br />
					<asp:ValidationSummary ID="ValidationSummary1" runat="server" />
					<br />
					<table border="0" cellpadding="3"  cellspacing="0" width="100%" class="list">
						<asp:Repeater ID="BackupList" runat="server" 
							onitemcommand="BackupList_ItemCommand">
							<HeaderTemplate>
								<tr>
									<th>
										Filename
									</th>
									<th>
										Date Created
									</th>
									<th>
										Size
									</th>
									<th>
									</th>
								</tr>
							</HeaderTemplate>
							<ItemTemplate>
								<tr>
									<td align="left">
										<a href="<%# DataBinder.Eval(Container.DataItem, "Filepath") %>">
											<%# DataBinder.Eval(Container.DataItem, "Filename") %></a>
									</td>
									<td>
										<%# DataBinder.Eval(Container.DataItem, "DateCreated") %>
									</td>
									<td>
										<%# DataBinder.Eval(Container.DataItem, "Filesize") %>
										KB
									</td>
									<td>
										<asp:ImageButton ID="DeleteImageButton" runat="server" ImageUrl="~/composite/services/Icon/GetIcon.ashx?resourceName=dataassociation-remove-association&resourceNamespace=Composite.Icons" CommandName="<%# this._deleteCommand %>" CommandArgument='<%# Eval("Filename") %>' OnClientClick='return confirm("Do you want to delete the backup?")' AlternateText="Delete" />
									</td>
								</tr>
							</ItemTemplate>
						</asp:Repeater>
					</table>
				</fieldset>
			</td>
			<td class="right">
			</td>
		</tr>
		<tr>
			<td class="corner">
				<img src="Images/bottomleft.png" alt="" />
			</td>
			<td class="bottom">
			</td>
			<td class="corner">
				<img src="Images/bottomright.png" alt="" />
			</td>
		</tr>
	</table>
	</form>
</body>
</html>
