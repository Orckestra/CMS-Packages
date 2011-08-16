<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="xsl in df"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:df="#dateExtensions"
	xmlns:n="http://c1.composite.net/News"
>

  <xsl:param name="items" select="/in:inputs/in:result[@name='GetNewsItemXml']/*" />
  <xsl:param name="showPage" select="/in:inputs/in:param[@name='ShowPage']" />
  <xsl:param name="showTeaser" select="/in:inputs/in:param[@name='ShowTeaser']" />
  <xsl:param name="dateFormat" select="/in:inputs/in:param[@name='DateFormat']" />
  <xsl:param name="currentCulture" select="/in:inputs/in:result[@name='CurrentCulture']" />

  <xsl:template match="/">
    <html>
      <head>
        <link rel="alternate" type="application/rss+xml" title="" href="~/NewsRssFeed.ashx/{$currentCulture}" />
        <style type="text/css">
          .News .Date {
          font-size: 80%;
          }
          .News a.Title {
          font-weight: bold;
          }

          .News .Paging a {
          padding: 2px;
          }
        </style>
      </head>
      <body>
        <div class="News">
          <xsl:if test="count($items) &gt; 0">
            <xsl:apply-templates mode="NewsItem" select="$items" />
          </xsl:if>
        </div>
      </body>
    </html>
  </xsl:template>

  <xsl:template mode="NewsItem" match="*">
    <div>
      <xsl:choose>
        <xsl:when test="position() = 1">
          <xsl:attribute name="class">First</xsl:attribute>
        </xsl:when>
        <xsl:when test="position() = last()">
          <xsl:attribute name="class">Last</xsl:attribute>
        </xsl:when>
      </xsl:choose>
      <a class="Title" href="~/Renderers/Page.aspx{n:GetPathInfo(@TitleUrl,@Date)}?pageId={@PageId.Id}">
        <xsl:value-of select="@Title" />
      </a>
      <xsl:if test="$showPage='true'">
        <br/><a class="Page" href="~/Renderers/Page.aspx?pageId={@PageId.Id}">
          <xsl:value-of select="@PageId.MenuTitle" />
        </a>
      </xsl:if>
      <div class="Date">
        <xsl:choose>
          <xsl:when test=" $dateFormat = ''">
            <xsl:value-of select="df:LongDateFormat(@Date)" />
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="df:Format(@Date, $dateFormat)" />
          </xsl:otherwise>
        </xsl:choose>
      </div>
      <xsl:if test="$showTeaser='true'">
        <div class="Teaser">
          <xsl:value-of select="@Teaser" />
        </div>
      </xsl:if>
    </div>
  </xsl:template>

</xsl:stylesheet>
