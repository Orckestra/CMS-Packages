<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml" 
	exclude-result-prefixes="xsl in lang f">
	
	<!-- function to inherit splash image metadata to subpages! -->
	
	<!-- these variables are inserted on the "Function Calls" tab -->
	<xsl:variable name="root" select="/in:inputs/in:result[@name='SitemapXml']"/>
	<xsl:variable name="splashes" select="/in:inputs/in:result[@name='GetSplashImageXml']"/>
	
	<!-- these variables are local to the stylesheet -->
	<xsl:variable name="current" select="$root/descendant::Page[@iscurrent='true']"/>
	
	<!-- output -->
	<xsl:template match="/">
		<html>
			<head>
				<style type="text/css">
					<xsl:text>div#splash {</xsl:text>
					
					<!-- compute image URL -->
					<xsl:apply-templates select="$current"/>
					
					<!-- adjust canvas size; smaller image on section subpages -->
					<xsl:if test="$current/parent::Page/parent::Page">
						<xsl:text> height: 200px;</xsl:text>
					</xsl:if>
					
					<xsl:text> }</xsl:text>
				</style>
			</head>
			<body/>
		</html>
	</xsl:template>
	
	<!-- compute image URL -->
	<xsl:template match="Page">
		<xsl:variable name="id" select="@Id"/>
		<xsl:variable name="hit" select="$splashes/SplashImage[@PageId.Id=$id]"/>
		<xsl:choose>
			<xsl:when test="$hit/@Image.CompositePath">
				<xsl:value-of select="concat('background-image: url(&quot;',/in:inputs/in:result[@name='ApplicationPath'],'/Renderers/ShowMedia.ashx?i=',$hit/@Image.CompositePath,'&quot;);')" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates select="parent::Page"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

</xsl:stylesheet>
