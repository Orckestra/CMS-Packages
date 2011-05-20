<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" xmlns:x="http://www.w3.org/1999/xhtml" xmlns:rendering="http://www.composite.net/ns/rendering/1.0" exclude-result-prefixes="xsl in lang f x">
  <xsl:template match="/">
    <html>
      <head></head>
      <body>
        <xsl:if test="/in:inputs/in:param[@name='Content']/node()">
          <article>
            <div class="contentbox">
              <xsl:if test="/in:inputs/in:param[@name='ShowTitle'] = 'true'">
                <h1>
                  <rendering:page.title />
                </h1>
              </xsl:if>
              <xsl:copy-of select="/in:inputs/in:param[@name='Content']/node()" />
            </div>
          </article>
        </xsl:if>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>