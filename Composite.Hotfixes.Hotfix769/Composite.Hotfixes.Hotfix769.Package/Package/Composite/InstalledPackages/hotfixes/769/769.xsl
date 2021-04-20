<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:ui="http://www.w3.org/1999/xhtml"
	xmlns:x="http://www.w3.org/1999/xhtml">

  <xsl:param name="mode">operate</xsl:param>
  <xsl:param name="browser">opera</xsl:param>
  <xsl:param name="platform">amigaos</xsl:param>
  <xsl:param name="version">-1</xsl:param>
  <xsl:param name="appVirtualPath"></xsl:param>

  <xsl:template match="/|@*|*|processing-instruction()|comment()">
    <xsl:copy>
      <xsl:apply-templates select="*|@*|text()|processing-instruction()|comment()" />
    </xsl:copy>
  </xsl:template>

 
  <xsl:template match="/x:html/x:head">
    <xsl:copy>
      <xsl:apply-templates select="*|@*|text()|processing-instruction()|comment()" />

	  <script type="application/javascript" src="/Composite/InstalledPackages/hotfixes/769/769.js"></script>
	  <link rel="stylesheet" type="text/css" href="/Composite/InstalledPackages/hotfixes/769/769.css" />
    
    </xsl:copy>
  </xsl:template>

  
   <xsl:template match="ui:popup">
    <ui:plainpopup>
	  <xsl:attribute name="binding">PopupBinding</xsl:attribute>
      <xsl:apply-templates select="*|@*|text()"/>
    </ui:plainpopup>
  </xsl:template>

</xsl:stylesheet>