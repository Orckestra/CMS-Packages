<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="xsl in lang f">
	
	<!-- this variable is inserted on the "Function Calls" tab -->
	<xsl:variable name="splashes" select="/in:inputs/in:result[@name='GetSplashTextXml']"/>
	
	<xsl:template match="/">
		<html>
			<head/>
			<body>
				<xsl:apply-templates select="$splashes/SplashText"/>
			</body>
		</html>
	</xsl:template>
	
	<xsl:template match="SplashText[normalize-space(@SplashText)!='']">
		<p>
			<em>
				<xsl:value-of select="@SplashText"/>
			</em>
		</p>
	</xsl:template>

</xsl:stylesheet>
