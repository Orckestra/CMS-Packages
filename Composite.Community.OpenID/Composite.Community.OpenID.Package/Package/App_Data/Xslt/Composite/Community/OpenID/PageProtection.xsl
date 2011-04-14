<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:cod="#OpenIDExtensions"
	exclude-result-prefixes="xsl in lang f cod">

	<xsl:variable name="signInPage" select="/in:inputs/in:param[@name='SignInPage']" />
	<xsl:variable name="userDisplayName" select="cod:GetCurrentUserDisplayName()" />

	<xsl:template match="/">
		<html>
			<head />
			<body>
				<xsl:if test ="$userDisplayName = ''">
					<xsl:value-of select="cod:Redirect($signInPage)" />
				</xsl:if>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>