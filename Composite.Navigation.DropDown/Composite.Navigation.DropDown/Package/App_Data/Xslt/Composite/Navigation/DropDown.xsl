<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="xsl in" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns="http://www.w3.org/1999/xhtml" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:csharp="http://c1.composite.net/sample/csharp">
	<xsl:param name="level" select="/in:inputs/in:param[@name='Level']" />
	<xsl:param name="depth" select="/in:inputs/in:param[@name='Depth']" />
	<xsl:param name="parent" select="/in:inputs/in:param[@name='Parent']" />
	<xsl:param name="sitemap" select="/in:inputs/in:result[@name='SitemapXml']" />
	<msxsl:script implements-prefix="csharp" language="C#">
		<msxsl:assembly name="Composite" />
		<msxsl:using namespace="Composite.Core.WebClient" />
		<![CDATA[
			public string ResolveRelativePath(string path)
			{
				if(path.StartsWith("~/"))
					return UrlUtils.PublicRootPath + path.Substring(1);
				return path;
			}
		]]>
	</msxsl:script>
	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Navigation/DropDown/DropDown.css" />
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Navigation/DropDown/Styles.css" />
				<xsl:comment>
					<xsl:text>[if IE 6]&gt;&lt;link rel="stylesheet" type="text/css" href="</xsl:text>
					<xsl:value-of select="csharp:ResolveRelativePath('~/Frontend/Composite/Navigation/DropDown/IE6.css')" />
					<xsl:text>" /&gt;&lt;![endif]</xsl:text>
				</xsl:comment>
				<script type="text/javascript" src="~/Frontend/Composite/Navigation/DropDown/Scripts/DropDown.js"></script>
			</head>
			<body>
				<xsl:apply-templates mode="Level" select="$sitemap">
					<xsl:with-param name="level" select="$level" />
				</xsl:apply-templates>
			</body>
		</html>
	</xsl:template>
	<xsl:template mode="Level" match="*">
		<xsl:param name="level" />
		<xsl:if test="$level &lt; 1">
			<div id="NavigationDropDown">
				<ul id="DropDown">
					<xsl:if test="$parent='true'">
						<xsl:apply-templates mode="Page" select="." />
					</xsl:if>
					<xsl:apply-templates mode="Page" select="./*">
						<xsl:with-param name="depth" select="$depth - 1" />
					</xsl:apply-templates>
				</ul>
				<br clear="all" />
			</div>
			<script type="text/javascript">
				DropDown('DropDown', 'hover', 25);
			</script>
		</xsl:if>
		<xsl:if test="$level &gt; 0">
			<xsl:apply-templates mode="Level" select="./*[@isopen='true']">
				<xsl:with-param name="level" select="$level - 1" />
			</xsl:apply-templates>
		</xsl:if>
	</xsl:template>
	<xsl:template mode="Page" match="*">
		<xsl:param name="depth">0</xsl:param>
		<xsl:if test="@MenuTitle != ''">
			<li>
				<xsl:if test="@isopen='true'">
					<xsl:attribute name="class">NavigationOpen</xsl:attribute>
				</xsl:if>
				<xsl:if test="@iscurrent='true'">
					<xsl:attribute name="class">NavigationSelected</xsl:attribute>
				</xsl:if>
				<a href="{@URL}">
					<xsl:if test="@isopen='true'">
						<xsl:attribute name="class">NavigationOpen</xsl:attribute>
					</xsl:if>
					<xsl:if test="@iscurrent='true'">
						<xsl:attribute name="class">NavigationSelected</xsl:attribute>
					</xsl:if>
					<xsl:value-of select="@MenuTitle" />
				</a>
				<xsl:if test="count(./*)&gt;0 and $depth != 0">
					<ul>
						<xsl:apply-templates mode="Page" select="./*">
							<xsl:with-param name="depth" select="$depth - 1" />
						</xsl:apply-templates>
					</ul>
				</xsl:if>
			</li>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>