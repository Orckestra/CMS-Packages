<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
				xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
				xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
				xmlns:lang="http://www.composite.net/ns/localization/1.0"
				xmlns:f="http://www.composite.net/ns/function/1.0"
				xmlns="http://www.w3.org/1999/xhtml"
				xmlns:fb="http://www.facebook.com/plugins/"
				xmlns:msxsl="urn:schemas-microsoft-com:xslt"
				xmlns:csharp="http://c1.composite.net/sample/csharp"
				exclude-result-prefixes="xsl in lang f fb msxsl csharp">

	<xsl:param name="url" select="/in:inputs/in:param[@name='URL']"/>
	<xsl:param name="width" select="/in:inputs/in:param[@name='Width']"/>
	<xsl:param name="showfaces" select="/in:inputs/in:param[@name='ShowFaces']"/>
	<xsl:param name="borderColor" select="/in:inputs/in:param[@name='BorderColor']"/>
	<xsl:param name="stream" select="/in:inputs/in:param[@name='Stream']"/>
	<xsl:param name="header" select="/in:inputs/in:param[@name='Header']"/>
	<xsl:param name="colorscheme" select="/in:inputs/in:param[@name='ColorScheme']"/>
	<xsl:param name="force_wall" select="/in:inputs/in:param[@name='ForceWall']"/>
	<xsl:param name="culture" select="/in:inputs/in:result[@name='CurrentCulture']" />
	<xsl:variable name="cultureForLikeBox" select="csharp:FixCurrentCultureString($culture)" />
	<xsl:template match="/">
		<html>
			<head>
				<script id="facebook-all-js" src="http://connect.facebook.net/{$cultureForLikeBox}/all.js#xfbml=1"></script>
			</head>
			<body>
				<div id="fb-root">
					<fb:like-box
					href="{$url}"
					width="{$width}"
					show_faces="{$showfaces}"
					border_color="{$borderColor}"
					stream="{$stream}"
					header="{$header}"
					colorscheme="{$colorscheme}"
					force_wall="{$force_wall}">
					</fb:like-box>
				</div>
			</body>
		</html>
	</xsl:template>
	<msxsl:script implements-prefix="csharp" language="C#">public string FixCurrentCultureString(string culture) { return culture.Replace("-","_"); }</msxsl:script>
</xsl:stylesheet>