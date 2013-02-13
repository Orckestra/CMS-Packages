<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:be="#BlogXsltExtensionsFunction"
	exclude-result-prefixes="xsl in lang f be">
	<xsl:variable name="blogPage" select="/in:inputs/in:param[@name='BlogPage']" />
	<xsl:variable name="isGlobal" select="/in:inputs/in:param[@name='IsGlobal']" />
	<xsl:variable name="pageId" select="/in:inputs/in:result[@name='GetPageId']" />

	<xsl:param name="page">
		<xsl:choose>
      <xsl:when test="$blogPage='00000000-0000-0000-0000-000000000000' and $isGlobal='true'">
				<xsl:value-of select="$pageId" />
			</xsl:when>
			<xsl:when test="$blogPage='00000000-0000-0000-0000-000000000000'">
				<xsl:value-of select="$pageId" />
			</xsl:when>
			<xsl:otherwise> 
				<xsl:value-of select="$blogPage" />
			</xsl:otherwise>
		</xsl:choose>	
	</xsl:param>
		
	<xsl:template match="/">
		<html>
			<head>
				<link id="BlogStyles" rel="stylesheet" type="text/css" href="~/Frontend/Composite/Community/Blog/Styles.css" />
			</head>
			<body>
				<div id="TagCloud">
					<xsl:for-each select="/in:inputs/in:result[@name='GetTagCloudXml']/Tags">
						<a title="{@Tag}" style="font-size:{@FontSize}px;" href="~/page({$page})/{be:Encode(@Tag)}" rel="{@Rel}">
							<xsl:value-of select="@Tag" /> (<xsl:value-of select="@Rel" />)
						</a>
					</xsl:for-each>
				</div>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>
