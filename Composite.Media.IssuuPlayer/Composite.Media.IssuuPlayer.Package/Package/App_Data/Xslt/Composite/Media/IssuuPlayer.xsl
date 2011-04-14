<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
	<xsl:param name="height" select="/in:inputs/in:param[@name='Height']" />
	<xsl:param name="width" select="/in:inputs/in:param[@name='Width']" />
	<xsl:template match="/">
		<html>
			<head></head>
			<body>
				<div>
					<xsl:apply-templates select="/in:inputs/in:result[@name='GetDocument']" />
				</div>
			</body>
		</html>
	</xsl:template>
	<xsl:template match="document">
		<xsl:if test="@publishing = 'true'">
			<script type="text/javascript">
				<xsl:comment>
					alert('The PDF File is being uploaded to the Issuu server and should be available within a few minutes. Please refresh this page to check the status.');
				</xsl:comment>
			</script>
		</xsl:if>
		<object style="width:{$width}px;height:{$height}px">
			<param name="movie" value="http://static.issuu.com/webembed/viewers/style1/v1/IssuuViewer.swf?mode=embed&amp;documentId={@documentId}&amp;layout=http%3A%2F%2Fskin.issuu.com%2Fv%2Flight%2Flayout.xml" />
			<param name="allowFullScreen" value="true" />
			<param name="menu" value="false" />
			<embed src="http://static.issuu.com/webembed/viewers/style1/v1/IssuuViewer.swf" type="application/x-shockwave-flash" allowFullScreen="true" menu="false" style="width:{$width}px;height:{$height}px" flashvars="mode=embed&amp;documentId={@documentId}&amp;layout=http%3A%2F%2Fskin.issuu.com%2Fv%2Flight%2Flayout.xml" />
		</object>
	</xsl:template>
</xsl:stylesheet>