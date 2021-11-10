<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/system.webServer/staticContent">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(remove[@fileExtension='.webp'])=0">
				<remove fileExtension=".webp" />
			</xsl:if>
			<xsl:if test="count(mimeMap[@fileExtension='.webp'])=0">
				<mimeMap fileExtension=".webp" mimeType="image/webp" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>