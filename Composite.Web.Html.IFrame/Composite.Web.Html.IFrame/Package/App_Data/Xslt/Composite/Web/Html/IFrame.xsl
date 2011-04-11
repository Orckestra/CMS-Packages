<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="xsl in lang" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml">
	<xsl:variable name="input" select="/in:inputs/in:param" />
	<xsl:variable name="height" select="$input[@name='Height']" />
	<xsl:variable name="src" select="$input[@name='Src']" />
	<xsl:variable name="id" select="$input[@name='Id']" />
	<xsl:variable name="class" select="$input[@name='Class']" />
	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Web/Html/IFrame/Styles.css " />
			</head>
			<body>
				<iframe src="{$src}" frameborder="0">
					<xsl:if test="string-length($height) &gt; 0">
						<xsl:attribute name="height">
							<xsl:value-of select="$height" />
						</xsl:attribute>
					</xsl:if>
					<xsl:if test="string-length($id) &gt; 0">
						<xsl:attribute name="id">
							<xsl:value-of select="$id" />
						</xsl:attribute>
					</xsl:if>
					<xsl:if test="string-length($class) &gt; 0">
						<xsl:attribute name="class">
							<xsl:value-of select="$class" />
						</xsl:attribute>
					</xsl:if>
					<noframes>
						<p>
							<strong>Unable to display embedded web page!</strong>
							Your browser does not support html frames. 
						</p>
						<p>
							Click here to load the embedded web page: 
							<a href="{$input[@name='src']}" target="_blank">
								<xsl:value-of select="$input[@name='src']" />
							</a>
						</p>
					</noframes>
				</iframe>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>