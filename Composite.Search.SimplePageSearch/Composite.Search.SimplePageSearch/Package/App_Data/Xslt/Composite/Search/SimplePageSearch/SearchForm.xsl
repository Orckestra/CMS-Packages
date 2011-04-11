<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:user="urn:my-scripts"
	exclude-result-prefixes="xsl in msxsl user">

	<xsl:param name="searchButtonLabel" select="/in:inputs/in:param[@name='SearchButtonLabel']" />
	<xsl:param name="searchResultPage" select="/in:inputs/in:param[@name='SearchResultPage']" />
	<xsl:param name="searchQuery" select="/in:inputs/in:result[@name='SearchQuery']" />

	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Search/SimplePageSearch/Styles.css" />
			</head>
			<body>
				<form action="/Renderers/Page.aspx?pageId={$searchResultPage}" method="get" id="SearchForm">
					<input type="text" name="SearchQuery" maxlength="1000">
						<xsl:if test="not($searchQuery='')">
							<xsl:attribute name="value">
								<xsl:value-of select="$searchQuery" />
							</xsl:attribute>
						</xsl:if>
					</input>
					<input type="submit" value="{$searchButtonLabel}"/>
				</form>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>
