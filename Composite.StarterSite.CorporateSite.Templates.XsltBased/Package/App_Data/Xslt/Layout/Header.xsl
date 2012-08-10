<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
  <xsl:param name="homeUrl" select="/in:inputs/in:result[@name='HomePages']/Page[@isopen='true']/@URL" />
  <xsl:template match="/">
    <html>
      <head></head>
      <body>
        <div id="Logo">
          <a href="{$homeUrl}"></a>
        </div>
        <f:function name="Layout.Navigation.Menu" />
        <div id="HeaderImage"></div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>