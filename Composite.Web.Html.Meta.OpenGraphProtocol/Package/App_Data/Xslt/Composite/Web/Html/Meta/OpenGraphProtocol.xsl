<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:mediaHelper="urn:my-scripts" xmlns:og="http://ogp.me/ns#" exclude-result-prefixes="xsl in lang f">
  <xsl:variable name="currentPage" select="/in:inputs/in:result[@name='SitemapXml']//Page[@iscurrent='true']" />
  <xsl:variable name="graph" select="/in:inputs/in:result[@name='GetOpenGraphProtocolXml']/OpenGraphProtocol[@PageId=$currentPage/@Id]" />

  <xsl:param name="sitename" select="/in:inputs/in:param[@name='SiteName']" />
  <xsl:param name="admins" select="/in:inputs/in:param[@name='FacebookAdmins']" />
  <xsl:param name="appids" select="/in:inputs/in:param[@name='FacebookApplicationIDs']" />

  <xsl:variable name="host" select="/in:inputs/in:result[@name='Host']" />
  <xsl:variable name="appPath" select="/in:inputs/in:result[@name='ApplicationPath']" />
  <msxsl:script language="C#" implements-prefix="mediaHelper">
    <msxsl:assembly name="Composite" />
    <msxsl:assembly name="System.Collections" />
    <msxsl:using namespace="Composite" />
    <![CDATA[public string GetUrl(string mediaId){
	  System.Collections.Specialized.NameValueCollection strCol = new System.Collections.Specialized.NameValueCollection();
	  strCol.Add("id", mediaId);	  
	  Composite.Data.Types.IMediaFile mediaFile = Composite.Core.WebClient.MediaUrlHelper.GetFileFromQueryString(strCol);
	  return Composite.Core.WebClient.MediaUrlHelper.GetUrl(mediaFile, false);}]]>
  </msxsl:script>

  <xsl:template match="/">
    <html>
      <head>
        <xsl:choose>
          <xsl:when test="string-length($graph/@Title) > 0">
            <meta property="og:title" content="{$graph/@Title}" />
          </xsl:when>
          <xsl:otherwise>
            <meta property="og:title" content="{$currentPage/@Title}" />
          </xsl:otherwise>
        </xsl:choose>
        <xsl:choose>
          <xsl:when test="string-length($graph/@Type) > 0">
            <meta property="og:type" content="{$graph/@Type}" />
          </xsl:when>
        </xsl:choose>
        <meta property="og:url" content="http://{$host}{$appPath}{$currentPage/@URL}" />
        <xsl:call-template name="Image">
          <xsl:with-param name="id" select="$currentPage/@Id" />
        </xsl:call-template>
        <xsl:choose>
          <xsl:when test="string-length($graph/@Description) > 0">
            <meta property="og:description" content="{$graph/@Description}" />
          </xsl:when>
          <xsl:otherwise>
            <xsl:if test="string-length($currentPage/@Description) > 0">
              <meta property="og:description" content="{$currentPage/@Description}" />
            </xsl:if>
          </xsl:otherwise>
        </xsl:choose>
        <xsl:if test="string-length($sitename) > 0">
          <meta property="og:site_name" content="{$sitename}" />
        </xsl:if>
        <xsl:if test="string-length($admins) > 0">
          <meta property="fb:admins" content="{$admins}" xmlns:fb="http://www.facebook.com/2008/fbml" />
        </xsl:if>
        <xsl:if test="string-length($appids) > 0">
          <meta property="fb:app_id" content="{$appids}" xmlns:fb="http://www.facebook.com/2008/fbml" />
        </xsl:if>
      </head>
      <body></body>
    </html>
  </xsl:template>

  <xsl:template match="*" name="Image">
    <xsl:param name="id" />
    <xsl:param name="graph1" select="/in:inputs/in:result[@name='GetOpenGraphProtocolXml']/OpenGraphProtocol[@PageId=$id]" />
    <xsl:param name="urlImage" select="mediaHelper:GetUrl($graph1/@Image)" />
    <xsl:choose>
      <xsl:when test="string-length($graph1/@Image) > 0">
        <meta property="og:image" content="http://{$host}{$appPath}/{$urlImage}" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:variable name="parentId" select="/in:inputs/in:result[@name='SitemapXml']//Page[Page[@Id = $id]]/@Id" />
        <xsl:if test="string-length($parentId) > 0">
          <xsl:call-template name="Image">
            <xsl:with-param name="id" select="$parentId" />
          </xsl:call-template>
        </xsl:if>
      </xsl:otherwise>
    </xsl:choose>

  </xsl:template>
</xsl:stylesheet>