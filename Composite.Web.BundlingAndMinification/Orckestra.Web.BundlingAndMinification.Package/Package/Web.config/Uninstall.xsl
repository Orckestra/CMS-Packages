<?xml version="1.0"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:ab="urn:schemas-microsoft-com:asm.v1">
  
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/system.webServer/modules/add[@name='OrckestraWebBundlingAndMinificationResponseFilter']" />
  <xsl:template match="/configuration/appSettings/add[@key='Orckestra.Web.BundlingAndMinification.BundleAndMinifyScripts']" />
  <xsl:template match="/configuration/appSettings/add[@key='Orckestra.Web.BundlingAndMinification.BundleAndMinifyStyles']" />
  <xsl:template match="/configuration/appSettings/add[@key='Orckestra.Web.BundlingAndMinification.RemoveComments']" />
 <!-- Do nothing with Newtonsoft.Json binding, because it is widely used library and such binding can be used by other apps-->
</xsl:stylesheet>