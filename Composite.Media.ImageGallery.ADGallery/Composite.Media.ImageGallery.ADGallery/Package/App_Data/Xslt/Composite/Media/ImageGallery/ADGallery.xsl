<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
	<xsl:param name="randomId" select="/in:inputs/in:result[@name='NewGuid']" />
	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Media/ImageGallery/ADGallery/jquery.ad-gallery.css" />
				<script id="jquery-1-4-2" src="http://ajax.microsoft.com/ajax/jquery/jquery-1.4.2.min.js" type="text/javascript"></script>
				<script type="text/javascript" src="~/Frontend/Composite/Media/ImageGallery/ADGallery/jquery.ad-gallery.pack.js"></script>
				<script type="text/javascript">
					$(function() {
						var id = '#id' + '<xsl:value-of select="$randomId" />';
						$(id).adGallery();
					});
				</script>
			</head>
			<body>
				<div id="id{$randomId}" class="ad-gallery">
					<div class="ad-image-wrapper"></div>
					<div class="ad-controls"></div>
					<div class="ad-nav">
						<div class="ad-thumbs">
							<ul class="ad-thumb-list">
								<xsl:for-each select="/in:inputs/in:result[@name='GetIImageFileXml']/IImageFile">
									<li>
										<a href="~/Renderers/ShowMedia.ashx?id={@Id}&amp;mw=600&amp;mh=400">
											<img src="~/Renderers/ShowMedia.ashx?id={@Id}&amp;mh=60" title="{@Title}" alt="{@Description}" class="image{position()-1}" />
										</a>
									</li>
								</xsl:for-each>
							</ul>
						</div>
					</div>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>