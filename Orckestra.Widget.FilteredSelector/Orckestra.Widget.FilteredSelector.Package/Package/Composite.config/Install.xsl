<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>
  <xsl:template match="Composite.Functions.Plugins.WidgetFunctionProviderConfiguration/WidgetFunctionProviderPlugins">
    <WidgetFunctionProviderPlugins>
      <xsl:apply-templates select="@* | node()"/>
      <xsl:if test="count(add[@name='Orckestra.Widget.FilteredSelector'])=0">
        <add type="Orckestra.Widget.FilteredSelector.WidgetProvider.FilteredSelectorWidgetProvider, Orckestra.Widget.FilteredSelector" name="Orckestra.Widget.FilteredSelector"  />
      </xsl:if>
      </WidgetFunctionProviderPlugins>
  </xsl:template>
  <xsl:template match="Composite.Functions.Plugins.FunctionProviderConfiguration/FunctionProviderPlugins">
    <FunctionProviderPlugins>
      <xsl:apply-templates select="@* | node()"/>
      <xsl:if test="count(add[@name='Orckestra.Widget.FilteredSelector'])=0">
        <add type="Orckestra.Widget.FilteredSelector.FunctionProvider.FilteredSelectorFunctionProvider, Orckestra.Widget.FilteredSelector" name="Orckestra.Widget.FilteredSelector"  /> 
      </xsl:if>
    </FunctionProviderPlugins>
  </xsl:template>
</xsl:stylesheet>