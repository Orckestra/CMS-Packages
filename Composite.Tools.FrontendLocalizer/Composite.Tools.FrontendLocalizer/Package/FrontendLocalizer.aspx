<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FrontendLocalizer.aspx.cs" Inherits="Frontend.FrontendLocalizer" UnobtrusiveValidationMode="None" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Frontend Localizer</title>
    <style type="text/css">
        .padded {
            padding: 30px;
            float: left;
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

        .info {
            color: #666;
        }

        .error {
            color: red;
        }
    </style>
</head>
<body>
    <form id="mainForm" runat="server">
        <div class="padded">
            <h1>Frontend Localizer</h1>
            <h4>This package allows you to localize all XSLT functions and templates used on your
					website.
            </h4>
            <div class="boxed">
                <asp:ValidationSummary ID="Validation" CssClass="error" runat="server" />

                <asp:Label ID="lblFileNameLabel" runat="server" CssClass="form-label">Resource file name:</asp:Label>
                App_GlobalResources/<asp:TextBox ID="txtFileName" runat="server" MaxLength="256" CssClass="form-control">Localization</asp:TextBox>
                <asp:RegularExpressionValidator
                    ID="valFileName" runat="server" ControlToValidate="txtFileName" ErrorMessage="*"
                    CssClass="error" ValidationExpression="^[a-zA-Z0-9]{1,256}$" /><asp:Label ID="lblFileName"
                        runat="server" />.resx
               <p>
                   <asp:Label ID="lblResult" runat="server" Font-Bold="true" />
               </p>
                <hr />
                <p class="info">
                    <strong>Important:</strong> All the modified files will be backed up and stored in '~/App_Data/Backups/FrontendLocalizer'
                </p>
            </div>
            <asp:Button ID="Localize" runat="server" CssClass="form-btn" OnClick="Localize_Click" Text="Localize" />
        </div>

    </form>
</body>
</html>
