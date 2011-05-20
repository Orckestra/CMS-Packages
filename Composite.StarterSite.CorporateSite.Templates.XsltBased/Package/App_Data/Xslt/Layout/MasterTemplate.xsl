<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" xmlns:x="http://www.w3.org/1999/xhtml" xmlns:de="#dateExtensions" exclude-result-prefixes="xsl in lang f de x">

  <xsl:variable name="ActiveLanguagesCount" select="/in:inputs/in:result[@name='GetActiveLanguagesCount']/text()" />

  <xsl:template match="/">
    <html>
      <head>
        <f:function name="Layout.CommonHtmlHead">
          <f:param name="CustomTitle">
            <xsl:copy-of select="/in:inputs/in:param[@name='CustomDefaultTitle']/node()" />
          </f:param>
        </f:function>
      </head>

      <body>
        <xsl:if test="$documentBodyClass != ''">
          <xsl:attribute name="class">
            <xsl:value-of select="$documentBodyClass" />
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="not($navigationColumnCount + $contentColumnCount + $asideColumnCount = 12)">
          <small style="color: #FF0000;">Warning: Sum of columns must be 12 - please fix this in the layout template.</small>
        </xsl:if>

        <div class="container" id="wrapper">
          <div class="row">
            <div id="header" class="twelvecol last">
              <f:function name="Layout.Header" />
            </div>
          </div>
          <div class="row">
            <xsl:if test="$navigationColumnCount!=0">
              <div id="navigationcolumn">
                <xsl:attribute name="class">
                  <xsl:apply-templates select="$navigationColumnCount" mode="Column" />
                </xsl:attribute>
                <f:function name="Layout.NavigationColumn" />
              </div>
            </xsl:if>
            <div id="contentcolumn">
              <xsl:attribute name="class">
                <xsl:apply-templates select="$contentColumnCount" mode="Column" />
                <xsl:if test="$asideColumnCount=0">
                  <xsl:text> last</xsl:text>
                </xsl:if>
              </xsl:attribute>
              <div id="content">
                <f:function name="Layout.ContentColumn">
                  <f:param name="Content">
                    <xsl:copy-of select="$content" />
                  </f:param>
                  <f:param name="ShowTitle" value="{/in:inputs/in:param[@name='ShowTitle']}" />
                </f:function>
              </div>
            </div>
            <xsl:if test="$asideColumnCount!=0">
              <div id="asidecolumn">
                <xsl:attribute name="class">
                  <xsl:apply-templates select="$asideColumnCount" mode="Column" />
                  <xsl:text> last</xsl:text>
                </xsl:attribute>
                <f:function name="Layout.AsideColumn">
                  <f:param name="Aside">
                    <xsl:copy-of select="$aside" />
                  </f:param>
                </f:function>
              </div>
            </xsl:if>
          </div>
          <div id="push"></div>
        </div>
        <div class="container" id="footer">
          <div class="row">
            <div class="twelvecol last">
              <f:function name="Layout.Footer" />
            </div>
          </div>
        </div>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="*" mode="Column">
    <xsl:choose>
      <xsl:when test=". = '1'">onecol</xsl:when>
      <xsl:when test=". = '2'">twocol</xsl:when>
      <xsl:when test=". = '3'">threecol</xsl:when>
      <xsl:when test=". = '4'">fourcol</xsl:when>
      <xsl:when test=". = '5'">fivecol</xsl:when>
      <xsl:when test=". = '6'">sixcol</xsl:when>
      <xsl:when test=". = '7'">sevencol</xsl:when>
      <xsl:when test=". = '8'">eightcol</xsl:when>
      <xsl:when test=". = '9'">ninecol</xsl:when>
      <xsl:when test=". = '10'">tencol</xsl:when>
      <xsl:when test=". = '11'">elevencol</xsl:when>
      <xsl:when test=". = '12'">twelvecol</xsl:when>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="filterEmptyDocument">
    <xsl:param name="raw" />
    <!--  if we get called with a completely empty xhtml document we throw it  away here - so code 'tasting' if we got content wont taste tmpty docs  -->
    <xsl:if test="not( count($raw)=1 and $raw/../x:html and count($raw/*/node())=0 )">
      <xsl:copy-of select="$raw" />
    </xsl:if>
  </xsl:template>

  <xsl:param name="navigationColumnCount" select="/in:inputs/in:param[@name='NavigationColumnCount']" />
  <xsl:param name="contentColumnCount" select="/in:inputs/in:param[@name='ContentColumnCount']" />
  <xsl:param name="asideColumnCount" select="/in:inputs/in:param[@name='AsideColumnCount']" />
  <xsl:param name="documentBodyClass" select="/in:inputs/in:param[@name='DocumentBodyClass']" />

  <xsl:param name="content">
    <xsl:call-template name="filterEmptyDocument">
      <xsl:with-param name="raw" select="/in:inputs/in:param[@name='Content']/node()" />
    </xsl:call-template>
  </xsl:param>
  <xsl:param name="aside">
    <xsl:call-template name="filterEmptyDocument">
      <xsl:with-param name="raw" select="/in:inputs/in:param[@name='Aside']/node()" />
    </xsl:call-template>
  </xsl:param>

</xsl:stylesheet>