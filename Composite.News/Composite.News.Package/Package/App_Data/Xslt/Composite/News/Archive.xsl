<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:n="http://c1.composite.net/News"
	xmlns:df="#dateExtensions"
	xmlns:mp="#MarkupParserExtensions"
	exclude-result-prefixes="xsl in df n">

	<xsl:param name="items" select="/in:inputs/in:result[@name='GetNewsItemXml']/NewsItem" />
	<xsl:param name="pageId" select="/in:inputs/in:result[@name='GetPageId']" />
	<xsl:param name="pagingInfo" select="/in:inputs/in:result[@name='GetNewsItemXml']/PagingInfo" />
	<xsl:param name="showTeaser" select="/in:inputs/in:param[@name='ShowTeaser']" />
	<xsl:param name="showTeaserInDetails" select="/in:inputs/in:param[@name='ShowTeaserInDetails']" />
	<xsl:param name="dateFormat" select="/in:inputs/in:param[@name='DateFormat']" />
	<xsl:param name="isNewsList" select="n:IsNewsList()" />

	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/News/Styles.css" />
			</head>
			<body>
				<div class="News">
					<xsl:choose>
						<xsl:when test="$isNewsList = 'false'">
							<xsl:if test="count($items) &gt; 0">
								<ul>
									<xsl:apply-templates mode="NewsItem" select="$items" />
								</ul>
							</xsl:if>
							<xsl:if test="$pagingInfo/@TotalPageCount &gt; 1">
								<div class="Paging">
									<xsl:apply-templates select="$pagingInfo" />
								</div>
							</xsl:if>
						</xsl:when>
						<xsl:otherwise>
							<xsl:apply-templates mode="NewsDetails" select="$items" />
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</body>
		</html>
	</xsl:template>

	<xsl:template mode="NewsItem" match="*">
		<li>
			<xsl:if test="position() = 1">
				<xsl:attribute name="class">First</xsl:attribute>
			</xsl:if>
			<xsl:if test="position() = last()">
				<xsl:attribute name="class">Last</xsl:attribute>
			</xsl:if>
			<span class="Title">
				<a href="~/Renderers/Page.aspx{n:GetPathInfo(@TitleUrl, @Date)}?pageId={$pageId}">
					<xsl:value-of select="@Title" />
				</a>
			</span>
			<span class="Date">
				<xsl:choose>
					<xsl:when test=" $dateFormat = ''">
						<xsl:value-of select="df:LongDateFormat(@Date)" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="df:Format(@Date, $dateFormat)"/>
					</xsl:otherwise>
				</xsl:choose>
			</span>
			<xsl:if test="$showTeaser='true'">
				<div class="Teaser">
					<xsl:value-of select="@Teaser" />
				</div>
			</xsl:if>
		</li>
	</xsl:template>

	<xsl:template mode="NewsDetails" match="*">
		<div class="Item">
			<div class="Title">
				<xsl:value-of select="@Title" />
			</div>
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
			<xsl:if test="$showTeaserInDetails='true'">
				<div class="Teaser">
					<xsl:value-of select="@Teaser" />
				</div>
			</xsl:if>
			<div class="Description">
				<xsl:copy-of select="mp:ParseXhtmlBodyFragment(@Description)" />
			</div>
		</div>
	</xsl:template>

	<xsl:template match="PagingInfo">
		<xsl:param name="page" select="1" />
		<xsl:if test="$page &lt; @TotalPageCount + 1">
			<xsl:if test="$page = @CurrentPageNumber">
				<span>
					<xsl:value-of select="$page" />
				</span>
			</xsl:if>
			<xsl:if test="not($page = @CurrentPageNumber)">
				<a href="~/Renderers/Page.aspx/{$page}?pageId={$pageId}">
					<xsl:value-of select="$page" />
				</a>
			</xsl:if>
			<xsl:apply-templates select=".">
				<xsl:with-param name="page" select="$page+1" />
			</xsl:apply-templates>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>