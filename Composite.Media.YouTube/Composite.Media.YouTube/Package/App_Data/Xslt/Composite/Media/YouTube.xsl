<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="xsl in"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml">

	<xsl:param name="VideoId" select="/in:inputs/in:param[@name='VideoId']" />
	<xsl:param name="Height" select="/in:inputs/in:param[@name='Height']" />
	<xsl:param name="Width" select="/in:inputs/in:param[@name='Width']" />
	<xsl:param name="FullScreen" select="/in:inputs/in:param[@name='FullScreen']" />
	<xsl:variable name="FullScreenCode">
		<xsl:if test ="/in:inputs/in:param[@name='FullScreen'] = 'true'">?fs=1</xsl:if>
	</xsl:variable>

	<xsl:template match="/">
		<html>
			<head />
			<body>
				<iframe src="http://www.youtube.com/embed/{$VideoId}{$FullScreenCode}"
					 width="{$Width}" height="{$Height}"></iframe>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>