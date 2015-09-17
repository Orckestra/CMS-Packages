<%@ Page Language="C#" AutoEventWireup="true" CodeFile="XmlBasedSiteBackup.aspx.cs"
    Inherits="XmlBasedSiteBackup" %>

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:control="http://www.composite.net/ns/uicontrol" xmlns:ui="http://www.w3.org/1999/xhtml">
<control:httpheaders runat="server" />
<head runat="server">
    <title>XmlBasedSiteBackup</title>
    <control:styleloader runat="server" />
    <control:scriptloader type="sub" runat="server" />
    <script type="text/javascript">
        var theatre = top.bindingMap.offlinetheatre;
        if (theatre) {
            theatre.stop();
        }

        function confirmDeleteXMLBasedBackup(fileName) {
            if (confirm("Do you want to delete the '" + fileName + "' backup?")) {
                document.getElementById("commandName").value = "delete";
                document.getElementById("deleteXMLBackupFile").value = fileName;
                document.forms["xmlBasedBackupForm"].submit();
            };
        };

        function createXMLBackup() {
            if (theatre) {
                theatre.play(true);
            }
            document.getElementById("commandName").value = "create";
            document.forms["xmlBasedBackupForm"].submit();
        };
    </script>
</head>
<body>
    <form id="xmlBasedBackupForm" runat="server">
        <ui:page id="page" image="${icon:save}">
            <ui:toolbar>
                <ui:toolbarbody>
                    <ui:toolbargroup>
                        <ui:toolbarbutton id="CreateBackupButton" label="Create Backup Now" image="${icon:mimetype-zip}"
                            oncommand="createXMLBackup()" />
                    </ui:toolbargroup>
                </ui:toolbarbody>
            </ui:toolbar>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            <table class="table table-bordered table-hover">
                <tbody>
                    <asp:Repeater ID="BackupList" runat="server">
                        <HeaderTemplate>
                            <tr class="head">
                                <th>Filename
                                </th>
                                <th>Date Created
                                </th>
                                <th>Size
                                </th>
                                <th></th>
                            </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <a href="<%# DataBinder.Eval(Container.DataItem, "Filepath") %>" title="Download">
                                        <%# DataBinder.Eval(Container.DataItem, "Filename") %>
                                    </a>
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "DateCreated") %>
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "Filesize") %>KB
                                </td>
                                <td>
                                    <ui:clickbutton id="delete" tooltip="Delete" image="${icon:delete}" oncommand="<%# GetConfirmationCode( Eval("Filename")) %>" class="simple-icon">
                                    </ui:clickbutton>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
            <input type="hidden" id="commandName" name="commandName" />
            <input type="hidden" id="deleteXMLBackupFile" name="deleteXMLBackupFile" />
        </ui:page>
    </form>
</body>
</html>
