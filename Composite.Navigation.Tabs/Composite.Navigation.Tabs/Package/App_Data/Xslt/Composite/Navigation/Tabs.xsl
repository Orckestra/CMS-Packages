<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
	<xsl:param name="sitemap" select="/in:inputs/in:result[@name='SitemapXml']/Page[@isopen='true']" />
	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Navigation/Tabs/Styles.css" />
			</head>
			<body>
				<div id="tbTabs" class="tbTabs">
					<xsl:apply-templates mode="Tab" select="$sitemap" />
					<xsl:apply-templates mode="Tab" select="$sitemap/*" />
				</div>
			</body>
		</html>
	</xsl:template>
	<xsl:template mode="Tab" match="*">
		<xsl:if test="@MenuTitle != ''">
			<div>
				<xsl:choose>
					<xsl:when test="@iscurrent = 'true'">
						<xsl:attribute name="class">tbTopMenuContainerSelected</xsl:attribute>
						<xsl:attribute name="id">tbTopMenuContainerSelected</xsl:attribute>
					</xsl:when>
					<xsl:otherwise>
						<xsl:attribute name="class">tbTopMenuContainer</xsl:attribute>
					</xsl:otherwise>
				</xsl:choose>
				<div class="tbTopMenuItem">
					<a href="{@URL}">
						<xsl:value-of select="@MenuTitle" />
					</a>
				</div>
			</div>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>