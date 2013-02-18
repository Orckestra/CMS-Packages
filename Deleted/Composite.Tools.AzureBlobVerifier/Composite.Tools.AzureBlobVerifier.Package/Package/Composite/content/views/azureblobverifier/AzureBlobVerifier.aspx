<?xml version="1.0" encoding="UTF-8" ?>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AzureBlobVerifier.aspx.cs" Inherits="C1AzureBlobVerifier.AzureBlobVerifier" %>

<%@ Register TagPrefix="control" TagName="httpheaders" Src="~/Composite/controls/HttpHeadersControl.ascx" %>
<%@ Register TagPrefix="control" TagName="scriptloader" Src="~/Composite/controls/ScriptLoaderControl.ascx" %>
<%@ Register TagPrefix="control" TagName="styleloader" Src="~/Composite/controls/StyleLoaderControl.ascx" %>
<html xmlns="http://www.w3.org/1999/xhtml" xmlns:ui="http://www.w3.org/1999/xhtml" xmlns:control="http://www.composite.net/ns/uicontrol">
<control:httpheaders ID="Httpheaders1" runat="server" />
<head>
    <title>Azure Blob Verifier Result</title>
    <control:styleloader ID="Styleloader1" runat="server" />
    <control:scriptloader ID="Scriptloader1" type="sub" runat="server" />
</head>
<body>
    <ui:page label="Azure Blob Verifier Result" icon="${skin}/dialogpages/message16.png">
        <ui:scrollbox>
            <asp:PlaceHolder ID="ResultPlaceHolder" runat="server" />
        </ui:scrollbox>
    </ui:page>
</body>
</html>
