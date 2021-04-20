<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/system.webServer/modules">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='CompositeAdministrativeHotfix769_Chrome_Popup'])=0">
				<add name="CompositeAdministrativeHotfix769_Chrome_Popup" type="Composite.Hotfixes.Hotfix679HttpModule, App_Code" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>