<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="xsl in"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml">

	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Navigation/Path/Styles.css" />
			</head>
			<body>
				<div id="NavigationPath">
					<xsl:apply-templates mode="Level" select="/in:inputs/in:result[@name='SitemapXml']">
						<xsl:with-param name="level" select="/in:inputs/in:param[@name='Level']" />
					</xsl:apply-templates>
				</div>
			</body>
		</html>
	</xsl:template>

	<xsl:template mode="Level" match="*">
		<xsl:param name="level" />
		<xsl:if test="$level &lt; 1">
			<xsl:apply-templates mode="Page" select="./*[@isopen='true']" />
		</xsl:if>
		<xsl:if test="$level &gt; 0">
			<xsl:apply-templates mode="Level" select="./*[@isopen='true']">
				<xsl:with-param name="level" select="$level - 1" />
			</xsl:apply-templates>
		</xsl:if>
	</xsl:template>

	<xsl:template mode="Page" match="*">
		<xsl:variable name="MenuTitle">
			<xsl:if test="@MenuTitle != ''">
				<xsl:value-of select="@MenuTitle" />
			</xsl:if>
			<xsl:if test="not(@MenuTitle != '')">
				<xsl:value-of select="@Title" />
			</xsl:if>
		</xsl:variable>
		<xsl:if test="not(@iscurrent='true')">
			<a href="{@URL}">
				<xsl:value-of select="$MenuTitle" />
			</a> :
			<xsl:if test="count(./*)>0 ">
				<xsl:apply-templates mode="Page" select="./*[@isopen='true']" />
			</xsl:if>
		</xsl:if>
		<xsl:if test="@iscurrent='true'">
			<xsl:value-of select="$MenuTitle" />
		</xsl:if>
	</xsl:template>

</xsl:stylesheet>
