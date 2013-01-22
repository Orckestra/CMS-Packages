<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
	<xsl:variable name="theme" select="/in:inputs/in:param[@name='Theme']" />
	<xsl:variable name="effect" select="/in:inputs/in:param[@name='Effects']" />
	<xsl:variable name="folder" select="/in:inputs/in:param[@name='ImagesFolder']" />
	<xsl:variable name="images" select="/in:inputs/in:result[@name='GetIImageFileXml']/IImageFile" />
	<xsl:variable name="width" select="/in:inputs/in:param[@name='Width']" />
	<xsl:variable name="height" select="/in:inputs/in:param[@name='Height']" />
	<xsl:variable name="navigation" select="/in:inputs/in:param[@name='Navigation']" />
	<xsl:variable name="repeat" select="/in:inputs/in:param[@name='Repeat']" />
	<xsl:variable name="captions" select="/in:inputs/in:param[@name='Captions']" />
	<xsl:template match="/">
		<html>
			<head>
				<link id="slider-theme-{$theme}" rel="stylesheet" href="~/Frontend/Composite/Media/NivoSlider/themes/{$theme}/{$theme}.css" type="text/css" media="screen" />
				<link id="slider-css" rel="stylesheet" href="~/Frontend/Composite/Media/NivoSlider/nivo-slider.css" type="text/css" media="screen" />
				<script id="jquery-js" src="//code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
				<script id="jquery-slider" src="~/Frontend/Composite/Media/NivoSlider/jquery.nivo.slider.pack.js" type="text/javascript"></script>
				<script id="slider-settings" type="text/javascript">
					$(window).load(function() {
					$('#slider').nivoSlider({
					effect: '<xsl:value-of select="$effect" />',
					slices: 15, /* For slice animations*/
					boxCols: 8, /* For box animations */
					boxRows: 4, /* For box animations */
					animSpeed: <xsl:value-of select="/in:inputs/in:param[@name='AnimSpeed']" />, /* Slide transition speed*/
					pauseTime: <xsl:value-of select="/in:inputs/in:param[@name='PauseTime']" />, /* How long each slide will show*/
					startSlide: 0, /* Set starting Slide (0 index)*/
					directionNav: <xsl:value-of select="/in:inputs/in:param[@name='DirectionNav']" />, /* Next and  Prev navigation*/
					controlNav: <xsl:value-of select="$navigation" />, /* 1,2,3... navigation*/
					controlNavThumbs: <xsl:value-of select="/in:inputs/in:param[@name='ControlNavThumbs']" />, /* Use thumbnails for Control Nav*/
					pauseOnHover: true, /* Stop animation while hovering*/
					manualAdvance: false, /* Force manual transitions*/
					prevText: 'Prev', /* Prev directionNav text*/
					nextText: 'Next', /* Next directionNav text*/
					beforeChange: function(){}, /* Triggers before a slide transition*/
					afterChange: function(){}, /* Triggers after a slide transition*/
					slideshowEnd: function(){}, /* Triggers after all slides have been shown*/
					lastSlide: function(){
					<xsl:if test="$repeat='false'"> primary.vars.stop = true;</xsl:if>
					}, /* Triggers when last slide is shown*/
					afterLoad: function(){} /* Triggers when slider has loaded*/
					});
					});
				</script>
				<xsl:variable name="widthStyleString">
					<xsl:if test="$width > 0">
						width:<xsl:value-of select="$width" />px !important;
					</xsl:if>
				</xsl:variable>
				<xsl:variable name="heightStyleString">
					<xsl:if test="$height > 0">
						height:<xsl:value-of select="$height" />px !important;
					</xsl:if>
				</xsl:variable>
				<style type="text/css">
					#slider img {
					margin:0;
					}
					#slider  {
					<xsl:value-of select="$widthStyleString" /> /* Change this to your images width */
					<xsl:value-of select="$heightStyleString" /> /* Change this to your images height */
					}
					<xsl:if test="$height > 0">
					.theme-dark #slider {overflow: hidden;}
					.theme-light #slider {overflow: hidden;}
					</xsl:if>
					.nivo-controlNav { <xsl:value-of select="$widthStyleString" /> }
					.slider-wrapper {<xsl:value-of select="$widthStyleString" /> }
				</style>

			</head>
			<body>
				<div class="slider-wrapper theme-{$theme}">
					<xsl:choose>
						<xsl:when test="$folder !=''">
							<div id="slider" class="nivoSlider">
								<xsl:for-each select="$images">
									<img src="~/media({@Id})" data-thumb="~/media({@Id})?h=50&amp;w=70" alt="{@Title}">
										<xsl:if test="$captions = 'true'">
											<xsl:attribute name="title">
												<xsl:value-of select="@Title" />
											</xsl:attribute>
										</xsl:if>
									</img>
								</xsl:for-each>
							</div>
						</xsl:when>
						<xsl:otherwise>
							<!--Demo example or put here your markup-->
							<div class="ribbon"></div>
							<div id="slider" class="nivoSlider">
								<img src="~/Frontend/Composite/Media/NivoSlider/demo/images/toystory.jpg" alt="" />
								<a href="http://dev7studios.com">
									<img src="~/Frontend/Composite/Media/NivoSlider/demo/images/up.jpg" alt="">
									<xsl:if test="$captions = 'true'">
											<xsl:attribute name="title">This is an example of a caption</xsl:attribute>
									</xsl:if>
									</img>
								</a>
								<img src="~/Frontend/Composite/Media/NivoSlider/demo/images/walle.jpg" alt="" />
								<img src="~/Frontend/Composite/Media/NivoSlider/demo/images/nemo.jpg" alt="">
								<xsl:if test="$captions = 'true'">
									<xsl:attribute name="title">#htmlcaption</xsl:attribute>
								</xsl:if>
								</img>
							</div>
							<div id="htmlcaption" class="nivo-html-caption">
								<strong>This</strong> is an example of a <em>HTML</em> caption with <a href="#">a link</a>.
							</div>
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>