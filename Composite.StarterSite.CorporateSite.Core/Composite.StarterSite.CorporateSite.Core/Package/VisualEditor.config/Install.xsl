<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/visualeditor/styles">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@file='../../styles/visualeditor.common.less'])=0">
				<style file="../../styles/visualeditor.common.less" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/visualeditor/styles/style[@file='../../styles/visualeditor.common.css']" />
</xsl:stylesheet>