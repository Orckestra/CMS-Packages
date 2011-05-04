<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="xsl in lang f">

	<xsl:param name="style" select="/in:inputs/in:param[@name='Style']" />
	<xsl:param name="profileId" select="/in:inputs/in:param[@name='ProfileId']" />

	<xsl:template match="/">
		<html>
			<head />
			<body>
				<xsl:variable name="pubId">
					<xsl:choose>
						<xsl:when test="string($profileId)">
							<xsl:value-of select="$profileId" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:value-of select="'xa-4dbaa23f4bdb7818'" />
						</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>
				<!-- AddThis Button BEGIN -->
				<xsl:choose>
					<xsl:when test="$style='SmallButtons'">
						<div class="addthis_toolbox addthis_default_style ">
							<a class="addthis_button_preferred_1"></a>
							<a class="addthis_button_preferred_2"></a>
							<a class="addthis_button_preferred_3"></a>
							<a class="addthis_button_preferred_4"></a>
							<a class="addthis_button_compact"></a>
							<a class="addthis_counter addthis_bubble_style"></a>
						</div>
					</xsl:when>
					<xsl:when test="$style='LargeButtons'">
						<div class="addthis_toolbox addthis_default_style addthis_32x32_style">
							<a class="addthis_button_preferred_1"></a>
							<a class="addthis_button_preferred_2"></a>
							<a class="addthis_button_preferred_3"></a>
							<a class="addthis_button_preferred_4"></a>
							<a class="addthis_button_compact"></a>
							<a class="addthis_counter addthis_bubble_style"></a>
						</div>
					</xsl:when>
					<xsl:when test="$style='SimpleWithButtons'">
						<div class="addthis_toolbox addthis_default_style ">
							<a href="http://www.addthis.com/bookmark.php?v=250&amp;pubid={$pubId}" class="addthis_button_compact">Share</a>
							<span class="addthis_separator">|</span>
							<a class="addthis_button_preferred_1"></a>
							<a class="addthis_button_preferred_2"></a>
							<a class="addthis_button_preferred_3"></a>
							<a class="addthis_button_preferred_4"></a>
						</div>
					</xsl:when>
					<xsl:when test="$style='SimpleNoCounter'">
						<div class="addthis_toolbox addthis_default_style ">
							<a href="http://www.addthis.com/bookmark.php?v=250&amp;pubid={$pubId}" class="addthis_button_compact">Share</a>
						</div>
					</xsl:when>
					<xsl:when test="$style='SimpleWithCounterAbove'">
						<div class="addthis_toolbox addthis_default_style ">
							<a class="addthis_counter"></a>
						</div>
					</xsl:when>
					<xsl:when test="$style='SimpleWithCounterBeside'">
						<div class="addthis_toolbox addthis_default_style ">
							<a class="addthis_counter addthis_pill_style"></a>
						</div>
					</xsl:when>
					<xsl:when test="$style='BarWithButtons'">
						<a class="addthis_button" href="http://www.addthis.com/bookmark.php?v=250&amp;pubid={$pubId}">
							<img src="http://s7.addthis.com/static/btn/v2/lg-share-en.gif" width="125" height="16" alt="Bookmark and Share" style="border:0" />
						</a>
					</xsl:when>
					<xsl:when test="$style='Bar'">
						<a class="addthis_button" href="http://www.addthis.com/bookmark.php?v=250&amp;pubid={$pubId}">
							<img src="http://s7.addthis.com/static/btn/sm-share-en.gif" width="83" height="16" alt="Bookmark and Share" style="border:0" />
						</a>
					</xsl:when>
					<xsl:otherwise>
						<div class="addthis_toolbox addthis_default_style ">
							<a class="addthis_button_facebook_like"></a>
							<a class="addthis_button_tweet"></a>
							<a class="addthis_counter addthis_pill_style"></a>
						</div>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:if test="string($profileId)">
					<script type="text/javascript">var addthis_config = {"data_track_clickback":true};</script>
				</xsl:if>
				<script type="text/javascript" src="http://s7.addthis.com/js/250/addthis_widget.js#pubid={$pubId}"></script>
				<!-- AddThis Button END -->
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>
