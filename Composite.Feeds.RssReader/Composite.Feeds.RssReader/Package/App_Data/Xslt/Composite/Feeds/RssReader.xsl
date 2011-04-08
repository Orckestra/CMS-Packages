<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="xsl in msxsl csharp"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:csharp="http://c1.composite.net/csharp">

	<xsl:param name="rss" select="/in:inputs/in:result[@name='LoadUrl']" />
	<xsl:param name="listLength" select="/in:inputs/in:param[@name='ListLength']" />

	<xsl:template match="/">
		<xsl:apply-templates select="$rss" />
	</xsl:template>

	<xsl:template match="rss/channel">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Feeds/RssReader/Styles.css" />
			</head>
			<body>
				<div class="RSSReader">
					<xsl:apply-templates select="title" />
					<xsl:apply-templates select="item[position() &lt; $listLength + 1]" />
				</div>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="title">
		<xsl:value-of select="text()" />
	</xsl:template>

	<xsl:template match="item">
		<div class="ArticleEntry">
			<div class="ArticleTitle">
				<a href="{link}">
					<xsl:value-of select="title" />
				</a>
			</div>
			<div class="ArticleDate">
				<xsl:value-of select="csharp:FormatDate(string(pubDate), 'D')" />
			</div>
			<div class="ArticleDescription">
				<xsl:apply-templates select="description" mode="striptag" />
			</div>
		</div>
	</xsl:template>

	<xsl:template mode="striptag" match="*">
		<xsl:param name="str" select="." />
		<xsl:choose>
			<xsl:when test="string-length(substring-after($str,'&lt;')) = 0">
				<xsl:value-of select="$str" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="substring-before($str,'&lt;')" />
				<xsl:if test="string-length(substring-after($str,'&gt;'))>0">
					<xsl:apply-templates select="." mode="striptag">
						<xsl:with-param name="str" select="substring-after($str,'&gt;')" />
					</xsl:apply-templates>
				</xsl:if>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<msxsl:script language="C#" implements-prefix="csharp">
		<![CDATA[
		public string FormatDate(string date, string format)
		{
			try
			{
				return DateTime.Parse(date).ToString(format);
			}
			catch
			{
				return date;
			}
		}
	]]>
	</msxsl:script>

</xsl:stylesheet>