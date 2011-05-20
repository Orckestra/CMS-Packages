<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
  xmlns:lang="http://www.composite.net/ns/localization/1.0"
  xmlns:f="http://www.composite.net/ns/function/1.0"
  xmlns="http://www.w3.org/1999/xhtml"
  exclude-result-prefixes="xsl in lang f">

	<xsl:param name="sitemap" select="/in:inputs/in:result[@name='SitemapXml']/Page" />

	<xsl:template match="/">
		<html>
			<head>
			</head>

			<body>
				<div id="Menu">
					<xsl:for-each select="$sitemap[@isopen='true']">
						<p class="Menu">
							<a href="{@URL}">
								<xsl:if test="@iscurrent='true'" >
									<xsl:attribute name="class">NavigationOpen</xsl:attribute>
								</xsl:if>
								<xsl:value-of select="@MenuTitle" />
							</a>
							<xsl:apply-templates select="Page" />
						</p>
					</xsl:for-each>
				</div>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="Page">
		<xsl:if test="count(@MenuTitle)">
			<a href="{@URL}">
				<xsl:if test="@isopen='true'" >
					<xsl:attribute name="class">NavigationOpen</xsl:attribute>
				</xsl:if>
				<xsl:value-of select="@MenuTitle" />
			</a>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>
