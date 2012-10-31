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
        <xsl:variable name="brush">
          <xsl:choose>
            <xsl:when test="$codeType='ActionScript3'">as3</xsl:when>
            <xsl:when test="$codeType='Bash/shell'">bash</xsl:when>
            <xsl:when test="$codeType='ColdFusion'">coldfusion</xsl:when>
            <xsl:when test="$codeType='C#'">csharp</xsl:when>
            <xsl:when test="$codeType='C++'">cpp</xsl:when>
            <xsl:when test="$codeType='CSS'">css</xsl:when>
            <xsl:when test="$codeType='Delphi'">delphi</xsl:when>
            <xsl:when test="$codeType='Diff'">diff</xsl:when>
            <xsl:when test="$codeType='Erlang'">erlang</xsl:when>
            <xsl:when test="$codeType='Groovy'">groovy</xsl:when>
            <xsl:when test="$codeType='JavaScript'">js</xsl:when>
            <xsl:when test="$codeType='Java'">java</xsl:when>
            <xsl:when test="$codeType='JavaFX'">javafx</xsl:when>
            <xsl:when test="$codeType='Perl'">perl</xsl:when>
            <xsl:when test="$codeType='PHP'">php</xsl:when>
            <xsl:when test="$codeType='Plain Text'">text</xsl:when>
            <xsl:when test="$codeType='PowerShell'">ps</xsl:when>
            <xsl:when test="$codeType='Python'">python</xsl:when>
            <xsl:when test="$codeType='Ruby'">ruby</xsl:when>
            <xsl:when test="$codeType='Scala'">scala</xsl:when>
            <xsl:when test="$codeType='SQL'">sql</xsl:when>
            <xsl:when test="$codeType='Visual Basic'">vb</xsl:when>
            <xsl:when test="$codeType='XML'">xml</xsl:when>
          </xsl:choose>
        </xsl:variable>
        <pre class="brush: {$brush}">
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
      <xsl:when test="$codeType='ActionScript3'">
        <script id="shBrushAS3" type="text/javascript" src="{$path}scripts/shBrushAS3.js"></script>
      </xsl:when>
      <xsl:when test="$codeType='Bash/shell'">
        <script id="shBrushBash" type="text/javascript" src="{$path}scripts/shBrushBash.js"></script>
      </xsl:when>
      <xsl:when test="$codeType='C#'">
        <script id="shBrushCSharp" type="text/javascript" src="{$path}scripts/shBrushCSharp.js"></script>
      </xsl:when>
      <xsl:when test="$codeType='C++'">
        <script id="shBrushCpp" type="text/javascript" src="{$path}scripts/shBrushCpp.js"></script>
      </xsl:when>
      <xsl:when test="$codeType='CSS'">
        <script id="shBrushCss" type="text/javascript" src="{$path}scripts/shBrushCss.js"></script>
      </xsl:when>
      <xsl:when test="$codeType='JavaScript'">
        <script id="shBrushJScript" type="text/javascript" src="{$path}scripts/shBrushJScript.js"></script>
      </xsl:when>
      <xsl:when test="$codeType='PHP'">
        <script id="shBrushPhp" type="text/javascript" src="{$path}scripts/shBrushPhp.js"></script>
      </xsl:when>
      <xsl:when test="$codeType='Plain Text'">
        <script id="shBrushPlain" type="text/javascript" src="{$path}scripts/shBrushPlain.js"></script>
      </xsl:when>
      <xsl:when test="$codeType='SQL' or $codeType='vbnet'">
        <script id="shBrushSql" type="text/javascript" src="{$path}scripts/shBrushSql.js"></script>
      </xsl:when>
      <xsl:when test="$codeType='Visual Basic'">
        <script id="shBrushVb" type="text/javascript" src="{$path}scripts/shBrushVb.js"></script>
      </xsl:when>
      <xsl:when test="$codeType='XML'">
        <script id="shBrushXml" type="text/javascript" src="{$path}scripts/shBrushXml.js"></script>
      </xsl:when>
      <xsl:otherwise>
        <script id="shBrush{$codeType}" type="text/javascript" src="{$path}scripts/shBrush{$codeType}.js"></script>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>