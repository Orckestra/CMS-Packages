<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="xsl in lang f">

	<xsl:param name="videoId" select="/in:inputs/in:param[@name='VideoId']" />
	<xsl:param name="playListId" select="/in:inputs/in:param[@name='PlayListId']" />
	<xsl:param name="autoPlay" select="/in:inputs/in:param[@name='AutoPlay']" />
	<xsl:param name="playerWidth" select="/in:inputs/in:param[@name='PlayerWidth']" />
	<xsl:param name="playerHeight" select="/in:inputs/in:param[@name='PlayerHeight']" />
	<xsl:param name="thumbSize" select="/in:inputs/in:param[@name='ThumbSize']" />
	<xsl:param name="showRelated" select="/in:inputs/in:param[@name='ShowRelated']" />
	<xsl:param name="showTitle" select="/in:inputs/in:param[@name='ShowTitle']" />
	<xsl:param name="rss" select="/in:inputs/in:result[@name='LoadUrl']" />
	<xsl:param name="cssName">
		<xsl:if test="$playListId = ''">Gallery</xsl:if>
		<xsl:if test="$playListId != ''">PlayList</xsl:if>
	</xsl:param>

	<xsl:template match="/">
		<html>
			<head>
				<script src="http://ajax.microsoft.com/ajax/jquery/jquery-1.4.2.min.js" type="text/javascript" id="jquery-1-4-2"></script>
				<script type="text/javascript" src="~/Frontend/Composite/Feeds/YouTubeReader/Scripts/jquery.youtubeplaylist.js"></script>
				<script type="text/ecmascript">
					$(function() {
					$("#YouTube").ytplaylist({
					addThumbs: true,
					autoPlay: <xsl:value-of select="$autoPlay" />,
					playerWidth: '<xsl:value-of select="$playerWidth" />',
					playerHeight: '<xsl:value-of select="$playerHeight" />',
					<xsl:choose>
						<xsl:when test="$playListId = ''">
							showInline: true,
							thumbSize: '<xsl:value-of select="$thumbSize" />',
						</xsl:when>
						<xsl:otherwise>
							holderId: '<xsl:value-of select="$playListId" />',
						</xsl:otherwise>
					</xsl:choose>
					showRelated: <xsl:value-of select="$showRelated" />
					});
					});
				</script>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Feeds/YouTubeReader/{$cssName}.css" />
			</head>
			<body>
				<div class="yt_{$cssName}">
					<xsl:if test="$playListId != ''">
						<div class="ytvideo">
							<xsl:attribute name="id">
								<xsl:value-of select="$playListId" />
							</xsl:attribute>
						</div>
					</xsl:if>
					<ul id="{$videoId}">
						<xsl:apply-templates select="$rss/rss/channel/item" />
					</ul>
				</div>
				<div class="yt_clear" />
			</body>
		</html>
	</xsl:template>
	<xsl:template match="item">
		<li>
			<xsl:if test="$playListId = ''">
				<xsl:attribute name="style">
					width: <xsl:value-of select="$playerWidth" />px;height: <xsl:value-of select="$playerHeight" />px;
				</xsl:attribute>
			</xsl:if>
			<a href="{link}">
				<xsl:if test="$showTitle = 'true'">
					<xsl:value-of select="title" />
				</xsl:if>
			</a>
		</li>
	</xsl:template>
</xsl:stylesheet>
