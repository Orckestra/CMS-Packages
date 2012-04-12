<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:atom="http://www.w3.org/2005/Atom" xmlns:be="#BlogXsltExtensionsFunction" xmlns:date="#dateExtensions" exclude-result-prefixes="xsl in be">
  <xsl:template match="/">
    <rss version="2.0">
      <channel>
        <atom:link href="{concat('http://', /in:inputs/in:result[@name='ServerVariableHTTPHOST'], /in:inputs/in:result[@name='ServerVariablePATHINFO'])}" rel="self" type="application/rss+xml" />
        <title>
          <xsl:text>Blog comments</xsl:text>
        </title>
        <link>
          <xsl:value-of select="concat('http://', /in:inputs/in:result[@name='ServerVariableHTTPHOST'])" />
        </link>
        <pubDate>
          <xsl:value-of select="date:Format(date:Now(), 'r')" />
        </pubDate>
        <generator>Composite C1</generator>
        <description>
          <xsl:text>What people comment on the blog!</xsl:text>
        </description>
        <language>en</language>
        <xsl:for-each select="/in:inputs/in:result[@name='GetCommentsXml']/Comments">
          <xsl:variable name="blogPost" select="/in:inputs/in:result[@name='GetEntriesXml']/Entries[@Id=current()/@BlogEntry]" />
          <item>
            <title>
              <xsl:value-of select="concat(@Title, ' (comment to ',$blogPost/@Title,')')" />
            </title>
            <link>
              <xsl:value-of select="concat('http://', /in:inputs/in:result[@name='ServerVariableHTTPHOST'], /in:inputs/in:result[@name='SitemapXml']//Page[@Id=$blogPost/@PageId]/@URL,be:GetBlogPath($blogPost/@Date, $blogPost/@Title),'#newcomment')" />
            </link>
            <pubDate>
              <xsl:value-of select="date:Format(@Date,'r')" />
            </pubDate>
            <guid>
              <xsl:value-of select="@Id" />
            </guid>
            <description>
              <xsl:value-of select="@Comment" />
            </description>
          </item>
        </xsl:for-each>
      </channel>
    </rss>
  </xsl:template>
</xsl:stylesheet>