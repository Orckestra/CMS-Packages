<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="xsl in lang f">
	
	<!-- this variable is inserted on the "Function Calls" tab -->
	<xsl:variable name="culture" select="/in:inputs/in:result[@name='CurrentCulture']"/>
	
	<xsl:template match="/">
		<html>
			<head/>
			<body>
				<div id="quickmap">
					<f:function name="Omnicorp.Navigation.Sitemap">
						<f:param name="Levels" value="2"/>
					</f:function>
				</div>
				<div id="disclaimer">
					<xsl:choose>
						<xsl:when test="$culture='da-DK'">
							<p>Alle firmaer og produkter på dette demosite er fiktive. Al tænkelig sammenfald med faktiske firmaer og produkter er helt og aldeles utilsigtet.</p>
						</xsl:when>
						<xsl:otherwise>
							<p>All companies and products on this demo site are fictitious. No similarities to actual companies or products is intended or should be inferred.</p>
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>
