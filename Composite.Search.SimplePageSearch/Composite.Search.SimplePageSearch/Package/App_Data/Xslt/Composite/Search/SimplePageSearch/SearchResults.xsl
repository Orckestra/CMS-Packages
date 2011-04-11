<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="xsl in">

	<xsl:param name="items" select="/in:inputs/in:result[@name='GetSearchResult']/Page" />
	<xsl:param name="pagingInfo" select="/in:inputs/in:result[@name='GetSearchResult']/PagingInfo" />
	<xsl:param name="pageId" select="/in:inputs/in:result[@name='GetPageId']" />
	<xsl:param name="searchQuery" select="/in:inputs/in:param[@name='SearchQuery']" />
	<xsl:template match="/">
		<html>
			<xsl:choose>
				<xsl:when test="$searchQuery = ''">
					<head />
					<body>
						<!-- show search form if no querying were done -->
						<f:function  name="Composite.Search.SimplePageSearch.SearchForm" xmlns:f="http://www.composite.net/ns/function/1.0">
							<f:param name="SearchResultPage" value="{$pageId}" />
						</f:function>
					</body>
				</xsl:when>
				<xsl:otherwise>
					<head>
						<title>Results for <xsl:value-of select="concat('&quot;',$searchQuery,'&quot;')" /></title>
						<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Search/SimplePageSearch/Styles.css" />
					</head>
					<body>
						<div id="SearchResults">
							<p>
								Found <xsl:value-of select="$pagingInfo/@HitsTotal" /> results for <xsl:value-of select="concat('&quot;',$searchQuery,'&quot;')" />
							</p>
							<xsl:if test="count($items)">
								<ol id="Results">
									<xsl:for-each select="$items">
										<li>
											<a href="{@Url}">
												<xsl:value-of select="@Title" />
											</a>
										</li>
									</xsl:for-each>
								</ol>
							</xsl:if>
							<xsl:if test="$pagingInfo/@PageTotal &gt; 1">
								<div class="Paging">
									<xsl:apply-templates select="$pagingInfo" />
								</div>
							</xsl:if>
						</div>
					</body>
				</xsl:otherwise>
			</xsl:choose>
		</html>
	</xsl:template>
	
	<xsl:template match="PagingInfo">
		<xsl:param name="page" select="1" />
		<xsl:variable name="currentSitePart">
			<xsl:if test="@CurrentSite='true'">
				<xsl:text>&amp;CurrentSite=true</xsl:text>
			</xsl:if>
		</xsl:variable>
		<xsl:variable name="pagePart">
			<xsl:if test="not($page = 1)">
				<xsl:text>&amp;Page=</xsl:text>
				<xsl:value-of select="$page" />
			</xsl:if>
		</xsl:variable>
		<xsl:if test="$page &lt; @PageTotal + 1">
			<xsl:if test="$page = @PageCount">
				<span>
					<xsl:value-of select="$page" />
				</span>
			</xsl:if>
			<xsl:if test="not($page = @PageCount)">
				<a href="~/Renderers/Page.aspx?SearchQuery={@SearchQuery}{$currentSitePart}{$pagePart}&amp;pageId={$pageId}">
					<xsl:value-of select="$page" />
				</a>
			</xsl:if>
			<xsl:apply-templates select=".">
				<xsl:with-param name="page" select="$page+1" />
			</xsl:apply-templates>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>