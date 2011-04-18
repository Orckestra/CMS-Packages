<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="Views_Comments_Index" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<script src="<%= Url.Content("~/Scripts/MicrosoftAjax.debug.js") %>" type="text/javascript"></script>
	<script src="<%= Url.Content("~/Scripts/MicrosoftMvcAjax.debug.js") %>" type="text/javascript"></script>
</head>
<body>
	<div id="Comments">
		<% Html.RenderPartial("Comments", ViewData["Comments"]);%>
	</div>
	<% using (Ajax.BeginForm("AddComment", new AjaxOptions { UpdateTargetId = "Comments" }))
	{ %>
	Name:
	<%= Html.TextBox("name","")%><br />
	Text
	<%= Html.TextArea("text","")%><br />
	<input type="submit" value="Submit" /><br />
	<% } %>
</body>
</html>
