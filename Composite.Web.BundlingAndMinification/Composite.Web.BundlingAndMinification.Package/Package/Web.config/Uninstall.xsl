<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>
  <xsl:template match="/configuration/runtime/assemblyBinding/dependentAssembly/assemblyIdentity[@name='WebGrease']" />
  <xsl:template match="/configuration/runtime/assemblyBinding/dependentAssembly/bindingRedirect[@oldVersion='0.0.0.0-1.5.2.14234']" />
</xsl:stylesheet>