<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in">
	<xsl:param name="useAnimation" select="/in:inputs/in:param[@name='UseAnimatedRotation']" />
	<xsl:param name="images" select="/in:inputs/in:result[@name='GetIImageFileXml']/IImageFile" />
	<xsl:template match="/">
		<html>
			<head></head>
			<body>
				<div id="ImageRotator">
					<xsl:apply-templates select="$images" />
				</div>
				<xsl:if test="$useAnimation = 'true'">
					<style type="text/css">
						#ImageRotator {
						position:relative;
						height: 350px;
						/*overflow: hidden;*/
						}
						#ImageRotator img {
						position:absolute;
						top:0;
						left:0;
						z-index:8;
						}
						#ImageRotator img.active {
						z-index:10;
						}
						#ImageRotator img.last-active {
						z-index:9;
						}
					</style>
					<script id="jquery-js" src="//code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
					<script type="text/javascript">
						function slideSwitch() {
						var $active = $('#ImageRotator IMG.active');

						if ( $active.length == 0 ) $active = $('#ImageRotator IMG:last');

						var $next =  $active.next().length ? $active.next()
						: $('#ImageRotator IMG:first');

						$active.addClass('last-active');

						$next.css({opacity: 0.0})
						.addClass('active')
						.animate({opacity: 1.0}, 1000, function() {
						$active.removeClass('active last-active');
						});
						}

						$(function() {
						setInterval( "slideSwitch()", 5000 );
						});
					</script>
				</xsl:if>
			</body>
		</html>
	</xsl:template>
	<xsl:template match="IImageFile">
		<img alt="image" title="{@Title}" src="~/Renderers/ShowMedia.ashx?id={@Id}">
			<xsl:attribute name="class">
				<xsl:if test="position() = 1">active</xsl:if>
			</xsl:attribute>
		</img>
	</xsl:template>
</xsl:stylesheet>