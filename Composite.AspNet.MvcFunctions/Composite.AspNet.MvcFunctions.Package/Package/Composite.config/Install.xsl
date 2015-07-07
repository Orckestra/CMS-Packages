<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Functions.Plugins.FunctionProviderConfiguration/FunctionProviderPlugins">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='MvcFunctionProvider'])=0">
				<add type="Composite.Plugins.Functions.FunctionProviders.MvcFunctions.MvcFunctionProvider, Composite.AspNet.MvcFunctions" name="MvcFunctionProvider" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>