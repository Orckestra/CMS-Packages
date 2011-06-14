﻿<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
xmlns:lang="http://www.composite.net/ns/localization/1.0"
xmlns:f="http://www.composite.net/ns/function/1.0"
xmlns:fb="http://www.facebook.com/plugins/"
xmlns="http://www.w3.org/1999/xhtml"
xmlns:msxsl="urn:schemas-microsoft-com:xslt"
xmlns:csharp="http://c1.composite.net/sample/csharp"
exclude-result-prefixes="xsl in lang f fb msxsl csharp">
  <xsl:param name="urlToLike" select="/in:inputs/in:param[@name='URL']" />
  <xsl:param name="sendButton" select="/in:inputs/in:param[@name='SendButton']" />
  <xsl:param name="layout" select="/in:inputs/in:param[@name='LayoutStyle']" />
  <xsl:param name="width" select="/in:inputs/in:param[@name='Width']" />
  <xsl:param name="showFaces" select="/in:inputs/in:param[@name='ShowFaces']" />
  <xsl:param name="verbToDisplay" select="/in:inputs/in:param[@name='VerbToDisplay']" />
  <xsl:param name="colorScheme" select="/in:inputs/in:param[@name='ColorScheme']" />
  <xsl:param name="font" select="/in:inputs/in:param[@name='Font']" />
  <xsl:param name="culture" select="/in:inputs/in:result[@name='CurrentCulture']" />
  <xsl:variable name="cultureForLikeButton" select="csharp:FixCurrentCultureString($culture)" />
  <xsl:template match="/">
    <html>
      <head>
        <!-- markup placed here will be shown in the head section of the rendered page --></head>
      <body>
        <div>
          <script src="http://connect.facebook.net/{$cultureForLikeButton}/all.js#xfbml=1"></script>
          <fb:like
          href="{$urlToLike}"
          send="{$sendButton}"
          layout="{$layout}"
          width="{$width}"
          show_faces="{$showFaces}"
          colorscheme="{$colorScheme}"
          action="{$verbToDisplay}"
          font="{$font}">
          </fb:like>
        </div>
      </body>
    </html>
  </xsl:template>
   <msxsl:script implements-prefix="csharp" language="C#">
    public string FixCurrentCultureString(string culture)
    {
      return culture.Replace("-","_");
    }
  </msxsl:script>
</xsl:stylesheet>