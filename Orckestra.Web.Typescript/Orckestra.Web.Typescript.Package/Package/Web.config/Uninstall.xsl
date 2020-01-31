<?xml version="1.0"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:ab="urn:schemas-microsoft-com:asm.v1">

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>
  
  <xsl:template match="/configuration/appSettings/add[@key='Orckestra.Web.Typescript.Enable']" />
  <xsl:template match="/configuration/system.webServer/modules/add[@name='TypescriptHttpModule']" />
  
</xsl:stylesheet>