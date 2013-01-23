<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="/configuration/Composite.C1Console.Elements.Plugins.ElementActionProviderConfiguration/ElementActionProviderPlugins">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='StaticDataTypeCreatorActionProvider'])=0">
				<add name="StaticDataTypeCreatorActionProvider" type="Composite.Tools.StaticDataTypeCreator.StaticDataTypeCreatorActionProvider, Composite.Tools.StaticDataTypeCreator" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>