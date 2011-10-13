<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
	<xsl:output method="xml" indent="yes"/>

	<xsl:param name="ConnectionString"></xsl:param>
	<xsl:param name="ConnectionStringName"></xsl:param>
	
	<xsl:template match="/configuration/Composite.Data.Plugins.DataProviderConfiguration/DataProviderPlugins">
		<xsl:copy>
			<xsl:if test="count(add[@name='DynamicSqlDataProvider'])=0">
				<add connectionStringName="{$ConnectionStringName}" sqlQueryLoggingEnabled="false" sqlQueryLoggingIncludeStack="false" type="Composite.Plugins.Data.DataProviders.MSSqlServerDataProvider.SqlDataProvider, Composite" name="DynamicSqlDataProvider" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="/configuration/Composite.Data.Plugins.DataProviderConfiguration/DataProviderPlugins/add[@name='DynamicSqlDataProvider']/@connectionStringName">
		<xsl:attribute name="connectionStringName">
			<xsl:value-of select="$ConnectionStringName"/>
		</xsl:attribute>
	</xsl:template>

	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()"/>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>
