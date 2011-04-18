<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Index.aspx.cs" Inherits="Views_Comments_Index" %>

<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="System.Web.Mvc.Ajax" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<script src="http://ajax.microsoft.com/ajax/4.0/1/MicrosoftAjax.js" type="text/javascript"></script>
	<script src="http://ajax.microsoft.com/ajax/mvc/1.0/MicrosoftMvcAjax.js" type="text/javascript"></script>
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
