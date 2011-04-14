<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="xsl in lang"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml">

	<xsl:template match="/">
		<html>
			<head>
			</head>
			<body>
				<asp:form xmlns:asp="http://www.composite.net/ns/asp.net/controls">
					<xsl:copy-of select="/in:inputs/in:result[@name='GetForm']/*" />
				</asp:form>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>
