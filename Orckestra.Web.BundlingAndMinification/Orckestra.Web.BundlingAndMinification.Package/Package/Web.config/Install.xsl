<?xml version="1.0"?>
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:ab="urn:schemas-microsoft-com:asm.v1"
                exclude-result-prefixes="msxsl ab">
  
  <xsl:output method="xml"
              omit-xml-declaration="yes"
              indent="yes"/>
  <xsl:strip-space elements="*"/>
  
  <!--
  Expected end configuration structure.
  Based on empty C1 solution, we expect, that runtime/assemplyBinding node will always exist:
  -->
  
  <xsl:variable name="structure">
    <configuration>
      <appSettings>
        <add key="Orckestra.Web.BundlingAndMinification.BundleAndMinifyScripts" value="true"/>
        <add key="Orckestra.Web.BundlingAndMinification.BundleAndMinifyStyles" value="true"/>
      </appSettings>
      <runtime>
        <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
          <dependentAssembly>
            <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral"/>
            <bindingRedirect oldVersion="0.0.0.0-6.0.0.0" newVersion="6.0.0.0" />
          </dependentAssembly>
        </assemblyBinding>
      </runtime>
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
  <xsl:template match="/configuration/runtime/ab:assemblyBinding">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
      <!--In case any other Newtonsoft.Json record exist, do nothing here. 
      The binding version will be in any case > 6.0 because it is base C1 library version-->
      <xsl:if test="not(/configuration/runtime/ab:assemblyBinding/ab:dependentAssembly[ab:assemblyIdentity[@name='Newtonsoft.Json']])">
        <xsl:copy-of select="msxsl:node-set($structure)/configuration/runtime/ab:assemblyBinding/
                     ab:dependentAssembly[ab:assemblyIdentity[@name='Newtonsoft.Json']]"/>
      </xsl:if>
      <xsl:if test="not(/configuration/runtime/ab:assemblyBinding/ab:dependentAssembly[ab:assemblyIdentity[@name='WebGrease']])">
        <xsl:copy-of select="msxsl:node-set($structure)/configuration/runtime/ab:assemblyBinding/
                     ab:dependentAssembly[ab:assemblyIdentity[@name='WebGrease']]"/>
      </xsl:if>
    </xsl:copy>
  </xsl:template>
  <xsl:template match="/configuration/appSettings">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
      <xsl:if test="not(/configuration/appSettings/add[@key='Orckestra.Web.BundlingAndMinification.BundleAndMinifyScripts'])">
        <xsl:copy-of select="msxsl:node-set($structure)/configuration/appSettings/
                     add[@key='Orckestra.Web.BundlingAndMinification.BundleAndMinifyScripts']" />
      </xsl:if>
      <xsl:if test="not(/configuration/appSettings/add[@key='Orckestra.Web.BundlingAndMinification.BundleAndMinifyStyles'])">
        <xsl:copy-of select="msxsl:node-set($structure)/configuration/appSettings/
                     add[@key='Orckestra.Web.BundlingAndMinification.BundleAndMinifyStyles']" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>