<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns="http://www.w3.org/1999/xhtml" xmlns:f="http://www.composite.net/ns/function/1.0" exclude-result-prefixes="xsl in f">
	<xsl:variable name="commentsServiceKey" select="/in:inputs/in:param[@name='CommentsServiceKey']" />
	<xsl:template match="/">
		<html>
			<head />
			<body>
				<xsl:choose>
					<xsl:when test="string-length($commentsServiceKey) &gt; 1">
						<div id="disqus_thread"></div>
						<script type="text/javascript">
							var disqus_shortname = '<xsl:value-of select="$commentsServiceKey"/>'; // required: replace example with your forum shortname
							/* * * DON'T EDIT BELOW THIS LINE * * */
							(function() {
							var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;
							dsq.src = 'http://' + disqus_shortname + '.disqus.com/embed.js';
							(document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
							})();
						</script>
						<noscript>
							Please enable JavaScript to view the <a href="http://disqus.com/?ref_noscript">comments powered by Disqus.</a>
						</noscript>
						<a href="http://disqus.com" class="dsq-brlink">
							comments powered by <span class="logo-disqus">Disqus</span>
						</a>
					</xsl:when>
					<xsl:otherwise>
						A short guide on how to register with disqus and how to update the primary blog function with SiteShortName
					</xsl:otherwise>
				</xsl:choose>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>