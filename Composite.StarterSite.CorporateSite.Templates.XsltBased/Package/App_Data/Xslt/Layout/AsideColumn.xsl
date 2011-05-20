<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns:x="http://www.w3.org/1999/xhtml" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f x">
  <xsl:template match="/">
    <html>
      <head></head>
      <body>
        <aside>
          <xsl:if test="/in:inputs/in:param[@name='Aside']/node()">
            <div class="contentbox">
              <xsl:copy-of select="/in:inputs/in:param[@name='Aside']/node()" />
            </div>
          </xsl:if>
        </aside>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>