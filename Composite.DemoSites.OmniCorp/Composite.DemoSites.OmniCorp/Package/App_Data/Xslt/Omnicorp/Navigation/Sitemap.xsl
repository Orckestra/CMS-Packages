<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml" 
	exclude-result-prefixes="xsl in lang f">
	
	<!-- this variable is inserted on the "Input Parameters" tab -->
	<xsl:variable name="levels" select="/in:inputs/in:param[@name='Levels']"/>
	
	<!-- this variable is inserted on the "Function Calls" tab -->
	<xsl:variable name="root" select="/in:inputs/in:result[@name='SitemapXml']"/>
	
	<!-- output -->
	<xsl:template match="/">
		<html>
			<head/>
			<body>
					<ul class="sitemap">
						<xsl:apply-templates select="$root/Page/Page[@MenuTitle]"/>
						<li>
							<span>[Other]</span>
							<ul>
								<xsl:apply-templates select="$root/Page/Page[not(@MenuTitle)]"/>
							</ul>
						</li>
					</ul>
				
			</body>
		</html>
	</xsl:template>
	
	<xsl:template match="Page">
		<xsl:if test="count(ancestor::Page) &lt;= $levels">
			<li>
				<a href="{@URL}">
				
					<xsl:if test="@iscurrent='true'">
						<xsl:attribute name="class">selected</xsl:attribute>
					</xsl:if>
					
					<!-- since not all pages have a MenuTitle, we may need to use the document title here -->
					<xsl:choose>
						<xsl:when test="@MenuTitle!=''">
							<xsl:value-of select="@MenuTitle"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="@Title"/>
						</xsl:otherwise>
					</xsl:choose>
				</a>
				<xsl:if test="Page[count(ancestor::Page) &lt;= $levels]">
					<ul>
						<xsl:apply-templates select="Page"/>
					</ul>
				</xsl:if>
			</li>
		</xsl:if>
	</xsl:template>

</xsl:stylesheet>
