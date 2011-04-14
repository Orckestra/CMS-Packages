<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:cod="#OpenIDExtensions"
	exclude-result-prefixes="xsl in lang f cod">

	<xsl:variable name="signInPage" select="/in:inputs/in:param[@name='SignInPage']" />
	<xsl:variable name="userDetailsPage" select="/in:inputs/in:param[@name='UserDetailsPage']" />
	<xsl:variable name="pageId" select="/in:inputs/in:result[@name='GetPageId']" />
	<xsl:variable name="isAuthenticated" select="cod:IsAuthenticated()" />
	<xsl:variable name="userDisplayName" select="cod:GetCurrentUserDisplayName()" />

	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Community/OpenID/Styles.css" />
			</head>
			<body>
				<div id="SignInStatus">
					<xsl:choose>
						<xsl:when test="$isAuthenticated = 'true'">
							<xsl:value-of select="cod:GetLocalized('SignInStatus', 'Welcome')" />
							<a href="~/Renderers/Page.aspx?pageId={$userDetailsPage}">
								<xsl:value-of select="$userDisplayName" />
							</a>.
							<a href="~/Renderers/Page.aspx?pageId={$signInPage}&amp;signOut=true&amp;returnUrl={cod:GetReturnUrl()}">
								<xsl:value-of select="cod:GetLocalized('SignInStatus', 'SignOut')" />
							</a>
						</xsl:when>
						<xsl:otherwise>
							<a href="~/Renderers/Page.aspx?pageId={$signInPage}&amp;returnUrl={cod:GetReturnUrl()}">
								<xsl:value-of select="cod:GetLocalized('SignInStatus', 'SignIn')" />
							</a>
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>
