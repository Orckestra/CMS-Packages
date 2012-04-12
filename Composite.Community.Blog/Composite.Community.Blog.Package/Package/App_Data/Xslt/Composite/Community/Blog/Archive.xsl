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
				<xsl:variable name="blogArchive" select="/in:inputs/in:result[@name='GetArchiveXml']/BlogEntries" />
				<xsl:if test ="count($blogArchive)>0">
					<ul id="BlogArchive">
						<xsl:for-each select="$blogArchive">
							<xsl:sort select="@Date" order="descending" />
							<li>
								<a title="{be:CustomDateFormat(@Date, 'MMMM yyyy')}" href="~/page({$blogPage})/{be:CustomDateFormat(@Date, 'yyyy/MM', 'en-US')}">
									<xsl:value-of select="be:CustomDateFormat(@Date, 'MMMM yyyy')" /> (<xsl:value-of select="@Count" />)
								</a>
							</li>
						</xsl:for-each>
					</ul>
				</xsl:if>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>
