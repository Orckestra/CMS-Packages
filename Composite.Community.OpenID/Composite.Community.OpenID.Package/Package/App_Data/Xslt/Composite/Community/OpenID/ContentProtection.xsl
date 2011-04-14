<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:cod="#OpenIDExtensions"
	exclude-result-prefixes="xsl in lang f cod">

	<xsl:variable name="signInPage" select="/in:inputs/in:param[@name='SignInPage']" />
	<xsl:variable name="message" select="/in:inputs/in:param[@name='Message']" />
	<xsl:variable name="contentToProtect" select="/in:inputs/in:param[@name='ContentToProtect']" />
	<xsl:variable name="userDisplayName" select="cod:GetCurrentUserDisplayName()" />
		
	<xsl:template match="/">
		<html>
			<head />
			<body>
				<div id="ContentProtection">
					<xsl:choose>
						<xsl:when test="$userDisplayName != ''">
							<xsl:copy-of select="$contentToProtect/*" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="$message" />&#160;
							<a href="~/Renderers/Page.aspx?pageId={$signInPage}&amp;returnUrl={cod:GetReturnUrl()}">
								<xsl:value-of select="cod:GetLocalized('SignInStatus', 'SignIn')" />
							</a>.
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>