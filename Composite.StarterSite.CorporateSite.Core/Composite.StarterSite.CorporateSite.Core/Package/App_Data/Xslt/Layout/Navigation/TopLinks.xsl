<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
  xmlns:lang="http://www.composite.net/ns/localization/1.0"
  xmlns:f="http://www.composite.net/ns/function/1.0"
  xmlns="http://www.w3.org/1999/xhtml"
  exclude-result-prefixes="xsl in lang f">


	<xsl:variable name="TopLinks" select="/in:inputs/in:result[@name='GetTopLinkXml']/TopLink" />

	<xsl:template match="/">
		<html>
			<head>
			</head>
			<body>
				<div id="TopLinks">
					<ul>
						<xsl:for-each select="$TopLinks">
							<li>
								<xsl:apply-templates select="." />
							</li>
						</xsl:for-each>
					</ul>
				</div>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="TopLink">
		<a href="~/Renderers/Page.aspx?pageId={@Page}">
			<xsl:choose>
				<xsl:when test="@Page.MenuTitle!=''">
					<xsl:value-of select="@Page.MenuTitle" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="@Page.Title" />
				</xsl:otherwise>
			</xsl:choose>
		</a>
	</xsl:template>

</xsl:stylesheet>
