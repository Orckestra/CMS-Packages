<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>
  <xsl:template match="/configuration/Composite.C1Console.Forms.Plugins.UiControlFactoryConfiguration/Channels/Channel[@name='AspNet.FormsRenderer']" />
  <xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Forms.Renderer']" />
  <xsl:template match="/configuration/Composite.Functions.Plugins.FunctionProviderConfiguration/FunctionProviderPlugins/add[@name='FormsRendererFunctionProvider']" />
</xsl:stylesheet>