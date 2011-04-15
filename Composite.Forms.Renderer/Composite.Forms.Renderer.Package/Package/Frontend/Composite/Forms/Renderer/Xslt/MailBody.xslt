<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="xsl in"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	>

	<xsl:template match="/">
		<html>
			<head>
				<title>Form Content</title>
			</head>
			<body>
				<table cellspacing="3">
					<xsl:for-each select="./*/Property">
						<tr>
							<td>
								<xsl:value-of select="@Label" />
							</td>
							<td>
								<xsl:value-of select="@Value" />
							</td>
						</tr>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>