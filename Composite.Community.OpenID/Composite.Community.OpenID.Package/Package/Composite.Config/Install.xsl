<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Functions.Plugins.XslExtensionsProviderConfiguration/XslExtensionProviders/add[@name='ConfigurationBasedXslExtensionsProvider']/xslExtensions">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='#OpenIDExtensions'])=0">
				<add name="#OpenIDExtensions" type="Composite.Community.OpenID.OpenIDExtensions, Composite.Community.OpenID" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>