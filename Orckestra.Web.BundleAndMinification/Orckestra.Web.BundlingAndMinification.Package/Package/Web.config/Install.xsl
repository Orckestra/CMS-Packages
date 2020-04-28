<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:ab="urn:schemas-microsoft-com:asm.v1" exclude-result-prefixes="msxsl ab">
  <xsl:output method="xml" omit-xml-declaration="yes" indent="yes"/>
  <xsl:strip-space elements="*"/>

  <xsl:variable name="structure">
    <configuration>
      <appSettings>
        <add key="Orckestra.Web.BundlingAndMinification.BundleAndMinifyScripts" value="true"/>
        <add key="Orckestra.Web.BundlingAndMinification.BundleAndMinifyStyles" value="true"/>
      </appSettings>
    </configuration>
  </xsl:variable>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
      <xsl:if test="not(/configuration/appSettings)">
        <xsl:copy-of select="msxsl:node-set($structure)/configuration/appSettings"/>
      </xsl:if>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/appSettings">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
      <xsl:if test="not(/configuration/appSettings/add[@key='Orckestra.Web.BundlingAndMinification.BundleAndMinifyScripts'])">
        <xsl:copy-of select="msxsl:node-set($structure)/configuration/appSettings/add[@key='Orckestra.Web.BundlingAndMinification.BundleAndMinifyScripts']" />
      </xsl:if>
      <xsl:if test="not(/configuration/appSettings/add[@key='Orckestra.Web.BundlingAndMinification.BundleAndMinifyStyles'])">
        <xsl:copy-of select="msxsl:node-set($structure)/configuration/appSettings/add[@key='Orckestra.Web.BundlingAndMinification.BundleAndMinifyStyles']" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>