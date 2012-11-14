<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns:fb="http://www.facebook.com/plugins/" xmlns="http://www.w3.org/1999/xhtml" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:csharp="http://c1.composite.net/sample/csharp" exclude-result-prefixes="xsl in lang f fb msxsl csharp">
	<xsl:param name="urlToLike" select="/in:inputs/in:param[@name='URL']" />
	<xsl:param name="sendButton" select="/in:inputs/in:param[@name='SendButton']" />
	<xsl:param name="layout" select="/in:inputs/in:param[@name='LayoutStyle']" />
	<xsl:param name="width" select="/in:inputs/in:param[@name='Width']" />
	<xsl:param name="showFaces" select="/in:inputs/in:param[@name='ShowFaces']" />
	<xsl:param name="verbToDisplay" select="/in:inputs/in:param[@name='VerbToDisplay']" />
	<xsl:param name="colorScheme" select="/in:inputs/in:param[@name='ColorScheme']" />
	<xsl:param name="font" select="/in:inputs/in:param[@name='Font']" />
	<xsl:param name="culture" select="csharp:FixCurrentCultureString(/in:inputs/in:result[@name='CurrentCulture'])" />
	<xsl:variable name="isCultureSupported" select="/in:inputs/in:result[@name='LoadUrl']/locales//*[.=$culture]" />
	<xsl:variable name="facebookLocale">
		<xsl:choose>
			<xsl:when test="$isCultureSupported">
				<xsl:value-of select="$culture" />
			</xsl:when>
			<xsl:otherwise>en_US</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<xsl:template match="/">
		<html>
			<head>
				<script id="facebook-all-js" src="http://connect.facebook.net/{$facebookLocale}/all.js#xfbml=1"></script>
			</head>
			<body>
				<div>
					<fb:like href="{$urlToLike}" send="{$sendButton}" layout="{$layout}" width="{$width}" show_faces="{$showFaces}" colorscheme="{$colorScheme}" action="{$verbToDisplay}" font="{$font}"></fb:like>
				</div>
			</body>
		</html>
	</xsl:template>
	<msxsl:script implements-prefix="csharp" language="C#">
		public string FixCurrentCultureString(string culture)
		{
		return culture.Replace("-","_");
		}
	</msxsl:script>
</xsl:stylesheet>