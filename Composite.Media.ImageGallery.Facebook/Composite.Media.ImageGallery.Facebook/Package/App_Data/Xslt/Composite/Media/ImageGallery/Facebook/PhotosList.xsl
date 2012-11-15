<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
	<xsl:variable name="albumId" select="/in:inputs/in:param[@name='AlbumID']" />
	<xsl:template match="/">
		<html>
			<head>
				<style type="text/css">
					.clear {clear: both;}
					.error {color: red;}
					.photo_item {float: left; margin: 8px;}
					.squared_div { width: 125px; height:100px; background: white; background-repeat: no-repeat; background-position: center 25%;}
				</style>
				<script id="jquery-js" src="//code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
				<script id="slimbox2-js" src="~/Frontend/Composite/Media/ImageGallery/Slimbox-2.04/js/slimbox2.js" type="text/javascript"></script>
				<link id="slimbox-2.04" media="screen" type="text/css" href="~/Frontend/Composite/Media/ImageGallery/Slimbox-2.04/css/slimbox2.css" rel="stylesheet" />
			</head>
			<body>
				<div>
					<a>
						<xsl:attribute name="name">
							<xsl:value-of select="concat('a',substring($albumId,0,3))" />
						</xsl:attribute>
					</a>
					<xsl:apply-templates select="/in:inputs/in:result[@name='GetPhotos']" />
					<br class="clear" />
				</div>
			</body>
		</html>
	</xsl:template>
	<xsl:template match="Photo">
		<div class="photo_item">
			<a rel="lightbox-{$albumId}" href="{@Source}" title="{@Name}">
				<div class="squared_div" style="background-image: url({@Picture});">&#160;</div>
			</a>
			<span style="display:none;">
				<a href="{@Link}" title="{@Name}">
					<xsl:value-of select="@Name" />
				</a>
			</span>
		</div>
	</xsl:template>
	<xsl:template match="Error">
		<span class="error">
			<xsl:value-of select="@Message" />
		</span>
	</xsl:template>
</xsl:stylesheet>