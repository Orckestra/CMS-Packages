<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Print.aspx.cs" Inherits="Composite_InstalledPackages_content_views_Composite_Administration_Print_Print" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xmlns:ui="http://www.w3.org/1999/xhtml"
xmlns:control="http://www.composite.net/ns/uicontrol">
<control:httpheaders id="Httpheaders1" runat="server" />
<head runat="server">
	<title></title>
	<control:styleloader id="Styleloader1" runat="server" />
	<control:scriptloader id="Scriptloader1" type="sub" runat="server" />
</head>
<body>
	<form id="form1" runat="server">
	<ui:page id="page" image="${icon:report}">
		<ui:scrollbox id="scrollbox">
			<script type="text/javascript" language="javascript">
				function PrintFrame() {
					printFrame.focus();
					printFrame.print();
				}
			</script>
			<asp:Literal ID="lPrintFrame" runat="server" />
		</ui:scrollbox>
	</ui:page>
	</form>
</body>
</html>
