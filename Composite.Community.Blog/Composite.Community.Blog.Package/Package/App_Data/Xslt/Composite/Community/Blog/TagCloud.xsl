<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:be="#BlogXsltExtensionsFunction"
	exclude-result-prefixes="xsl in lang f be">
	<xsl:variable name="blogPage" select="/in:inputs/in:param[@name='BlogPage']" />
	<xsl:template match="/">
		<html>
			<head>
				<link id="BlogStyles" rel="stylesheet" type="text/css" href="~/Frontend/Composite/Community/Blog/Styles.css" />
			</head>
			<body>
				<div id="TagCloud">
					<xsl:for-each select="/in:inputs/in:result[@name='GetTagCloudXml']/Tags">
						<a title="{@Tag}" style="font-size:{@FontSize}px;" href="~/page({$blogPage})/{be:Encode(@Tag)}" rel="{@Rel}">
							<xsl:value-of select="@Tag" /> (<xsl:value-of select="@Rel" />)
						</a>
					</xsl:for-each>
				</div>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>
