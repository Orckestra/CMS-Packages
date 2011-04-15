<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
	<xsl:output method="xml" indent="yes"/>

	<xsl:param name="DynamicTypeDataProviderName"></xsl:param>

	<xsl:template match="/configuration/Composite.Data.Plugins.DataProviderConfiguration/@defaultDynamicTypeDataProviderName">
		<xsl:attribute name="defaultDynamicTypeDataProviderName">
			<xsl:value-of select="$DynamicTypeDataProviderName"/>
		</xsl:attribute>
	</xsl:template>

	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()"/>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>
