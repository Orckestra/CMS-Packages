<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
 xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
 xmlns:msxsl="urn:schemas-microsoft-com:xslt"
 exclude-result-prefixes="msxsl">

	<xsl:output method="xml" indent="yes"/>

	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()"/>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/appSettings/add[@key='ChartImageHandler']" />
	<xsl:template match="configuration/system.webServer/handlers/add[@name='ChartImageHandler']" />
	<xsl:template match="configuration/system.web/httpHandlers/add[@path='ChartImg.axd']" />

</xsl:stylesheet>


