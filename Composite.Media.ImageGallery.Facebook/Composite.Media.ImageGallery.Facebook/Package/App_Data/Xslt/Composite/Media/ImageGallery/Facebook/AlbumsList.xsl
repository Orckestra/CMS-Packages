<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
	<xsl:variable name="access_token" select="/in:inputs/in:param[@name='AccessToken']" />
	<xsl:variable name="showFacebookLinks" select="/in:inputs/in:param[@name='ShowFacebookLinksToTheAlbum']" />
	<xsl:variable name="id" select="/in:inputs/in:param[@name='ObjectUniqueID']" />
	<xsl:template match="/">
		<html>
			<head>
				<style type="text/css">
					.album_wrapper {line-height: 18px;}
					.album_item {float: left; margin: 8px;}
					.clear {clear: both;}
					.album_item a.img_a, span.img_wrap1, span.img_wrap2 {display:block; width: 125px; height:100px; background: white;}
					.album_item a.img_a {position:relative; border: solid 1px silver;  padding: 4px; clear: both;}
					.album_item a span.img_wrap1 {position: absolute; top: 4px; left: 4px; padding: 4px; border: solid 1px silver;}
					.album_item a span.img_wrap2 {overflow:hidden; background-repeat: no-repeat; background-position: center 25%;}
					.album_title{font-weight:bold; font-size: 11px; margin-top: 4px;}
					.album_items_count {color: #808080; font-size: 11px;}
					.error {color: red;}
				</style>
			</head>
			<body>
				<xsl:apply-templates select="/in:inputs/in:result[@name='GetAlbums']/Error" />
				<div class="album_wrapper">
					<xsl:apply-templates select="/in:inputs/in:result[@name='GetAlbums']/Album" mode="Album" />
					<br class="clear" />
				</div>
			</body>
		</html>
	</xsl:template>
	<xsl:template mode="Album" match="*">
		<div class="album_item">
			<a class="img_a" title="{@Name}">
				<xsl:call-template name="Href">
					<xsl:with-param name="album" select="." />
				</xsl:call-template>
				<span class="img_wrap1">
					<span class="img_wrap2" style="background-image: url(https://graph.facebook.com/{@Cover_Photo}/picture?type=album&amp;access_token={$access_token})"></span>
				</span>
			</a>
			<div class="album_title">
				<a title="{@Name}">
					<xsl:call-template name="Href">
						<xsl:with-param name="album" select="." />
					</xsl:call-template>
					<xsl:value-of select="substring(@Name, 0, 25)" />
					<xsl:if test="string-length(@Name) &gt; 25">...</xsl:if>
				</a>
			</div>
			<div class="album_items_count">
				<xsl:value-of select="@Count" />&#160;items
			</div>
		</div>
	</xsl:template>
	<xsl:template match="Error">
		<span class="error">
			<xsl:value-of select="@Message" />
		</span>
	</xsl:template>
	<xsl:template name="Href">
		<xsl:param name="album" />
		<xsl:attribute name="href">
			<xsl:choose>
				<xsl:when test="$showFacebookLinks = 'true'">
					<xsl:value-of select="$album/@Link" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="concat('?a=',$album/@Id)" />
					<xsl:value-of select="concat('&amp;id=',$id)" />
					<xsl:value-of select="concat('#a',substring($album/@Id,0,3))" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:attribute>
	</xsl:template>
</xsl:stylesheet>