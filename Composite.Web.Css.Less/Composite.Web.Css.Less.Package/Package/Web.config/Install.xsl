<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml"
              omit-xml-declaration="yes"
              indent="yes"/>
  <xsl:strip-space elements="*"/>
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>
  <xsl:template match="/configuration/system.webServer/modules">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="not(add[@name='LessHttpModule'])">
        <add name="LessHttpModule" type="Orckestra.Web.Css.Less.LessHttpModule, Orckestra.Web.Css.Less" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>
  <xsl:template match="/configuration/system.web/httpModules">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="not(add[@name='LessHttpModule'])">
        <add name="LessHttpModule" type="Orckestra.Web.Css.Less.LessHttpModule, Orckestra.Web.Css.Less" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>