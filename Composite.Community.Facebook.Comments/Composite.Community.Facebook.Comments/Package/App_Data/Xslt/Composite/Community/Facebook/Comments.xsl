<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
xmlns:lang="http://www.composite.net/ns/localization/1.0"
xmlns:f="http://www.composite.net/ns/function/1.0"
xmlns="http://www.w3.org/1999/xhtml"
xmlns:fb="http://www.facebook.com/2008/fbml"
xmlns:msxsl="urn:schemas-microsoft-com:xslt"
xmlns:csharp="http://c1.composite.net/sample/csharp"
exclude-result-prefixes="xsl in lang f fb msxsl csharp">
	<xsl:param name="url" select="/in:inputs/in:param[@name='URL']" />
	<xsl:param name="width" select="/in:inputs/in:param[@name='Width']" />
	<xsl:param name="numPosts" select="/in:inputs/in:param[@name='NumPosts']" />
	<xsl:param name="colorScheme" select="/in:inputs/in:param[@name='ColorScheme']" />
	<xsl:param name="admins" select="/in:inputs/in:param[@name='ModeratorIDs']" />
	<xsl:param name="appid" select="/in:inputs/in:param[@name='AppID']" />
	<xsl:param name="culture" select="/in:inputs/in:result[@name='CurrentCulture']" />
	<xsl:variable name="cultureForComments" select="csharp:FixCurrentCultureString($culture)" />
	<xsl:variable name="currentUrl">
		<xsl:choose>
			<xsl:when test="$url != ''">
				<xsl:value-of select="$url" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="csharp:GetCurrentUrl()" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<xsl:template match="/">
		<html>
			<head>
				<xsl:if test="$admins !=''">
					<meta property="fb:admins" content="{$admins}" />
				</xsl:if>
				<xsl:if test="$admins !=''">
					<meta property="fb:app_id" content="{$appid}" />
				</xsl:if>
			</head>
			<body>
				<div>
					<script id="facebook-all-js" src="http://connect.facebook.net/{$cultureForComments}/all.js#xfbml=1"></script>
					<fb:comments
					href="{$currentUrl}"
					num_posts="{$numPosts}"
					colorscheme="{$colorScheme}"
					width="{$width}">
					</fb:comments>
				</div>
			</body>
		</html>
	</xsl:template>
	<msxsl:script implements-prefix="csharp" language="C#">
		<msxsl:assembly name="System.Web" />
		public string FixCurrentCultureString(string culture)
		{
		return culture.Replace("-","_");
		}
		public string GetCurrentUrl()
		{
		return System.Web.HttpContext.Current.Request.Url.ToString();
		}
	</msxsl:script>
</xsl:stylesheet>