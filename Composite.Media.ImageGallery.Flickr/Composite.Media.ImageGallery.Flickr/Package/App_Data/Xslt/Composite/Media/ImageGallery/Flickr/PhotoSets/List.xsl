<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
  <xsl:variable name="response" select="/in:inputs/in:result[@name='LoadUrl']/rsp" />
  <xsl:variable name="setid" select="/in:inputs/in:result[@name='PathInfo']" />
  <xsl:variable name="pageId" select="/in:inputs/in:result[@name='GetPageId']" />
  <xsl:template match="/">
    <html>
      <head>
        <link id="flickr-styles" rel="stylesheet" type="text/css" href="/Frontend/Composite/Media/ImageGallery/Flickr/Styles/Styles.css" />
      </head>
      <body>
        <xsl:choose>
          <xsl:when test="$response//err">
            <span class="error">
              <xsl:value-of select="$response//err/@msg" />
            </span>
          </xsl:when>
          <xsl:otherwise>
            <div class="flickr_photosets">
              <xsl:for-each select="$response//photoset">
                <div class="flickr_set">
                  <div class="flickr_set_thumb">
                    <a title="{title}" href="~/page({$pageId})/{@id}">
                      <img alt="{title}" src="http://farm{@farm}.static.flickr.com/{@server}/{@primary}_{@secret}_s.jpg" />
                    </a>
                  </div>
                  <div class="flickr_set_title">
                    <xsl:value-of select="title" />
                    <br />
                    <span class="flickr_set_count">
                      <xsl:value-of select="@photos" /> items
                    </span>
                  </div>
                </div>
              </xsl:for-each>
              <br class="clear" />
            </div>
          </xsl:otherwise>
        </xsl:choose>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>