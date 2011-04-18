<%@ Control Language="C#" AutoEventWireup="true"  CodeFile="Comments.ascx.cs" Inherits="Views_Comments" %>

 
    <% foreach (var m in ViewData.Model)
       { %>
        <table>
        <tr><td>Name</td><td><%= m.Name%></td></tr>
		<tr><td>Date</td><td><%= m.Date%></td></tr>
		<tr><td>text</td><td><%= m.Text%></td></tr>
		
        </table>
    <% } %>
    
    

