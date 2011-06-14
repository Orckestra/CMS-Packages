<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" 
xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
xmlns:in="http://www.composite.net/ns/transformation/input/1.0" 
xmlns:lang="http://www.composite.net/ns/localization/1.0" 
xmlns:f="http://www.composite.net/ns/function/1.0" 
xmlns="http://www.w3.org/1999/xhtml" 
xmlns:og="http://ogp.me/ns#" 
xmlns:fb="http://www.facebook.com/2008/fbml" 
xmlns:msxsl="urn:schemas-microsoft-com:xslt"
xmlns:csharp="http://c1.composite.net/sample/csharp"
exclude-result-prefixes="xsl in lang f msxsl csharp">
  <xsl:param name="title" select="/in:inputs/in:param[@name='Title']" />
  <xsl:param name="type" select="/in:inputs/in:param[@name='Type']" />
  <xsl:param name="image" select="/in:inputs/in:param[@name='Image']" />
  <xsl:param name="url" select="/in:inputs/in:param[@name='URL']" />
  <xsl:param name="sitename" select="/in:inputs/in:param[@name='SiteName']" />
  <xsl:param name="description" select="/in:inputs/in:param[@name='Description']" />
  <xsl:param name="admins" select="/in:inputs/in:param[@name='Admins']" />
  <xsl:param name="appids" select="/in:inputs/in:param[@name='ApplicationIDs']" />
  <xsl:variable name="currentPage" select="/in:inputs/in:result[@name='GetIPageXml']/IPage" />
  <xsl:template match="/">
    <html>
      <head>
        <xsl:choose>
          <xsl:when test="$title = ''">
            <meta property="og:title" content="{$currentPage/@Title}" />
          </xsl:when>
          <xsl:otherwise>
            <meta property="og:title" content="{$title}" />
          </xsl:otherwise>
        </xsl:choose>
        <xsl:if test="$type!= ''">
          <meta property="og:type" content="{$type}" />
        </xsl:if>
        <xsl:choose>
          <xsl:when test="$url = ''">
             <xsl:variable name="currUrl" select="csharp:GetCurrentUrl()" />
             <meta property="og:url" content="{$currUrl}" />
          </xsl:when>
          <xsl:otherwise>
              <meta property="og:url" content="{$url}" />
          </xsl:otherwise>
        </xsl:choose>
        <xsl:if test="$image!=''">
          <meta property="og:image" content="{$image}" />
        </xsl:if>
        <xsl:choose>
          <xsl:when test="$description != ''">
            <meta property="og:description" content="{$description}" />
          </xsl:when>
          <xsl:otherwise>
            <xsl:if test="$currentPage/@Description != ''">
              <meta property="og:description" content="{$currentPage/@Description}" />
            </xsl:if>
          </xsl:otherwise>
        </xsl:choose>
        <xsl:if test="$sitename != ''">
          <meta property="og:site_name" content="{$sitename}" />
        </xsl:if>
        <xsl:if test="$admins != ''">
          <meta property="fb:admins" content="{$admins}" />
        </xsl:if>
         <xsl:if test="$appids != ''">
          <meta property="fb:app_id" content="{$appids}" />
        </xsl:if>
      </head>
      <body></body>
    </html>
  </xsl:template>
  <msxsl:script implements-prefix="csharp" language="C#">
  <msxsl:assembly name="System.Web" /> 
   public string GetCurrentUrl()
    {
      return System.Web.HttpContext.Current.Request.Url.ToString();
    }
  </msxsl:script>
</xsl:stylesheet>