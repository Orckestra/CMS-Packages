<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml" 
	exclude-result-prefixes="xsl in lang f">
	
	<!-- this variable is inserted on the "Function Calls" tab -->
	<xsl:variable name="root" select="/in:inputs/in:result[@name='SitemapXml']"/>
	
	<xsl:template match="/">
		<html>
			<head/>
			<body>
				<ul>
					<xsl:apply-templates select="$root/Page"/>
					<xsl:apply-templates select="$root/Page/Page[@MenuTitle!='']"/>
				</ul>
			</body>
		</html>
	</xsl:template>
	
	<xsl:template match="Page">
		<li>
			<a href="{@URL}">
				<xsl:if test="@iscurrent='true'">
					<xsl:attribute name="class">selected</xsl:attribute>
				</xsl:if>
				<span>
					<xsl:value-of select="@MenuTitle"/>
				</span>
			</a>
		</li>
	</xsl:template>
	
	<xsl:template match="Page/Page">
		<li>
			<a href="{@URL}">
				<xsl:if test="@isopen='true'">
					<xsl:attribute name="class">selected</xsl:attribute>
				</xsl:if>
				<span>
					<xsl:value-of select="@MenuTitle"/>
				</span>
			</a>
		</li>
	</xsl:template>

</xsl:stylesheet>