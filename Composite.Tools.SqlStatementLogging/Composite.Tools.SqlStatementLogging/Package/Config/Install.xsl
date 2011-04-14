<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>
  
  <xsl:template match="add[@name='DynamicSqlDataProvider']">    
    <xsl:element name="{name()}">
      <xsl:attribute name="sqlQueryLoggingEnabled">true</xsl:attribute>
      <xsl:apply-templates select="@*[name()!='sqlQueryLoggingEnabled']"/>
      <xsl:apply-templates select="node()" />
    </xsl:element>
  </xsl:template>
  
  <xsl:template match="Composite.Data.Plugins.DataProviderConfiguration/DataProviderPlugins">
    <DataProviderPlugins>      
      <xsl:apply-templates select="@* | node()" />
    </DataProviderPlugins>
  </xsl:template>  
</xsl:stylesheet>