<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:atom="http://www.w3.org/2005/Atom"
	xmlns:twitter="http://api.twitter.com/"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:csharp="http://c1.composite.net/sample/csharp"
	xmlns:de="#dateExtensions"
	exclude-result-prefixes="xsl in lang f atom twitter de">

	<xsl:param name="feed" select="/in:inputs/in:result[@name='GetFeed']/atom:feed" />

	<msxsl:script implements-prefix="csharp" language="C#">
		<![CDATA[
		public static string FormatDate(string updated)
		{
			DateTime date;
			if(DateTime.TryParse(updated,out date))
			{
				if (date.AddMinutes(2) > DateTime.Now)
				{
					return "1 minute ago";
				}
				else if (date.AddHours(1) > DateTime.Now)
				{
					return string.Format("{0:0} minutes ago", (DateTime.Now - date).TotalMinutes);
				}
				else if (date.AddDays(1) > DateTime.Now)
				{
					return string.Format("about {0:0} hours ago", (DateTime.Now - date).TotalHours);
				}
				else if (date.AddDays(2) > DateTime.Now)
				{
					return "1 day ago";
				}
				else
				{
					return string.Format("{0:0} days ago", (DateTime.Now - date).TotalDays);
				}
			}
			return string.Empty;
		}
		]]>
	</msxsl:script>


	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Community/Twitter/FeedAggregator/Styles.css" />
			</head>
			<body>
				<div id="FeedAggregator">
					<ul style="border-top: 0pt none;">
						<xsl:for-each select="$feed/atom:entry" >
							<xsl:variable name="id" select="substring-after(atom:id,'tag:search.twitter.com,2005:')" />
							<xsl:variable name="author" select="substring-before(atom:author/atom:name,' (')" />
							<li class="result ">
								<div class="avatar">
									<a href="{atom:author/atom:uri}">
										<img src="{atom:link[@rel='image']/@href}" />
									</a>
								</div>
								<div class="msg">
									<a class="username" href="{atom:author/atom:uri}">
										<xsl:value-of select="$author" />
									</a>: <span class="msgtxt da">
										<xsl:copy-of select="atom:content[@type='html']/node()" />
									</span>
								</div>
								<div class="info">
									<xsl:value-of select="csharp:FormatDate(atom:updated)" />
									<span class="source">
										via <xsl:copy-of select="twitter:source/node()" />
									</span>
									·
									<a class="litnv" href="http://twitter.com/?status=@{$author}%20&amp;in_reply_to_status_id={$id}&amp;in_reply_to={$author}">Reply</a>
									· <a class="lit" href="{atom:link[@rel='alternate']/@href}">View Tweet</a>
								</div>
								<p class="clearleft"></p>
							</li>
						</xsl:for-each>
					</ul>
				</div>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>
