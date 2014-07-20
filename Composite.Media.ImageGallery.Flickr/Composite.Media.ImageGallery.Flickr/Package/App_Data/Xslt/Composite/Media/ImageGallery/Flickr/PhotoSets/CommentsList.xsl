<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
  <xsl:variable name="response" select="/in:inputs/in:result[@name='LoadUrl']/rsp" />
  <xsl:template match="/">
    <html>
      <head>
        <!-- markup placed here will be shown in the head section of the rendered page -->
      </head>
      <body>
        <xsl:if test="$response//comment">
          <div class="flickr_set_comments clearfix">
            <h4>Comments on this set:</h4>
            <xsl:for-each select="$response//comment">
              <div class="flickr_comment">
                <strong>
                  <a href="http://www.flickr.com/photos/{@author}/" title="Photos of {@authorname}">
                    <xsl:value-of select="@authorname" />
                  </a> says:
                </strong>
                <p>
                  <span>
                    <xsl:value-of select="." />
                  </span>
                </p>
              </div>
            </xsl:for-each>
          </div>
        </xsl:if>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>