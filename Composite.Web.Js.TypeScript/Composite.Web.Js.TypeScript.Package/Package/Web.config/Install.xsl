<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/system.web/httpModules">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='TypeScriptHttpModule'])=0">
				<add name="TypeScriptHttpModule" type="Composite.Web.Js.TypeScript.SassHttpModule, Composite.Web.Js.TypeScript" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/system.webServer/modules">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='TypeScriptHttpModule'])=0">
				<add name="TypeScriptHttpModule" type="Composite.Web.Js.TypeScript.TypeScriptHttpModule, Composite.Web.Js.TypeScript" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>