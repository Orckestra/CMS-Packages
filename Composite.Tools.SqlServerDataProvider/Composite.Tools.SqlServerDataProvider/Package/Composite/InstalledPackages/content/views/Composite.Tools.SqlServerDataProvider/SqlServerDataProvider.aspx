<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SqlServerDataProvider.aspx.cs"
    Inherits="SqlServerDataProvider" %>

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:control="http://www.composite.net/ns/uicontrol" xmlns:ui="http://www.w3.org/1999/xhtml">
<control:httpheaders runat="server" />
<head id="Head1" runat="server">
    <control:styleloader runat="server" />
    <control:scriptloader type="sub" runat="server" />
    <title>Data Store Migrator</title>
</head>
<body>
    <form id="mainForm" runat="server">
        <ui:page id="page" image="${icon:database}">
            <div class="padded">
                <asp:Label ID="lblAlreadySql" runat="server" Text="You already have default SQL provider"
                    Visible="false"></asp:Label>
                <asp:Wizard ID="wzdSqlMigrator" runat="server" ActiveStepIndex="0" DisplaySideBar="False" StartNextButtonStyle-CssClass="form-btn" StepNextButtonStyle-CssClass="form-btn" FinishPreviousButtonStyle-CssClass="form-btn" FinishCompleteButtonStyle-CssClass="form-btn" StepPreviousButtonStyle-CssClass="form-btn">
                    <WizardSteps>
                        <asp:WizardStep ID="WizardStepConnectionString" runat="server" Title="Step 1">
                            <ui:fieldgroup class="boxed">
                                <ui:field class="first last">
                                    <p>
                                        Before you can migrate your data to SQL Server you need a database and a login which have DBO access to the database.
                                    </p>
                                    <strong>Examples:</strong>
                                    <p>
                                        <i>Standard Security:</i><br />
                                        <code>
                                            <span class="text-primary">Data Source</span>=<span>myServerAddress;</span><span class="text-primary">Initial Catalog</span>=<span>myDataBase;</span><span class="text-primary">User Id</span>=<span>myUsername;</span><span
                                                class="text-primary">Password</span>=<span>myPassword;</span>
                                        </code>
                                    </p>
                                    <p>
                                        <i>Trusted Connection:</i>
                                        <br />
                                        <code>
                                            <span class="text-primary">Data Source</span>=<span>myServerAddress;</span><span class="text-primary">Initial Catalog</span>=<span>myDataBase;</span><span class="text-primary">Integrated Security</span>=<span>SSPI;</span>
                                        </code>
                                    </p>
                                    <ui:fielddesc>Connection String:
<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
    ControlToValidate="txtConnectionString"></asp:RequiredFieldValidator></ui:fielddesc>
                                    <asp:TextBox ID="txtConnectionString" runat="server" Columns="80" CssClass="form-control"></asp:TextBox>
                                    <br />
                                    <asp:CustomValidator ID="ConnectionStringValidator" runat="server" ErrorMessage="Connection String"
                                        ControlToValidate="txtConnectionString" OnServerValidate="ConnectionStringValidator_ServerValidate"
                                        Display="Dynamic"></asp:CustomValidator>
                                </ui:field>
                            </ui:fieldgroup>

                        </asp:WizardStep>
                        <asp:WizardStep ID="WizardStep2" runat="server" Title="Step 2">
                            <ui:fieldgroup class="boxed">
                                <ui:field class="first last">
                                    <ui:fielddesc> Source:</ui:fielddesc>
                                    <asp:DropDownList ID="ddlSourceDataProvider" runat="server" ToolTip="Source data provider" CssClass="form-control" />
                                    <asp:CustomValidator ID="SourceValidator" runat="server" OnServerValidate="SourceValidator_ServerValidate"
                                        Display="Dynamic">
                                    </asp:CustomValidator>
                                    <asp:Label ID="SourceValidatorResults" runat="server" />
                                    <script id="jquery-js" src="//code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
                                    <script type="text/javascript">
                                        $(document).ready(function () {
                                            $('input[name$="FinishButton"]').click(function () {
                                                var theatre = top.bindingMap.offlinetheatre;
                                                if (theatre) {
                                                    theatre.play(true);
                                                }
                                            });
                                        });
                                    </script>
                                </ui:field>
                            </ui:fieldgroup>
                        </asp:WizardStep>
                        <asp:WizardStep ID="WizardStep3" runat="server" StepType="Complete">
                            <ui:fieldgroup class="boxed">
                                <ui:field class="first last">
                                    <asp:Label ID="lblComplete" runat="server" Text="Data store migration completed"></asp:Label>
                                    <script type="text/javascript">
                                        parent.MessageQueue.update();
                                        var theatre = top.bindingMap.offlinetheatre;
                                        if (theatre) {
                                            theatre.stop();
                                        }
                                    </script>
                                </ui:field>
                            </ui:fieldgroup>
                        </asp:WizardStep>
                    </WizardSteps>
                </asp:Wizard>
            </div>
        </ui:page>
    </form>
</body>
</html>
