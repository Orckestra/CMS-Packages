<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/Composite.C1Console.Elements.Plugins.ElementActionProviderConfiguration/ElementActionProviderPlugins">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="count(add[@name='AzureBlobValidatorElementActionProvider'])=0">
        <add name="AzureBlobValidatorElementActionProvider" type="Composite.Tools.AzureBlobVerifier.ValidatorElementActionProvider, Composite.Tools.AzureBlobVerifier" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
