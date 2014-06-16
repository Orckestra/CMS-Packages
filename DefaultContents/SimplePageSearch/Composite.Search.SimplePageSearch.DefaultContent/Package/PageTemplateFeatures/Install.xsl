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

  <xsl:template match="/x:html/x:body">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="count(f:function[@name='Composite.Search.SimplePageSearch.NavbarSearchForm'])=0">
        <f:function name="Composite.Search.SimplePageSearch.NavbarSearchForm" xmlns:f="http://www.composite.net/ns/function/1.0">
          <f:param name="SearchResultPage" value="fec78884-8f7a-4f4d-b9b7-eb0c9654c378" />
        </f:function>
      </xsl:if>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>