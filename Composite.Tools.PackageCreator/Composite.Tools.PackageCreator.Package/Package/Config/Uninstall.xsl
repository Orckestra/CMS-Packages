<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.C1Console.Elements.Plugins.ElementProviderConfiguration/ElementProviderPlugins/add[@name='Composite.Tools.PackageCreator']" />
	<xsl:template match="/configuration/Composite.C1Console.Elements.Plugins.ElementActionProviderConfiguration/ElementActionProviderPlugins/add[@name='Composite.Tools.PackageCreator']" />
	<xsl:template match="/configuration/Composite.C1Console.Elements.Plugins.ElementProviderConfiguration/ElementProviderPlugins/add[@name='VirtualElementProvider']/VirtualElements/add[@name='Composite.Tools.PackageCreator']" />
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Tools.PackageCreator']" />
</xsl:stylesheet>