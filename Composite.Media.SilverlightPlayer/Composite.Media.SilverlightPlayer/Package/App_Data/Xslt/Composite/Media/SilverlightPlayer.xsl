<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="xsl in lang f">

	<xsl:variable name="ClientId" select="/in:inputs/in:param[@name='ClientId']/text()" />
	<xsl:variable name="Source" select="/in:inputs/in:param[@name='Source']/text()" />
	<xsl:variable name="Width" select="/in:inputs/in:param[@name='Width']/text()" />
	<xsl:variable name="Height" select="/in:inputs/in:param[@name='Height']/text()" />

	<xsl:variable name="VirtualFolderPath" select="/in:inputs/in:result[@name='ApplicationPath']/text()" />
	
	<xsl:template match="/">
		<html>
			<head />
			<body>
				<object width="{$Width}" height="{$Height}" data="data:application/x-silverlight-2," type="application/x-silverlight-2">
					<xsl:if test="$ClientId != ''">
						<xsl:attribute name="id">
							<xsl:value-of select="$ClientId" />
						</xsl:attribute>
					</xsl:if>
					<param name="source" value="{$VirtualFolderPath}{$Source}" />

					<a href="http://go.microsoft.com/fwlink/?LinkID=149156&amp;v=4.0.50401.0" style="text-decoration: none;">
						<img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" style="border-style: none" />
					</a>
				</object>
			</body>
		</html>
	</xsl:template>
	
</xsl:stylesheet>