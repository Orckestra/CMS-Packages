<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" 
xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
xmlns:in="http://www.composite.net/ns/transformation/input/1.0" 
xmlns:lang="http://www.composite.net/ns/localization/1.0" 
xmlns:f="http://www.composite.net/ns/function/1.0" 
xmlns="http://www.w3.org/1999/xhtml" 
exclude-result-prefixes="xsl in lang f">
  
  <xsl:param name="size" select="/in:inputs/in:param[@name='Size']" />
  <xsl:param name="count" select="/in:inputs/in:param[@name='Count']" />
  <xsl:param name="lang" select="/in:inputs/in:result[@name='CurrentCulture']" />
  <xsl:param name="url" select="/in:inputs/in:param[@name='URL']" />
  <xsl:template match="/">
    <html>
      <head>
        <script id="plusone-js" type="text/javascript" src="https://apis.google.com/js/plusone.js">
          {"lang": '<xsl:value-of select="$lang" />'}
        </script>
      </head>
      <body>
        <div class="g-plusone" 
             data-size="{$size}" 
             data-count="{$count}"
             data-href="{$url}">
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>