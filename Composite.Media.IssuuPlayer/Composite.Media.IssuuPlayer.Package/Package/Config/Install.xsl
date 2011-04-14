<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.C1Console.Elements.Plugins.ElementAttachingProviderConfiguration/ElementAttachingProviderPlugins">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='CompositeMediaIssuuPlayer'])=0">
				<add name="CompositeMediaIssuuPlayer" type="Composite.Media.IssuuPlayer.ElementProvider.IssuuAttachingProvider, Composite.Media.IssuuPlayer" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.C1Console.Elements.Plugins.ElementActionProviderConfiguration/ElementActionProviderPlugins">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='Composite.Media.IssuuPlayer'])=0">
				<add name="Composite.Media.IssuuPlayer" type="Composite.Media.IssuuPlayer.ElementProvider.IssuuElementActionProvider, Composite.Media.IssuuPlayer" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>