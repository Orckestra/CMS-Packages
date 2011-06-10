<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
        xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
        xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
        xmlns:lang="http://www.composite.net/ns/localization/1.0"
        xmlns:f="http://www.composite.net/ns/function/1.0"
        xmlns="http://www.w3.org/1999/xhtml"
        exclude-result-prefixes="xsl in lang f">
	<xsl:variable name="isSquareSize" select="/in:inputs/in:param[@name='AutoSquareThumbnails']" />
	<xsl:variable name="thWidth" select="/in:inputs/in:param[@name='ThumbnailMaxWidth']" />
	<xsl:variable name="thHeight" select="/in:inputs/in:param[@name='ThumbnailMaxHeight']" />
	<xsl:variable name="imgWidth" select="/in:inputs/in:param[@name='ImageMaxWidth']" />
	<xsl:variable name="imgHeight" select="/in:inputs/in:param[@name='ImageMaxHeight']" />
	<xsl:variable name="downloadOriginal" select="/in:inputs/in:param[@name='DownloadLinkText']" />
	<xsl:variable name="appRoot" select="/in:inputs/in:result[@name='ApplicationPath']" />
	<xsl:variable name="singleImage" select="/in:inputs/in:param[@name='MediaImage']" />
	<xsl:template match="/">
		<html>
			<head>
				<script id="jquery-1-4-2" src="http://ajax.microsoft.com/ajax/jquery/jquery-1.4.2.min.js" type="text/javascript"></script>
				<script id="slimbox2-js" src="/Frontend/Composite/Media/ImageGallery/Slimbox-2.04/js/slimbox2.js" type="text/javascript"></script>
				<link id="slimbox-2.04" media="screen" type="text/css" href="/Frontend/Composite/Media/ImageGallery/Slimbox-2.04/css/slimbox2.css" rel="stylesheet" />
			</head>
			<body>

				<xsl:for-each select="/in:inputs/in:result[@name='GetIImageFileXml']/IImageFile">
					<a title="{@Description}" href="/Renderers/ShowMedia.ashx?id={@Id}&amp;mw={$imgWidth}&amp;mh={$imgHeight}">
						<xsl:attribute name="rel">
							<xsl:choose>
								<xsl:when test="$singleImage != ''">lightbox</xsl:when>
								<xsl:otherwise>lightbox-<xsl:value-of select="@FolderPath" />
								</xsl:otherwise>
							</xsl:choose>
						</xsl:attribute>
						<xsl:choose>
							<xsl:when test="$isSquareSize = 'true'">
								<xsl:variable name="bgImageUrl" select="concat('url(&quot;', $appRoot, '/Renderers/ShowMedia.ashx?id=', @Id, '&amp;h=',$thWidth, '&quot;);')" />
								<div class="squared_div" style="width: {$thWidth}px; height: {$thWidth}px; background-image: {$bgImageUrl};">
									&#160;
								</div>
							</xsl:when>
							<xsl:otherwise>
								<img src="/Renderers/ShowMedia.ashx?id={@Id}&amp;mw={$thWidth}&amp;mh={$thHeight}" title="{@Description}" alt="{@Title}" />
							</xsl:otherwise>
						</xsl:choose>
					</a>
					<xsl:choose>
						<xsl:when test="$downloadOriginal != ''">
							<span style="display:none;">
								<xsl:value-of select="@Description" />
								<br />
								<a target="_blank" href="/Renderers/ShowMedia.ashx?id={@Id}&amp;download=true">
									<xsl:value-of select="$downloadOriginal" />
								</a>
							</span>
						</xsl:when>
						<xsl:otherwise>
							<span style="display:none;">
								<xsl:value-of select="@Description" />
							</span>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:for-each>
				<xsl:if test="/in:inputs/in:param[@name='MediaFolder']!=''">
					<br class="clear_slimbox"/>
				</xsl:if>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>