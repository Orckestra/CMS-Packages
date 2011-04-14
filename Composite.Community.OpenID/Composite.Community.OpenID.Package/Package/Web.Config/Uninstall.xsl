<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="msxsl"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt">
	<xsl:output method="xml" indent="yes"/>
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="configuration/configSections/section[@name='dotNetOpenAuth']" />
	<xsl:template match="configuration/dotNetOpenAuth" />
</xsl:stylesheet>
