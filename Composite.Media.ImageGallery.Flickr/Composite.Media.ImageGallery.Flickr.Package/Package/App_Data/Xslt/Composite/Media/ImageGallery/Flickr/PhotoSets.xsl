<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
  <xsl:variable name="setid" select="/in:inputs/in:result[@name='PathInfo']" />
  <xsl:param name="showcomments" select="/in:inputs/in:param[@name='ShowComments']" />
  <xsl:template match="/">
    <html>
      <head>
      </head>
      <body>
        <xsl:choose>
          <xsl:when test="$setid != ''">
            <f:function name="Composite.Media.ImageGallery.Flickr.PhotoSets.Photos">
              <f:param name="ApiKey" value="{/in:inputs/in:param[@name='ApiKey']}" />
              <f:param name="SetId" value="{$setid}" />
            </f:function>
            <xsl:if test="$showcomments = 'true'">
              <f:function name="Composite.Media.ImageGallery.Flickr.PhotoSets.CommentsList">
                <f:param name="ApiKey" value="{/in:inputs/in:param[@name='ApiKey']}" />
                <f:param name="SetId" value="{$setid}" />
              </f:function>
            </xsl:if>
          </xsl:when>
          <xsl:otherwise>
            <f:function name="Composite.Media.ImageGallery.Flickr.PhotoSets.List">
              <f:param name="ApiKey" value="{/in:inputs/in:param[@name='ApiKey']}" />
              <f:param name="UserID" value="{/in:inputs/in:param[@name='UserID']}" />
            </f:function>
          </xsl:otherwise>
        </xsl:choose>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>