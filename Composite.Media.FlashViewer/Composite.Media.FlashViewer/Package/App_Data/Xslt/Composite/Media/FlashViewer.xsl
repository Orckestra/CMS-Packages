<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="xsl in msxsl" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns="http://www.w3.org/1999/xhtml" xmlns:msxsl="urn:schemas-microsoft-com:xslt">
	<xsl:param name="flashFile" select="/in:inputs/in:param[@name='FlashFile']" />
	<xsl:param name="height" select="/in:inputs/in:param[@name='Height']" />
	<xsl:param name="width" select="/in:inputs/in:param[@name='Width']" />
	<xsl:param name="loop" select="/in:inputs/in:param[@name='Repeat']" />
	<xsl:param name="flashVersion" select="/in:inputs/in:param[@name='FlashVersion']" />
	<xsl:param name="newGuid" select="/in:inputs/in:result[@name='NewGuid']" />
	<xsl:param name="rootPath" select="/in:inputs/in:result[@name='ApplicationPath']" />
	<xsl:variable name="flashId" select="concat('swf',translate($newGuid,'-',''))" />
	<xsl:template match="/">
		<xsl:param name="flashFilePath" select="concat($rootPath,'/Renderers/ShowMedia.ashx?i=',$flashFile, '&amp;download=false')" />
		<html>
			<head>
				<script id="swfobject" type="text/javascript" src="~/Frontend/Composite/Media/FlashViewer/JavaScript/swfobject.js"></script>
				<script type="text/javascript">
					<xsl:text>swfobject.registerObject("</xsl:text>
					<xsl:value-of select="$flashId" />
					<xsl:text>", "</xsl:text>
					<xsl:value-of select="$flashVersion" />
					<xsl:text>", "</xsl:text>
					<xsl:value-of select="concat($rootPath, '/Frontend/Composite/Media/FlashViewer/Flash/expressInstall.swf')" />
					<xsl:text>");</xsl:text>
				</script>
			</head>
			<body>
				<object id="{$flashId}" classid="clsid:D27CDB6E-AE6D-11cf-96B8-444553540000" height="{$height}" width="{$width}">
					<param name="movie" value="{$flashFilePath}" />
					<param name="loop" value="{$loop}" />
					<xsl:comment>[if !IE]&gt;</xsl:comment>
					<object type="application/x-shockwave-flash" data="{$flashFilePath}" height="{$height}" width="{$width}" loop="{$loop}">
						<xsl:comment>[if !IE]&gt;</xsl:comment>
						<a href="http://www.adobe.com/go/getflashplayer">
							<img src="http://www.adobe.com/images/shared/download_buttons/get_flash_player.gif" alt="Get Adobe Flash player" />
						</a>
						<xsl:comment>&lt;![endif]</xsl:comment>
					</object>
					<xsl:comment>&lt;![endif]</xsl:comment>
				</object>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>