<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml" 
	exclude-result-prefixes="xsl in lang f">
	
	<!-- this variable is inserted on the "Function Calls" tab -->
	<xsl:variable name="root" select="/in:inputs/in:result[@name='SitemapXml']"/>
	
	<!-- this variable is local to the stylesheet :) -->
	<xsl:variable name="section" select="$root/Page/Page[@isopen='true']"/>
	
	<xsl:template match="/">
		<html>
			<head/>
			<body>
				<h1>
					<a href="{$section/@URL}">
						<xsl:value-of select="$section/@MenuTitle"/>
					</a>
				</h1>
				<ul>
					<xsl:apply-templates select="$section/Page"/>
				</ul>
			</body>
		</html>
	</xsl:template>
	
	<xsl:template match="Page[@MenuTitle!='']">
		<li>
			<a href="{@URL}">
				<xsl:if test="@iscurrent='true'">
					<xsl:attribute name="class">selected</xsl:attribute>
				</xsl:if>
				<span>
					<xsl:value-of select="@MenuTitle"/>
				</span>
			</a>
			<xsl:if test="@isopen='true' and Page">
				<ul>
					<xsl:apply-templates select="Page"/>
				</ul>
			</xsl:if>
		</li>
	</xsl:template>
	
</xsl:stylesheet>