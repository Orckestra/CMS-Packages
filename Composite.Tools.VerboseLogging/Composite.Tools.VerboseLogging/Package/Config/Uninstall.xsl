<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>

	<!-- Chaning logging from Verbose and above to Information and above (leaving out Verbose) -->
	<xsl:template match="/configuration/loggingConfiguration/specialSources/allEvents[@name='All Events']/@switchValue[. = 'All']">
		<xsl:attribute name="switchValue">
			<xsl:value-of select="'Information'"/>
		</xsl:attribute>
	</xsl:template>
</xsl:stylesheet>