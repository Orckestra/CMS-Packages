<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" xmlns:openSearch="http://a9.com/-/spec/opensearchrss/1.0/" exclude-result-prefixes="xsl in lang f openSearch">
	<xsl:param name="rss" select="/in:inputs/in:result[@name='LoadUrl']" />
	<xsl:param name="playerWidth" select="/in:inputs/in:param[@name='PlayerWidth']" />
	<xsl:param name="playerHeight" select="/in:inputs/in:param[@name='PlayerHeight']" />
	<xsl:param name="playeroptions" select="/in:inputs/in:param[@name='PlayerOptions']" />
	<xsl:param name="listoptions" select="/in:inputs/in:param[@name='ListOptions']" />

	<xsl:param name="lightbox" select="contains($playeroptions, 'LightBox View')" />
	<xsl:param name="showRelated" select="contains($playeroptions, 'Show Related')" />
	<xsl:param name="autoPlay" select="contains($playeroptions, 'AutoPlay')" />
	<xsl:param name="fullscreen" select="contains($playeroptions,'Allow Fullscreen')" />

	<xsl:param name="showTitle" select="contains($listoptions, 'Show Title')" />
	<xsl:param name="showDescription" select="contains($listoptions, 'Show Description')" />
	<xsl:param name="showAuthor" select="contains($listoptions, 'Show Author')" />
	<xsl:param name="showViewsCount" select="contains($listoptions, 'Show Views Count')" />
	<xsl:param name="thumbSmall" select="contains($listoptions, 'Small Thumbnails')" />
	<xsl:param name="showPaging" select="contains($listoptions, 'Show Paging')" />

	<xsl:param name="pageId" select="/in:inputs/in:result[@name='GetPageId']" />
	<xsl:param name="videoId" select="/in:inputs/in:result[@name='NewGuid']"/>
	<xsl:param name="maxDescriptionLength" select="350" />

	<xsl:param name="thumbImg">
		<xsl:choose>
			<xsl:when test="$thumbSmall = 'true'">
				<xsl:text>default.jpg</xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<xsl:text>0.jpg</xsl:text>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:param>
	<xsl:template match="/">
		<html>
			<head>
				<script src="http://ajax.microsoft.com/ajax/jquery/jquery-1.4.2.min.js" type="text/javascript" id="jquery-1-4-2"></script>
				<script type="text/javascript" src="~/Frontend/Composite/Feeds/YouTubeReader/Scripts/jquery.youtubeplaylist.js" id="youtubeplaylist"></script>
				<xsl:if test="$lightbox='true'">
					<link id="youtube-lightbox-css" media="screen" type="text/css" href="/Frontend/Composite/Feeds/YouTubeReader/lightbox-css/html-lightbox.css" rel="stylesheet" />
					<script id="youtube-lightbox-js" type="text/javascript" src="/Frontend/Composite/Feeds/YouTubeReader/lightbox-js/jquery.html-lightbox.js"></script>
				</xsl:if>
				<script type="text/ecmascript">
					$(function() {
						var videoId = '<xsl:value-of select="$videoId"/>'
						$("#YouTube" + videoId).ytplaylist({
						autoPlay: <xsl:value-of select="$autoPlay" />,
						playerWidth: '<xsl:value-of select="$playerWidth" />',
						playerHeight: '<xsl:value-of select="$playerHeight" />',
						showLightbox: <xsl:value-of select="$lightbox" />,
						showRelated: <xsl:value-of select="$showRelated" />,
						allowFullScreen: <xsl:value-of select="$fullscreen" />
					});

					if (window.location.hash.indexOf('#show') == 0)
					{
						var videoid = window.location.hash.substring(5);
						$("a.video-link[href*='"+videoid+"']").click();
					}
					});
				</script>
				<style type="text/css">
					.video_paging {border-bottom: solid 1px silver; border-top: solid 1px silver; margin: 0px 0px 10px 0px;}
					.video_paging div.links {float: left; font-size: 85%;}
					.video_paging div.links a {text-decoration: none;}
					.video_paging div.info {float: right; font-weight: bold;}
					.clear {clear: both;}
					.video-list-item {margin-bottom: 10px; clear: both;}
					.video-list-item a img { display: block; float: left; padding: 3px; border: solid 1px silver; margin: 0px 15px 5px 0px;}
					.video-list-item .title, .video-list-item .description, video-list-item .author, .video-list-item .view-count {display: block; line-height: 17px; }
					.video-list-item .description {font-size: 90%;}
					.video-list-item .view-count, .video-list-item .author {color: #666666; font-size: 85%;}
				</style>
			</head>
			<body>
				<div class="video-list">
					<xsl:if test="$showPaging = 'true'">
						<xsl:call-template name="Paging" />
					</xsl:if>
					<div id="YouTube{$videoId}">
						<xsl:apply-templates select="$rss/rss/channel/item" />
						<div class="clear" />
					</div>
					<xsl:if test="$showPaging = 'true'">
						<xsl:call-template name="Paging" />
					</xsl:if>
				</div>
			</body>
		</html>
	</xsl:template>
	<xsl:template match="item">
		<div class="video-list-item">
			<xsl:variable name="videoID" select="substring-after(guid, 'http://gdata.youtube.com/feeds/api/videos/')" />
			<a href="{link}" title="{title}" class="video-link">
				<img src="http://img.youtube.com/vi/{$videoID}/{$thumbImg}" />
			</a>
			<xsl:if test="$showTitle='true'">
				<a href="{link}" class="video-link title">
					<xsl:value-of select="title" />
				</a>
			</xsl:if>
			<xsl:if test="$showDescription='true'">
				<span class="description">
					<xsl:choose>
						<xsl:when test="string-length(description) &lt; $maxDescriptionLength">
							<xsl:value-of select="description" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="substring(description,0,$maxDescriptionLength - 4)" />...
						</xsl:otherwise>
					</xsl:choose>
				</span>
			</xsl:if>
			<xsl:if test="$showAuthor='true'">
				<span class="author">
					by&#160;
					<xsl:value-of select="author" />
				</span>
			</xsl:if>
			<xsl:if test="$showViewsCount='true'">
				<span class="view-count">
					<xsl:value-of select="yt:statistics/@viewCount" xmlns:yt="http://gdata.youtube.com/schemas/2007" />&#160;views
				</span>
			</xsl:if>
			<br class="clear" />
		</div>
	</xsl:template>
	<xsl:template name="Paging">
		<xsl:variable name="totalCount" select="$rss/rss/channel/openSearch:totalResults" />
		<xsl:variable name="itemsPerPage" select="$rss/rss/channel/openSearch:itemsPerPage" />
		<xsl:variable name="startIndex" select="$rss/rss/channel/openSearch:startIndex" />
		<xsl:variable name="nextIndex" select="$startIndex + $itemsPerPage" />
		<xsl:variable name="prevIndex" select="$startIndex - $itemsPerPage" />
		<xsl:variable name="curEndIndex">
			<xsl:choose>
				<xsl:when test="($startIndex + $itemsPerPage - 1) &lt;  $totalCount">
					<xsl:value-of select="$startIndex + $itemsPerPage - 1" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$totalCount" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<div class="video_paging">
			<div class="links">
				<xsl:choose>
					<xsl:when test="$startIndex &gt; 2">
						<a name="next" rel="prev" href="~/page({$pageId})/{$prevIndex}#next">&lt;&lt; Previous</a>
					</xsl:when>
					<xsl:otherwise>&lt;&lt; Previous</xsl:otherwise>
				</xsl:choose>&#160;&#160;
				<xsl:choose>
					<xsl:when test="($startIndex + $itemsPerPage) &lt; $totalCount">
						<a name="next" rel="next" href="~/page({$pageId})/{$nextIndex}#next">Next &gt;&gt;</a>
					</xsl:when>
					<xsl:otherwise>Next &gt;&gt;</xsl:otherwise>
				</xsl:choose>
			</div>
			<div class="info">
				<xsl:value-of select="$startIndex" />&#160;-&#160;<xsl:value-of select="$curEndIndex" />&#160;of about&#160;<xsl:value-of select="$totalCount" />
			</div>
			<br class="clear" />
		</div>
	</xsl:template>
</xsl:stylesheet>