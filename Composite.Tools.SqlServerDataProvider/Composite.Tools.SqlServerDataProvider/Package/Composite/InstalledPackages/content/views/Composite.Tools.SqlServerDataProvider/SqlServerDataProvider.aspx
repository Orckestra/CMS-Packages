<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SqlServerDataProvider.aspx.cs"
    Inherits="SqlServerDataProvider" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Data Store Migrator</title>
    <link href="Styles.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="mainForm" runat="server">
        <table cellpadding="0" cellspacing="0" style="margin-left: auto; margin-right: auto;">
            <tr>
                <td class="corner topleft"></td>
                <td class="top"></td>
                <td class="corner topright"></td>
            </tr>
            <tr>
                <td class="left"></td>
                <td class="backgroundfill">
                    <div class="title">
                        <img src="Images/CompositeC1.png" alt="Composite C1" />
                        SqlServer Data Provider
                    </div>
                    <asp:Label ID="lblAlreadySql" runat="server" Text="You already have default SQL provider"
                        Visible="false"></asp:Label>
                    <asp:Wizard ID="wzdSqlMigrator" runat="server" ActiveStepIndex="0" DisplaySideBar="False">
                        <WizardSteps>
                            <asp:WizardStep ID="WizardStepConnectionString" runat="server" Title="Step 1">
                                <div>
                                    Before you can migrate your data to SQL Server you need a database and a login which have DBO access to the database.
                                </div>
                                <h4>Examples:</h4>
                                <i>Standard Security:</i>
                                <div class="sourcebody">
                                    <span class="kwrd">Data Source</span>=<span>myServerAddress;</span><span class="kwrd">Initial Catalog</span>=<span>myDataBase;</span><span class="kwrd">User Id</span>=<span>myUsername;</span><span
                                        class="kwrd">Password</span>=<span>myPassword;</span>
                                </div>
                                <i>Trusted Connection:</i>
                                <div class="sourcebody">
                                    <span class="kwrd">Data Source</span>=<span>myServerAddress;</span><span class="kwrd">Initial Catalog</span>=<span>myDataBase;</span><span class="kwrd">Integrated Security</span>=<span>SSPI;</span>
                                </div>
                                <br />
                                Connection String:<br />
                                <asp:TextBox ID="txtConnectionString" runat="server" Columns="80"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Required"
                                    ControlToValidate="txtConnectionString"></asp:RequiredFieldValidator>
                                <br />
                                <asp:CustomValidator ID="ConnectionStringValidator" runat="server" ErrorMessage="Connection String"
                                    ControlToValidate="txtConnectionString" OnServerValidate="ConnectionStringValidator_ServerValidate"
                                    Display="Dynamic"></asp:CustomValidator>
                            </asp:WizardStep>
                            <asp:WizardStep ID="WizardStep2" runat="server" Title="Step 2">
                                Source:<br />
                                <asp:DropDownList ID="ddlSourceDataProvider" runat="server" ToolTip="Source data provider" />
                                <br />
                                <asp:CustomValidator ID="SourceValidator" runat="server" OnServerValidate="SourceValidator_ServerValidate"
                                    Display="Dynamic">
                                </asp:CustomValidator>
                                <asp:Label ID="SourceValidatorResults" runat="server" />
                                <script id="jquery-js" src="//code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
                                <script id="Script1" src="js/jquery.blockUI.js" type="text/javascript"></script>
                                <script type="text/javascript" language="javascript">
                                    $(document).ready(function () {
                                        $('input[name$="FinishButton"]').click(function () {
                                            $.blockUI();
                                        });
                                    });
                                </script>
                            </asp:WizardStep>
                            <asp:WizardStep ID="WizardStep3" runat="server" StepType="Complete">
                                <asp:Label ID="lblComplete" runat="server" Text="Data store migration completed"></asp:Label>
                                <script type="text/javascript" language="javascript">
                                    parent.MessageQueue.update();
                                </script>
                            </asp:WizardStep>
                        </WizardSteps>
                    </asp:Wizard>
                </td>
                <td class="right"></td>
            </tr>
            <tr>
                <td class="corner bottomleft"></td>
                <td class="bottom"></td>
                <td class="corner bottomright"></td>
            </tr>
        </table>
    </form>
</body>
</html>
