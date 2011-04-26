<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="xsl in lang f">

	<xsl:variable name="isRemote" select="/in:inputs/in:param[@name='UseRemoteVersion']" />
	<xsl:variable name="codeType" select="/in:inputs/in:param[@name='CodeType']" />
	<xsl:variable name="cssfile" select="/in:inputs/in:param[@name='Themes']" />
	<xsl:template match="/">
		<html>
			<head>
				<xsl:choose>
					<xsl:when test="$isRemote = 'False'">
						<xsl:call-template name="Includes">
							<xsl:with-param name="path">~/Frontend/Composite/Web/Html/SyntaxHighlighter/</xsl:with-param>
						</xsl:call-template>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="Includes">
							<xsl:with-param name="path">http://alexgorbatchev.com/pub/sh/current/</xsl:with-param>
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
				<script id="initializeSyntaxHighlighter" type="text/javascript">
					SyntaxHighlighter.defaults['toolbar'] = false;
					SyntaxHighlighter.defaults['gutter'] = true;
					SyntaxHighlighter.all();
				</script>
			</head>
			<body>
				<pre>
					<xsl:attribute name="class">
						brush: <xsl:value-of select="/in:inputs/in:param[@name='CodeType']" />;
					</xsl:attribute>
					<xsl:value-of select="/in:inputs/in:param[@name='SourceCode']" />
				</pre>
			</body>
		</html>
	</xsl:template>
	<xsl:template name="Includes">
		<xsl:param name="path" />
		<link id="shCoreDefault" type="text/css" rel="stylesheet" href="{$path}styles/shCoreDefault.css" />
		<link id="shTheme" type="text/css" rel="stylesheet" href="{$path}styles/{$cssfile}" />
		<script id="shCore" type="text/javascript" src="{$path}scripts/shCore.js"></script>
		<xsl:choose>
			<xsl:when test="$codeType='c#' or $codeType='csharp' or $codeType='c-sharp'">
				<script id="shBrushCSharp" type="text/javascript" src="{$path}scripts/shBrushCSharp.js"></script>
			</xsl:when>
			<xsl:when test="$codeType='css'">
				<script id="shBrushCss" type="text/javascript" src="{$path}scripts/shBrushCss.js"></script>
			</xsl:when>
			<xsl:when test="$codeType='cpp' or $codeType='c'">
				<script id="shBrushCpp" type="text/javascript" src="{$path}scripts/shBrushCpp.js"></script>
			</xsl:when>
			<xsl:when test="$codeType='js' or $codeType='javascript' or $codeType='jscript'">
				<script id="shBrushJScript" type="text/javascript" src="{$path}scripts/shBrushJScript.js"></script>
			</xsl:when>
			<xsl:when test="$codeType='java'">
				<script id="shBrushJava" type="text/javascript" src="{$path}scripts/shBrushJava.js"></script>
			</xsl:when>
			<xsl:when test="$codeType='php'">
				<script id="shBrushPhp" type="text/javascript" src="{$path}scripts/shBrushPhp.js"></script>
			</xsl:when>
			<xsl:when test="$codeType='sql'">
				<script id="shBrushSql" type="text/javascript" src="{$path}scripts/shBrushSql.js"></script>
			</xsl:when>
			<xsl:when test="$codeType='xml' or $codeType='html' or $codeType='xhtml' or $codeType='xslt'">
				<script id="shBrushXml" type="text/javascript" src="{$path}scripts/shBrushXml.js"></script>
			</xsl:when>
			<xsl:when test="$codeType='vb' or $codeType='vbnet'">
				<script id="shBrushVb" type="text/javascript" src="{$path}scripts/shBrushVb.js"></script>
			</xsl:when>
			<xsl:when test="$codeType='text' or $codeType='plain'">
				<script id="shBrushPlain" type="text/javascript" src="{$path}scripts/shBrushPlain.js"></script>
			</xsl:when>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>