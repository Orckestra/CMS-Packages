<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="xsl in lang f">

	<xsl:param name="siteShortname" select="/in:inputs/in:param[@name='SiteShortname']" />
	<xsl:param name="mode" select="/in:inputs/in:param[@name='Mode']" />
	<xsl:param name="items" select="/in:inputs/in:param[@name='Items']" />
	<xsl:param name="moderatorsRanking" select="/in:inputs/in:param[@name='ModeratorsRanking']" />
	<xsl:param name="colorTheme" select="/in:inputs/in:param[@name='ColorTheme']" />
	<xsl:param name="defaultTabView" select="/in:inputs/in:param[@name='DefaultTabView']" />
	<xsl:param name="excerptLength" select="/in:inputs/in:param[@name='ExcerptLength']" />
	<xsl:param name="showAvatars" select="/in:inputs/in:param[@name='ShowAvatars']" />
	<xsl:param name="avatarSize" select="/in:inputs/in:param[@name='AvatarSize']" />
	
	<xsl:template match="/">
		<html>
			<head />
			<body>
				<xsl:choose>
					<xsl:when test="$mode='Combination'">
						<script type="text/javascript" src="http://{$siteShortname}.disqus.com/combination_widget.js?num_items={$items}&amp;hide_mods={$moderatorsRanking}&amp;color={$colorTheme}&amp;default_tab={$defaultTabView}&amp;excerpt_length={$excerptLength}"></script>
					</xsl:when>
					<xsl:when test="$mode='Popular'">
						<div id="popularthreads" class="dsq-widget">
							<h2 class="dsq-widget-title">Popular Threads</h2>
							<script type="text/javascript" src="http://{$siteShortname}.disqus.com/popular_threads_widget.js?num_items={$items}"></script>
						</div>
					</xsl:when>
					<xsl:when test="$mode='Top'">
						<div id="topcommenters" class="dsq-widget">
							<h2 class="dsq-widget-title">Top Commenters</h2>
							<script type="text/javascript" src="http://{$siteShortname}.disqus.com/top_commenters_widget.js?num_items={$items}&amp;hide_mods={$moderatorsRanking}&amp;hide_avatars={$showAvatars}&amp;avatar_size={$avatarSize}"></script>
						</div>
					</xsl:when>
					<xsl:otherwise>
						<div id="recentcomments" class="dsq-widget">
							<h2 class="dsq-widget-title">Recent Comments</h2>
							<script type="text/javascript" src="http://{$siteShortname}.disqus.com/recent_comments_widget.js?num_items={$items}&amp;hide_avatars={$showAvatars}&amp;avatar_size={$avatarSize}&amp;excerpt_length={$excerptLength}"></script>
						</div>
					</xsl:otherwise>
				</xsl:choose>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>
