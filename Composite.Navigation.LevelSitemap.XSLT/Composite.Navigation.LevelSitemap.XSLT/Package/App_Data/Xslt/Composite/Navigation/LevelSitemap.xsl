<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="xsl in"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml">

	<xsl:param name="level" select="/in:inputs/in:param[@name='Level']" />
	<!-- Int32 -->
	<xsl:param name="depth" select="/in:inputs/in:param[@name='Depth']" />
	<!-- Int32 -->
	<xsl:param name="parent" select="/in:inputs/in:param[@name='Parent']" />
	<!-- Boolean -->
	<xsl:param name="navigationId" select="/in:inputs/in:param[@name='NavigationId']" />
	<!-- String -->

	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Navigation/LevelSitemap/Styles.css" />
			</head>
			<body>
				<xsl:apply-templates mode="Level" select="/in:inputs/in:result[@name='SitemapXml']">
					<xsl:with-param name="level" select="/in:inputs/in:param[@name='Level']" />
				</xsl:apply-templates>
			</body>
		</html>
	</xsl:template>

	<xsl:template mode="Level" match="*">
		<xsl:param name="level" />
		<xsl:if test="$level &lt; 1">
			<div id="{$navigationId}">
				<ul class="level0">
					<xsl:if test="$parent='true'">
						<xsl:apply-templates mode="Page" select=".">
							<xsl:with-param name="level" select="0" />
						</xsl:apply-templates>
					</xsl:if>
					<xsl:apply-templates mode="Page" select="./*[@MenuTitle!='']">
						<xsl:with-param name="depth" select="$depth - 1" />
					</xsl:apply-templates>
				</ul>
			</div>
		</xsl:if>
		<xsl:if test="$level &gt; 0">
			<xsl:apply-templates mode="Level" select="./*[@isopen='true']">
				<xsl:with-param name="level" select="$level - 1" />
			</xsl:apply-templates>
		</xsl:if>
	</xsl:template>

	<xsl:template mode="Page" match="*">
		<xsl:param name="depth" >0</xsl:param>
		<xsl:param name="level" select="/in:inputs/in:param[@name='Depth'] - $depth - 1" />

		<li>
			<xsl:attribute name="class">
				<xsl:text>level</xsl:text>
				<xsl:value-of select="$level" />
				<xsl:if test = "@isopen='true'">
					<xsl:text> NavigationOpen</xsl:text>
				</xsl:if>
				<xsl:if test = "@iscurrent='true'">
					<xsl:text> NavigationSelected</xsl:text>
				</xsl:if>
				<xsl:if test="$level = '0'">
					<xsl:text> mainarea</xsl:text>
					<xsl:choose>
						<xsl:when test="$parent='true' and $depth != 0">
							<xsl:value-of select="position()+1"/>
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="position()"/>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:if>
			</xsl:attribute>
			<a href="{@URL}">
				<xsl:if test="@isopen='true'">
					<xsl:attribute name="class">NavigationOpen</xsl:attribute>
				</xsl:if>
				<xsl:if test="@iscurrent='true'">
					<xsl:attribute name="class">NavigationSelected</xsl:attribute>
				</xsl:if>
				<xsl:value-of select="@MenuTitle" />
			</a>
			<xsl:if test="count(./*)>0 and $depth != 0">
				<ul class="level{/in:inputs/in:param[@name='Depth'] - $depth}">
					<xsl:apply-templates mode="Page" select="./*[@MenuTitle!='']">
						<xsl:with-param name="depth" select="$depth - 1" />
					</xsl:apply-templates>
				</ul>
			</xsl:if>
		</li>
	</xsl:template>

</xsl:stylesheet>