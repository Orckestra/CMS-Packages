<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
	<xsl:variable name="resultsPageId" select="/in:inputs/in:param[@name='ResultsPage']" />
	<xsl:variable name="searchEngineId" select="/in:inputs/in:param[@name='EngineID']" />
	<xsl:variable name="language" select="substring(/in:inputs/in:result[@name='CurrentCulture'],0,3)" />
	<xsl:template match="/">
		<html>
			<head></head>
			<body>
				<form action="/Renderers/Page.aspx?pageId={$resultsPageId}" id="cse-search-box">
					<div>
						<input type="hidden" name="cx" value="{$searchEngineId}" />
						<input type="hidden" name="cof" value="FORID:10" />
						<input type="hidden" name="ie" value="UTF-8" />
						<input type="text" name="q" size="31" />
						<input type="submit" name="sa" value="Search" />
					</div>
				</form>
				<script type="text/javascript" src="http://www.google.com/cse/brand?form=cse-search-box&amp;lang={$language}"></script>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>