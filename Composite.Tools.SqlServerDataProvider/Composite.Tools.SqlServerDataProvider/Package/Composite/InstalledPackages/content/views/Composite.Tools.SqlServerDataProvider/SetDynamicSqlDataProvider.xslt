<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
	<xsl:output method="xml" indent="yes"/>

	<xsl:param name="ConnectionString"></xsl:param>

	<xsl:template match="/configuration/Composite.Data.Plugins.DataProviderConfiguration/DataProviderPlugins">
		<xsl:copy>
			<xsl:if test="count(add[@name='DynamicSqlDataProvider'])=0">
				<add connectionString="{$ConnectionString}" sqlQueryLoggingEnabled="false" sqlQueryLoggingIncludeStack="false" type="Composite.Plugins.Data.DataProviders.MSSqlServerDataProvider.SqlDataProvider, Composite" name="DynamicSqlDataProvider" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="/configuration/Composite.Data.Plugins.DataProviderConfiguration/DataProviderPlugins/add[@name='DynamicSqlDataProvider']/@connectionString">
		<xsl:attribute name="connectionString">
			<xsl:value-of select="$ConnectionString"/>
		</xsl:attribute>
	</xsl:template>

	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()"/>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>
