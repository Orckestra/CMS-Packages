<?xml version="1.0" encoding="UTF-8" ?>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ListBrokenLinks.aspx.cs" Inherits="ListBrokenLinks" %>
<html xmlns="http://www.w3.org/1999/xhtml" xmlns:ui="http://www.w3.org/1999/xhtml" xmlns:control="http://www.composite.net/ns/uicontrol">
    <control:httpheaders runat="server" />
    <head>
        <control:styleloader runat="server" />
        <control:scriptloader type="sub" runat="server" />
        <title><%= GetResourceString("LinkCheckerActionToken.Label") %></title>
        <link rel="stylesheet" type="text/css" href="ListBrokenLinks.css.aspx"/>
    </head>
    <body>
        <ui:page label="<%= GetResourceString("BrokenLinkReport.Title") %>" image="${icon:link}">
            <ui:toolbar id="toolbar">
                <ui:toolbarbody>
                    <ui:toolbargroup>
                        <ui:toolbarbutton oncommand="window.location.reload()" id="refreshbutton" image="${icon:refresh}" label="Refresh" />
                    </ui:toolbargroup>
                </ui:toolbarbody>
            </ui:toolbar>
            <asp:PlaceHolder runat="server" ID="visualOutput" />
            <asp:PlaceHolder runat="server" ID="emptyLabelPlaceHolder" Visible="false">
                <div id="emptylabel"><%= GetResourceString("BrokenLinkReport.NoBrokenLinksFound") %></div>
            </asp:PlaceHolder>
        </ui:page>
    </body>
</html>