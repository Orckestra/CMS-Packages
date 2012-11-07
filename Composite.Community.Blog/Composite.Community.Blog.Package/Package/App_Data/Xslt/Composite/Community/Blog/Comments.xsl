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
						<p>
							To use the DISQUS service as your comment system in this blog, you should have a DISQUS account and <a href="http://disqus.com/admin/register/">register your website</a>.
						</p>
						<ol>
							<li>
								<a href="http://disqus.com/profile/login/">Log into your DISQUS account</a>
							</li>
							<li>
								Copy or make a note of the site shortname shown in the <a href="http://disqus.com/admin/settings/">"Settings"</a> under "Site Identity".
							</li>
							<li>Now log into the C1 Console of your website and edit the page with the "Composite.Community.Blog.BlogRenderer" function.</li>
							<li>In the function properties of "Composite.Community.Blog.BlogRenderer", select the "Comments Service Keys" parameter, switch to "Constant" and enter the site shortname.</li>
							<li>Click "OK" to save the changes, then save and publish the page.</li>
						</ol>
					</xsl:otherwise>
				</xsl:choose>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>