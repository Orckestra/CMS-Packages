<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml" 
	xmlns:x="http://www.w3.org/1999/xhtml" 
	xmlns:parser="#MarkupParserExtensions" 
	exclude-result-prefixes="xsl in lang f parser">
	
	<!-- these variables are inserted on the "Function Calls" tab -->
	<xsl:variable name="root" select="/in:inputs/in:result[@name='SitemapXml']"/>
	<xsl:variable name="allspots" select="/in:inputs/in:result[@name='GetAllSpotsXml']"/>
	<xsl:variable name="pagespots" select="/in:inputs/in:result[@name='GetPageSpotsXml']"/>
	
	<!-- this variable is local to the stylesheet -->
	<xsl:variable name="current" select="$root/descendant::Page[@iscurrent='true']"/>
	
	<!-- output -->
	<xsl:template match="/">
		<html>
			<head/>
			<body>
				<xsl:apply-templates select="$current"/>
			</body>
		</html>
	</xsl:template>
	
	<!-- from current page, travel upwards in sitemap in search of defined spots -->
	<xsl:template match="Page">
		<xsl:variable name="id" select="@Id"/>
		<xsl:variable name="hit" select="$pagespots/PageSpots[@PageId.Id=$id]"/>
		<xsl:choose>
			<xsl:when test="$hit/@Spots!=''">
				<xsl:call-template name="renderSpot">
					<xsl:with-param name="spots" select="$hit/@Spots"/>
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:apply-templates select="parent::Page"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<!-- recursively render spots from a string of GUIDs -->
	<xsl:template name="renderSpot">
		
		<xsl:param name="spots"/>
		<xsl:variable name="guid">
			<xsl:choose>
				<xsl:when test="contains($spots,',')">
					 <xsl:value-of select="substring-before($spots,',')"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$spots"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:copy-of select="parser:ParseXhtmlBodyFragment($allspots/AllSpots[@Id=$guid]/@Content)/x:html/x:body/*"/>
		<xsl:if test="contains($spots,',')">
			<xsl:call-template name="renderSpot">
				<xsl:with-param name="spots" select="substring-after($spots,',')"/>
			</xsl:call-template>
		</xsl:if>
		
	</xsl:template>

</xsl:stylesheet>
