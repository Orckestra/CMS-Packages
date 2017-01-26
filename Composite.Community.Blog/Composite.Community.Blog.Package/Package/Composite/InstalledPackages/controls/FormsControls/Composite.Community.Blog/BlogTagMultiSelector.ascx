<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BlogTagMultiSelector.ascx.cs" Inherits="BlogTagMultiSelector" %>
<%@ Import Namespace="Composite.Community.Blog" %>

<asp:Repeater ID="tagTypesRepeater" runat="server">
	<ItemTemplate>
    <h3><%# Eval("Name") %></h3>
   <asp:Repeater ID="tagsRepeater" runat="server" DataSource='<%# GetTags(Container.DataItem) %>'>
      <ItemTemplate>
	      <aspui:CheckBox ID="CheckBox" runat="server"
              ItemLabel='<%# System.Web.HttpUtility.HtmlEncode((string)Eval("Tag")) %>'
              Text='<%# System.Web.HttpUtility.HtmlEncode((string)Eval("Tag")) %>'
              Checked='<%# IsChecked(((Tags)Container.DataItem).Tag) %>'/>
      </ItemTemplate>
    </asp:Repeater>
  </ItemTemplate>
</asp:Repeater>
