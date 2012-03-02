<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
  xmlns:lang="http://www.composite.net/ns/localization/1.0"
  xmlns:f="http://www.composite.net/ns/function/1.0"
  xmlns="http://www.w3.org/1999/xhtml"
  xmlns:parser="#MarkupParserExtensions"
  exclude-result-prefixes="xsl in lang f parser">
	<xsl:variable name="teaser" select="/in:inputs/in:param[@name='LightboxTeaser']/*" />
	<xsl:variable name="content" select="/in:inputs/in:param[@name='LightboxContent']/*" />
	<xsl:variable name="width" select="/in:inputs/in:param[@name='ContentWidth']" />
	<xsl:variable name="height" select="/in:inputs/in:param[@name='ContentHeight']" />
	<xsl:variable name="id" select="/in:inputs/in:result[@name='NewGuid']" />
	<xsl:template match="/">
		<html>
			<head>
				<script id="jquery-1-4-2" src="http://ajax.microsoft.com/ajax/jquery/jquery-1.4.2.min.js" type="text/javascript"></script>
				<script id="html-lightbox-js" src="~/Frontend/Composite/Web/Html/Lightbox/js/jquery.html-lightbox.js" type="text/javascript"></script>
				<link id="html-lightbox-css" media="screen" type="text/css" href="~/Frontend/Composite/Web/Html/Lightbox/css/html-lightbox.css" rel="stylesheet" />
				<script type="text/javascript">
					$(document).ready(function(){
					var id = 'id' + '<xsl:value-of select="$id" />';
					$('#'+ id).click( function() {
					showHtmlLightbox(this, {
					width: <xsl:value-of select="$width" />,
					height: <xsl:value-of select="$height" />
					});
					}
					);
					});
				</script>
			</head>

			<body>
				<div id="id{$id}" class="teaser-lightbox" title="More...">
					<xsl:copy-of select="$teaser" />
					<br class="clear"/>
				</div>
				<div class="content-lightbox">
					<xsl:copy-of select="$content" />
				</div>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>
