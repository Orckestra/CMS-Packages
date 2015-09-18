<%@ Page Language="C#" AutoEventWireup="true" CodeFile="DataStoreMigrator.aspx.cs"
    Inherits="DataStoreMigrator" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Data Store Migrator</title>
    <style type="text/css">
        .padded {
            padding: 30px;
        }

        .boxed {
            border: solid 1px #ccc;
            border-radius: 5px;
            background: #f7f7f7;
            padding: 20px;
            margin-bottom: 10px;
        }

        .form-btn {
            border: solid 1px #999;
            border-radius: 4px;
            background: #fff;
            color: #333;
            padding: 6px 15px;
            margin: 0 0 0 3px;
            text-transform: uppercase;
        }

            .form-btn:hover {
                color: #000;
                border: solid 1px #000;
            }

        .form-label {
            display: block;
            color: #999;
            margin-bottom: 2px;
        }

        .form-control {
            width: 250px;
            border: solid 1px #ccc;
            border-radius: 4px;
            background: #fff;
            padding: 5px 7px;
            margin-bottom: 8px;
        }
    </style>
</head>
<body>
    <form id="mainForm" runat="server">
        <div class="padded">
            <h1>Data Store Migrator</h1>
            <h4>Using this page, you can transfer all types and data from one data provider to another.</h4>
            <div class="boxed">
                <div>
                    <p>
                        Select the source and target data providers:
                    </p>
                    <label class="form-label">
                        Source
                    </label>
                    <asp:DropDownList ID="ddlSourceDataProvider" CssClass="form-control" runat="server" ToolTip="Source data provider"
                        OnSelectedIndexChanged="ddlSourceDataProvider_SelectedIndexChanged" AutoPostBack="true" />
                    <asp:Button ID="btnTestOutOfBoundStringFields" OnClick="btnTestOutOfBoundStringFields_Click"
                        runat="server" Text="Test Out-of-bound String Fields" Visible="False" CssClass="form-btn" />
                    <asp:Button ID="btnTestSourceSQLConnection" OnClick="btnTestSourceSQLConnection_Click"
                        runat="server" Text="Test Source SQL Connection" Visible="False" CssClass="form-btn" />
                </div>
                <div>
                    <label class="form-label">
                        Target
                    </label>
                    <asp:DropDownList ID="ddlTargetDataProvider" CssClass="form-control" runat="server" ToolTip="Target data provider"
                        OnSelectedIndexChanged="ddlTargetDataProvider_SelectedIndexChanged" AutoPostBack="true" />
                    <asp:Button ID="btnTestTargetSQLConnection" CssClass="form-btn" OnClick="btnTestTargetSQLConnection_Click"
                        runat="server" Text="Test Target SQL Connection " Visible="False" />
                    <br />
                    <asp:Label ID="lblResult" runat="server" Font-Bold="true" />
                </div>

            </div>
            <asp:Button ID="btnMigrate" OnClick="btnMigrate_Click" CssClass="form-btn" runat="server" Text="Migrate" />
        </div>
    </form>
</body>
</html>
