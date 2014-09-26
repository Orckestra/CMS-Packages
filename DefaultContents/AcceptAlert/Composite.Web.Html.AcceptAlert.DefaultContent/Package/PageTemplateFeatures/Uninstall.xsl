<?xml version="1.0"?>
<xsl:stylesheet version="1.0"
								xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
								xmlns="http://www.w3.org/1999/xhtml"
								xmlns:x="http://www.w3.org/1999/xhtml"
								exclude-result-prefixes="x"
								xmlns:f="http://www.composite.net/ns/function/1.0"
								>
  <xsl:output indent="yes"  method="html" omit-xml-declaration="yes"/>
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="f:function[@name='Composite.Web.Html.CountrySpecificContent']"  xmlns:f="http://www.composite.net/ns/function/1.0"/>
</xsl:stylesheet>