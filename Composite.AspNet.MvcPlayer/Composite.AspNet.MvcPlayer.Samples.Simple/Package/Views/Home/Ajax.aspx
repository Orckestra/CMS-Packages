<%@ Page Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="System.Web.Mvc.Ajax" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<script src="http://ajax.microsoft.com/ajax/4.0/1/MicrosoftAjax.js" type="text/javascript"></script>
	<script src="http://ajax.microsoft.com/ajax/mvc/1.0/MicrosoftMvcAjax.js" type="text/javascript"></script>
</head>
<body>
	<span id="Text">
		<%= DateTime.Now.ToLongTimeString()%>
	</span>
	<br />
	<%= Ajax.ActionLink("Get Date", "GetDate", new AjaxOptions{UpdateTargetId="Text" }) %><br />
	<% using (Ajax.BeginForm("SetText", new AjaxOptions { UpdateTargetId = "Text" }))
	{ %>
	Text
	<%= Html.TextBox("text","")%>
	<input type="submit" value="Set Text" /><br />
	<% } %>
	<br />
	<%= Html.ActionLink("Return to index", "Index") %><br />
</body>
</html>
