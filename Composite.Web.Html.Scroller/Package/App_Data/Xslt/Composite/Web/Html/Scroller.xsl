<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns:mp="#MarkupParserExtensions" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f mp">
	<xsl:variable name="title" select="/in:inputs/in:param[@name='Title']" />
	<xsl:variable name="content" select="/in:inputs/in:param[@name='Content']" />
	<xsl:variable name="id" select="/in:inputs/in:result[@name='NewGuid']" />
	<xsl:variable name="width" select="/in:inputs/in:param[@name='Width']" />
	<xsl:variable name="height" select="/in:inputs/in:param[@name='Height']" />
	<xsl:template match="/">
		<html>
			<head>
				<script id="jquery-js" src="//code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
				<script id="web-html-scroller-js" src="~/Frontend/Composite/Web/Html/Scroller/Scroller.js" type="text/javascript"></script>
				<script id="jquery-mousewheel" src="~/Frontend/Composite/Web/Html/Scroller/jquery.mousewheel.min.js" type="text/javascript"></script>
				<link id="web-html-scroller-css" rel="stylesheet" type="text/css" href="~/Frontend/Composite/Web/Html/Scroller/Styles.css" />
			</head>
			<body>
				<div id="cs{$id}" class="plnItemsContainer" style="width: {$width}px; height: {$height}px;">
					<xsl:if test="$title != ' '">
						<div class="plnTitle">
							<xsl:value-of select="$title" />
						</div>
					</xsl:if>
					<div class="plnScrollBox">
						<div class="plnContent">
							<xsl:copy-of select="mp:ParseXhtmlBodyFragment($content)" />
						</div>
					</div>
				</div>
				<br class="clear" />
				<script type="text/javascript">
					$(document).ready(function($) {
					var id = '<xsl:value-of select="$id" />';
					$('#cs' + id).autoScrolledContent();
					});
				</script>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>