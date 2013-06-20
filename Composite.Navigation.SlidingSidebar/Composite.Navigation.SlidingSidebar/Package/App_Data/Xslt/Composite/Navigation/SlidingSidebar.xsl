<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="xsl in"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:data="http://www.composite.net/ns/data"
	xmlns="http://www.w3.org/1999/xhtml">
  <xsl:output indent="no" method="xml" />

  <xsl:param name="parent" select="/in:inputs/in:param[@name='Parent']" />
  <xsl:param name="childs" select="/in:inputs/in:param[@name='Childs']" />
  <xsl:param name="expand" select="/in:inputs/in:param[@name='Expand']" />
  <xsl:param name="title" select="/in:inputs/in:param[@name='Title']" />
  <xsl:param name="position" >
    <xsl:if test="/in:inputs/in:param[@name='Position'] = 'true'">right</xsl:if>
    <xsl:if test="/in:inputs/in:param[@name='Position'] = 'false'">left</xsl:if>
  </xsl:param >

  <xsl:template match="/">
    <html>
      <head>
        <link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Navigation/SlidingSidebar/Styles.css" />
        <script id="jquery-js" src="//code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
        <script type="text/javascript" src="~/Frontend/Composite/Navigation/SlidingSidebar/Scripts/side-bar.js"></script>
      </head>
      <body>
        <div id="SlidingSidebar" class="{$position}">
          <a href="#" id="SlidingSidebarTab" class="SlidingSidebarTab{$position}"></a>
          <div id="SlidingSidebarContents" style="width:0px;">
            <div id="SlidingSidebarContentsInner">
              <h2>
                <xsl:value-of select="$title" />
              </h2>
              <xsl:apply-templates mode="Level" select="/in:inputs/in:result[@name='SitemapXml']">
                <xsl:with-param name="level" select="/in:inputs/in:param[@name='Level']" />
              </xsl:apply-templates>
            </div>
          </div>
        </div>
      </body>
    </html>
  </xsl:template>

  <xsl:template mode="Level" match="*">
    <xsl:param name="level" />
    <xsl:if test="$level &lt; 1">
      <ul>
        <xsl:if test="$parent='true'">
          <xsl:apply-templates mode="Page" select=".">
            <xsl:with-param name="showchilds" select="'false'" />
          </xsl:apply-templates>
        </xsl:if>
        <xsl:apply-templates mode="Page" select="./*" />
      </ul>
    </xsl:if>
    <xsl:if test="$level &gt; 0">
      <xsl:apply-templates mode="Level" select="./*[@isopen='true']">
        <xsl:with-param name="level" select="$level - 1" />
      </xsl:apply-templates>
    </xsl:if>
  </xsl:template>

  <xsl:template mode="Page" match="*">
    <xsl:param name="showchilds" select="$childs" />
    <xsl:if test="@MenuTitle != ''">
      <li>
        <xsl:if test="@isopen='true'">
          <xsl:attribute name="class">NavigationOpen</xsl:attribute>
        </xsl:if>
        <xsl:if test="@iscurrent='true'">
          <xsl:attribute name="class">NavigationSelected</xsl:attribute>
        </xsl:if>
        <a href="{@URL}">
          <xsl:if test="@isopen='true'">
            <xsl:attribute name="class">NavigationOpen</xsl:attribute>
          </xsl:if>
          <xsl:if test="@iscurrent='true'">
            <xsl:attribute name="class">NavigationSelected</xsl:attribute>
          </xsl:if>
          <xsl:value-of select="@MenuTitle" />
        </a>
        <xsl:if test="count(./*)>0 and $showchilds='true' and (@isopen='true' or $expand='true')">
          <ul>
            <xsl:apply-templates mode="Page" select="./*" />
          </ul>
        </xsl:if>
      </li>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>