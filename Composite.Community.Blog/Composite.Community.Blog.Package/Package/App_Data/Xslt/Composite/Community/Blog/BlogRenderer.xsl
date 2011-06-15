<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" xmlns:c1="http://c1.composite.net/StandardFunctions" xmlns:mp="#MarkupParserExtensions" xmlns:be="#BlogXsltExtensionsFunction" exclude-result-prefixes="xsl in lang f c1 mp be">
	<xsl:param name="pageId" select="/in:inputs/in:result[@name='GetPageId']" />
	<xsl:param name="pagingInfo" select="/in:inputs/in:result[@name='GetEntriesXml']/PagingInfo" />
	<xsl:variable name="isBlogItem" select="be:IsBlogList()" />
	<xsl:variable name="currentCultureName" select="be:GetCurrentCultureName()" />
	<xsl:variable name="displayMode" select="/in:inputs/in:param[@name='DisplayMode']" />
	<xsl:template match="/">
		<html>
			<head>
				<xsl:if test="$isBlogItem='true'">
					<title>
						<xsl:value-of select="/in:inputs/in:result[@name='GetEntriesXml']/Entries/@Title" />
					</title>
				</xsl:if>
				<link id="BlogStyles" rel="stylesheet" type="text/css" href="~/Frontend/Composite/Community/Blog/Styles.css" />
			</head>
			<body>
				<xsl:choose>
					<xsl:when test="$isBlogItem='true'">
						<xsl:apply-templates mode="BlogItem" select="/in:inputs/in:result[@name='GetEntriesXml']/Entries" />
					</xsl:when>
					<xsl:otherwise>
						<div id="BlogList">
							<xsl:apply-templates mode="BlogList" select="/in:inputs/in:result[@name='GetEntriesXml']/Entries" />
							<xsl:if test="$pagingInfo/@TotalPageCount &gt; 1">
								<div class="BlogPaging">
									<xsl:apply-templates select="$pagingInfo" />
								</div>
							</xsl:if>
						</div>
					</xsl:otherwise>
				</xsl:choose>
			</body>
		</html>
	</xsl:template>
	
	<xsl:template mode="BlogItem" match="*">
		<xsl:if test="@Image.Id != ''">
			<img class="BlogImage" border="0" src="~/Renderers/ShowMedia.ashx?id={@Image.Id}" alt="{@Image.Title}" />
		</xsl:if>
		<div class="BlogTitle">
			<h1>
				<xsl:value-of select="@Title" />
			</h1>
		</div>
		<xsl:call-template name="Author" />
		<div class="BlogContent">
			<xsl:copy-of select="mp:ParseXhtmlBodyFragment(@Content)" />
		</div>
		<xsl:call-template name="AddThis" />
		<xsl:if test="@DisplayComments = 'true'">
			<xsl:variable name="blogComments">
				<f:function name="Composite.Community.Blog.Comments">
					<f:param name="BlogEntryGuid" value="{@Id}" />
					<f:param name="AllowNewComments" value="{@AllowNewComments}" />
				</f:function>
			</xsl:variable>
			<xsl:copy-of select="c1:CallFunction($blogComments)" />
		</xsl:if>
	</xsl:template>
	
	<xsl:template mode="BlogList" match="*">
		<xsl:variable name="Tags" select="be:GetBlogTags(@Tags)/Tag" />
		<div class="BlogItem">
			<xsl:if test="@Image.Id != ''">
				<img class="BlogImage" border="0" src="~/Renderers/ShowMedia.ashx?id={@Image.Id}" alt="{@Image.Title}" />
			</xsl:if>
			<xsl:choose>
				<xsl:when test="$displayMode = 'compact'">
					<div class="BlogTitle">
						<a href="~/Renderers/Page.aspx{be:GetBlogUrl(@Date, @Title)}?pageId={$pageId}" title="{@Title}">
							<xsl:value-of select="@Title" />
						</a>
						<a class="t-count" href="~/Renderers/Page.aspx{be:GetBlogUrl(@Date, @Title)}?pageId={$pageId}#newcomment">
							<span>
								<xsl:call-template name="CommentsCount" />
							</span>
						</a>
					</div>
					<div class="BlogTeaser">
						<xsl:value-of select="@Teaser" />
					</div>
					<xsl:if test="count($Tags)&gt;0">
						<div class="BlogTags">
							<xsl:value-of select="be:GetLocalized('Blog','subjectsText')" />
							<xsl:apply-templates mode="TagsList" select="$Tags" />
						</div>
					</xsl:if>
				</xsl:when>
				<xsl:otherwise>
					<div class="BlogTitle">
						<a href="~/Renderers/Page.aspx{be:GetBlogUrl(@Date, @Title)}?pageId={$pageId}" title="{@Title}">
							<xsl:value-of select="@Title" />
						</a>
					</div>
					<xsl:call-template name="Author" />
					<div class="BlogTeaser">
						<xsl:choose>
							<xsl:when test="$displayMode = 'teaser'">
								<xsl:value-of select="@Teaser" />
							</xsl:when>
							<xsl:otherwise>
								<xsl:copy-of select="mp:ParseXhtmlBodyFragment(@Content)" />
							</xsl:otherwise>
						</xsl:choose>
					</div>
					<xsl:if test="count($Tags)&gt;0">
						<div class="BlogTags">
							<xsl:value-of select="be:GetLocalized('Blog','subjectsText')" />
							<xsl:apply-templates mode="TagsList" select="$Tags" />
						</div>
					</xsl:if>
					<div class="BlogCommentsCount">
						<a href="~/Renderers/Page.aspx{be:GetBlogUrl(@Date, @Title)}?pageId={$pageId}#newcomment">
							<xsl:value-of select="be:GetLocalized('Blog','commentsText')" /> (<xsl:call-template name="CommentsCount" />)
						</a> &#160;|&#160;
						<a title="Blog Feed" href="/BlogRssFeed.ashx?bid={$pageId}&amp;cultureName={$currentCultureName}">RSS</a>
						<xsl:if test="$displayMode = 'content'">
							<xsl:call-template name="AddThis" />
						</xsl:if>
					</div>
				</xsl:otherwise>
			</xsl:choose>
		</div>
	</xsl:template>
	
	<xsl:template name="Author">
		<xsl:if test="@Author.Picture != ''">
			<img class="BlogAuthorPicture" border="0" src="~/Renderers/ShowMedia.ashx?i={@Author.Picture}" alt="{@Author.Name}" />
		</xsl:if>
		<div class="BlogAuthorDate">
			<xsl:value-of select="be:GetLocalized('Blog','writtenByText')" />&#160;
			<xsl:choose>
				<xsl:when test="@Author.Email != '' and @Author.DisplayEmail = 'true'">
					<a href="mailto:{@Author.Email}">
						<xsl:value-of select="@Author.Name" />
					</a>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="@Author.Name" />
				</xsl:otherwise>
			</xsl:choose>-
			<xsl:value-of select="be:CustomDateFormat(@Date, 'dd MMMM yyyy')" />
		</div>
	</xsl:template>
	
	<xsl:template mode="TagsList" match="*">
		<a href="~/Renderers/Page.aspx/{be:Encode(string(.))}?pageId={$pageId}" title="{.}">
			<xsl:value-of select="." />
		</a>
	</xsl:template>
	
	<xsl:template name="CommentsCount">
		<xsl:variable name="commentsCount" select="/in:inputs/in:result[@name='GetCommentsCount']/Comment[@Id=current()/@Id]/@Count" />
		<xsl:variable name="Count">
			<xsl:choose>
				<xsl:when test="$commentsCount">
					<xsl:value-of select="$commentsCount" />
				</xsl:when>
				<xsl:otherwise>0</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:value-of select="$Count" />
	</xsl:template>
	
	<xsl:template name="AddThis">
		<a class="AddThis" href="http://www.addthis.com/bookmark.php?v=250&amp;">
			<img src="http://s7.addthis.com/static/btn/v2/lg-share-en.gif" width="125" height="16" alt="Bookmark and Share" style="border:0" />
		</a>
		<script type="text/javascript" src="http://s7.addthis.com/js/250/addthis_widget.js"></script>
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
				<a href="~/Renderers/Page.aspx{be:GetCurrentPath()}?p={$page}&amp;pageId={$pageId}">
					<xsl:value-of select="$page" />
				</a>
			</xsl:if>
			<xsl:apply-templates select=".">
				<xsl:with-param name="page" select="$page+1" />
			</xsl:apply-templates>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>