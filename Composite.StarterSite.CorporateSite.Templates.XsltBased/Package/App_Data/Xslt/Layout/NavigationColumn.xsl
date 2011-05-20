<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
  <xsl:template match="/">
    <html>
      <head></head>
      <body>
        <nav>
        <div class="contentbox">
          <f:function name="Composite.Navigation.Distributed">
            <f:param name="Level" value="2" />
            <f:param name="Parent" value="False" />
            <f:param name="Childs" value="True" />
            <f:param name="Expand" value="False" />
            <f:param name="NavigationId" value="NavigationMenu" />
          </f:function>
        </div>
        </nav>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>