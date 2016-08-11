<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
	<xsl:output method="xml" indent="yes"/>

	<xsl:param name="ConnectionString"></xsl:param>
	<xsl:param name="ConnectionStringName"></xsl:param>

	<xsl:template match="/configuration">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />

			<xsl:if test="count(connectionStrings)=0" xml:space="preserve">
	<connectionStrings>
		<add name="{$ConnectionStringName}" connectionString="{$ConnectionString}" />
	</connectionStrings>
</xsl:if>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="/configuration/connectionStrings">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />

			<xsl:if test="count(add[@name=$ConnectionStringName])=0" xml:space="preserve">
				<add name="{$ConnectionStringName}" connectionString="{$ConnectionString}" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="/configuration/connectionStrings/add">
		<xsl:choose>
			<xsl:when test="@name=$ConnectionStringName">
				<add name="{$ConnectionStringName}" connectionString="{$ConnectionString}" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:copy>
					<xsl:apply-templates select="@* | node()" />
				</xsl:copy>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()"/>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>
