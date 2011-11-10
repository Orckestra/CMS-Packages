<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml" 
	xmlns:parser="#MarkupParserExtensions" 
	xmlns:x="http://www.w3.org/1999/xhtml" 
	exclude-result-prefixes="xsl in lang f parser">

	<xsl:variable name="spots" select="/in:inputs/in:result[@name='GetAllSpotsXml']"/>

	<xsl:template match="/">
		<html>
			<head/>
			<body>
				<xsl:apply-templates select="$spots/AllSpots"/>
			</body>
		</html>
	</xsl:template>
	
	<xsl:template match="AllSpots">
		<xsl:copy-of select="parser:ParseWellformedDocumentMarkup(@Content)/x:html/x:body/*"/>
	</xsl:template>

</xsl:stylesheet>
