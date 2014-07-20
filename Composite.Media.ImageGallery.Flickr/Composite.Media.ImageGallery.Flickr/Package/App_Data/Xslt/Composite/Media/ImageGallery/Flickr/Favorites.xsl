<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
  <xsl:variable name="response" select="/in:inputs/in:result[@name='LoadUrl']/rsp" />
  <xsl:template match="/">
    <html>
      <head>
        <script id="jquery-js" src="//code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
        <script id="slimbox2-js" src="~/Frontend/Composite/Media/ImageGallery/Slimbox-2/js/slimbox2.js" type="text/javascript"></script>
        <link id="slimbox2-css" media="screen" type="text/css" href="~/Frontend/Composite/Media/ImageGallery/Slimbox-2/css/slimbox2.css" rel="stylesheet" />
        <link id="flickr-styles" rel="stylesheet" type="text/css" href="~/Frontend/Composite/Media/ImageGallery/Flickr/Styles/Styles.css" />
      </head>
      <body>
        <xsl:choose>
          <xsl:when test="$response//err">
            <span class="error">
              <xsl:value-of select="$response//err/@msg" />
            </span>
          </xsl:when>
          <xsl:otherwise>
            <div class="clearfix">
              <xsl:for-each select="$response//photo">
                <a rel="lightbox-favOf{/in:inputs/in:param[@name='UserID']}" title="{@title}" href="http://farm{@farm}.static.flickr.com/{@server}/{@id}_{@secret}.jpg">
                  <img alt="{@title}" title="{@title}" src="http://farm{@farm}.static.flickr.com/{@server}/{@id}_{@secret}_s.jpg" />
                </a>
                <div class="slimbox-img-caption" style="display:none;">
                  <strong>
                    <xsl:value-of select="@title" />
                  </strong>
                  by <a href="http://www.flickr.com/photos/{@owner}/{@id}" title="{@ownername}">
                    <xsl:value-of select="@ownername" />
                  </a>
                </div>
              </xsl:for-each>
            </div>
          </xsl:otherwise>
        </xsl:choose>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>