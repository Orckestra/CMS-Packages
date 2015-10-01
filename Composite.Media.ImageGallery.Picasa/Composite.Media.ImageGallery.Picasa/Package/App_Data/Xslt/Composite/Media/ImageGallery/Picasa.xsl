<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" 
				xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
				xmlns:in="http://www.composite.net/ns/transformation/input/1.0" 
				xmlns:lang="http://www.composite.net/ns/localization/1.0" 
				xmlns:f="http://www.composite.net/ns/function/1.0" 
				xmlns="http://www.w3.org/1999/xhtml" 
				xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
				xmlns:csharp="http://c1.composite.net/sample/csharp" 
				exclude-result-prefixes="xsl in lang f csharp msxsl">
	<xsl:param name="albumrss" select="/in:inputs/in:param[@name='AlbumRss']" />
	<xsl:param name="width" select="/in:inputs/in:param[@name='Width']" />
	<xsl:param name="height" select="/in:inputs/in:param[@name='Height']" />
	<xsl:param name="showcaptions" select="/in:inputs/in:param[@name='ShowCaptions']" />
	<xsl:param name="autoplay" select="/in:inputs/in:param[@name='AutoPlay']" />
	<xsl:param name="bg" select="/in:inputs/in:param[@name='BackgroundColor']" />
	<xsl:variable name="albumrssEncoded" select="csharp:GetPicasaAlbumUrl($albumrss)" />

	<xsl:template match="/">
		<html>
			<head>
				<!-- markup placed here will be shown in the head section of the rendered page -->
			</head>
			<body>
				<embed
				type="application/x-shockwave-flash"
				src="https://photos.gstatic.com/media/slideshow.swf"
				width="{$width}" height="{$height}"
				pluginspage="http://www.macromedia.com/go/getflashplayer">
					<xsl:attribute name="flashvars">
						host=picasaweb.google.com&amp;hl=en_US&amp;feat=flashalbum&amp;RGB=0x<xsl:value-of select="$bg" />&amp;feed=<xsl:value-of select="$albumrssEncoded" />
						<xsl:if test="$showcaptions='true'">&amp;captions=1</xsl:if>
						<xsl:if test="$autoplay='false'">&amp;noautoplay=1</xsl:if>
					</xsl:attribute>
				</embed>
			</body>
		</html>
	</xsl:template>
	<msxsl:script implements-prefix="csharp" language="C#">
		<msxsl:assembly name="System.Web" />
		<msxsl:using namespace="System.Web" />
		public String GetPicasaAlbumUrl(string source) {
		return HttpUtility.UrlEncode(source.Replace("/base/", "/api/"));
		}
	</msxsl:script>
</xsl:stylesheet>