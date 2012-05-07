<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns="http://www.w3.org/1999/xhtml" xmlns:n="http://c1.composite.net/News" xmlns:df="#dateExtensions" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:mp="#MarkupParserExtensions" exclude-result-prefixes="xsl in df n lang">
	<xsl:param name="pagingInfo" select="/in:inputs/in:result[@name='GetNewsItemXml']/PagingInfo" />
	<xsl:param name="listOptions" select="/in:inputs/in:param[@name='ListOptions']" />
	<xsl:param name="itemOptions" select="/in:inputs/in:param[@name='ItemOptions']" />
	<xsl:param name="dateFormat" select="/in:inputs/in:param[@name='DateFormat']" />
	<xsl:param name="isNewsList" select="n:IsNewsList()" />
	<xsl:param name="currentCulture" select="/in:inputs/in:result[@name='CurrentCulture']" />
	<xsl:param name="pageId" select="/in:inputs/in:result[@name='GetPageId']" />
	<xsl:template match="/">
		<html>
			<head>
				<xsl:if test="$isNewsList=false">
					<title>
						<xsl:value-of select="/in:inputs/in:result[@name='GetNewsItemXml']/NewsItem/@Title" />
					</title>
				</xsl:if>
				<style type="text/css">
				.News .First h1 {margin-top: 0px;}
          		.News .Date {font-size: 80%;}
				.News .Paging, .News .RSS, .News .BackToAll {margin-top: 20px;}
           		.News .Paging a {padding: 5px;}
        	</style>
			</head>
			<body>
				<div class="News">
					<xsl:choose>
						<xsl:when test="$isNewsList">
							<xsl:apply-templates mode="NewsItem" select="/in:inputs/in:result[@name='GetNewsItemXml']/NewsItem">
								<xsl:with-param name="options" select="$listOptions" />
							</xsl:apply-templates>
							<xsl:if test="contains($listOptions,'Show RSS')">
								<div class="RSS">
									<a title="News RSS Feed" href="~/NewsRssFeed.ashx/{$currentCulture}">RSS</a>
								</div>
							</xsl:if>
							<xsl:if test="$pagingInfo/@TotalPageCount &gt; 1">
								<div class="Paging">
									<xsl:apply-templates select="$pagingInfo" />
								</div>
							</xsl:if>
						</xsl:when>
						<xsl:otherwise>
							<xsl:apply-templates mode="NewsItem" select="/in:inputs/in:result[@name='GetNewsItemXml']/NewsItem">
								<xsl:with-param name="options" select="$itemOptions" />
							</xsl:apply-templates>
							<xsl:if test="contains($itemOptions,'Back to News')">
								<div class="BackToAll">
									<a href="~/page({$pageId})">
										<lang:string key="Resource, Resources.News.labelBackToNews" />
									</a>
								</div>
							</xsl:if>
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</body>
		</html>
	</xsl:template>
	<xsl:template mode="NewsItem" match="*">
		<xsl:param name="options" />
		<div>
			<xsl:if test="position() = 1">
				<xsl:attribute name="class">First</xsl:attribute>
			</xsl:if>
			<h1>
				<xsl:choose>
					<xsl:when test="$isNewsList">
						<a href="~/page({$pageId}){n:GetPathInfo(@TitleUrl, @Date)}">
							<xsl:value-of select="@Title" />
						</a>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="@Title" />
					</xsl:otherwise>
				</xsl:choose>
			</h1>
			<xsl:if test="contains($options, 'Show date')">
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
			</xsl:if>
			<xsl:if test="contains($options, 'Show teaser')">
				<div class="Teaser">
					<p>
						<xsl:value-of select="@Teaser" />
					</p>
				</div>
			</xsl:if>
			<xsl:if test="contains($options, 'Show description')">
				<div class="Description">
					<xsl:copy-of select="mp:ParseXhtmlBodyFragment(@Description)" />
				</div>
			</xsl:if>
			<xsl:if test="contains($options, 'Show share icons')">
				<div class="Share">
					<!-- AddThis Button BEGIN -->
					<div class="addthis_toolbox addthis_default_style ">
						<a class="addthis_button_preferred_1"></a>
						<a class="addthis_button_preferred_2"></a>
						<a class="addthis_button_compact"></a>
						<a class="addthis_counter addthis_bubble_style"></a>
					</div>
					<script type="text/javascript" src="http://s7.addthis.com/js/250/addthis_widget.js#pubid=xa-4f9a9e3a16daa79f"></script>
					<!-- AddThis Button END -->
				</div>
			</xsl:if>
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
				<a href="~/page({$pageId})/{$page}">
					<xsl:value-of select="$page" />
				</a>
			</xsl:if>
			<xsl:apply-templates select=".">
				<xsl:with-param name="page" select="$page+1" />
			</xsl:apply-templates>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>