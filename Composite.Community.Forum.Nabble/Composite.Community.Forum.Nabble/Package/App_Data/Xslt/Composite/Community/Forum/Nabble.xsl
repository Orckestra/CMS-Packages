<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" 
xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
xmlns:in="http://www.composite.net/ns/transformation/input/1.0" 
xmlns:lang="http://www.composite.net/ns/localization/1.0" 
xmlns:f="http://www.composite.net/ns/function/1.0" 
xmlns="http://www.w3.org/1999/xhtml" 
xmlns:mp="#MarkupParserExtensions" 
exclude-result-prefixes="xsl in lang f mp">
  <xsl:variable name="code" select="/in:inputs/in:param[@name='EmbedCode']" />
  <xsl:template match="/">
    <html>
      <head></head>
      <body>
        <div>
          <xsl:copy-of select="mp:ParseXhtmlBodyFragment($code)" />
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>