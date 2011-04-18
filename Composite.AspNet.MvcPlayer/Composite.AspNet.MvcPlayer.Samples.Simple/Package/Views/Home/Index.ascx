<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<h2>
	Hello
	<%= Html.Encode(ViewData["ToGreet"]) %>!</h2>
<%= Html.ActionLink("Ajax sample", "Ajax", "Home") %>