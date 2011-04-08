<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="xsl in lang f">

	<xsl:param name="username" select="/in:inputs/in:param[@name='Username']" />
	<xsl:param name="tweetsCount" select="/in:inputs/in:param[@name='TweetsCount']" />
	<xsl:param name="loaderText" select="/in:inputs/in:param[@name='LoaderText']" />
	<xsl:param name="slideIn" select="/in:inputs/in:param[@name='SlideIn']" />
	<xsl:param name="slideDuration" select="/in:inputs/in:param[@name='SlideDuration']" />
	<xsl:param name="showHeading" select="/in:inputs/in:param[@name='ShowHeading']" />
	<xsl:param name="headingText" select="/in:inputs/in:param[@name='HeadingText']" />
	<xsl:param name="showProfileLink" select="/in:inputs/in:param[@name='ShowProfileLink']" />
	<xsl:param name="showTimestamp" select="/in:inputs/in:param[@name='ShowTimestamp']" />

	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Feeds/TwitterReader/Styles.css" />
				<script src="http://ajax.microsoft.com/ajax/jquery/jquery-1.4.2.min.js" type="text/javascript" id="jquery-1-4-2"></script>
				<script type="text/javascript" src="~/Frontend/Composite/Feeds/TwitterReader/Scripts/jquery.twitter.js"></script>
				<script type="text/javascript">
					$(document).ready(function() {
					$("#<xsl:value-of select="/in:inputs/in:param[@name='TwitterReaderId']" />").getTwitter({
					userName: "<xsl:value-of select="$username" />",
					numTweets: <xsl:value-of select="$tweetsCount" />,
					loaderText: "<xsl:value-of select="$loaderText" />",
					slideIn: <xsl:value-of select="$slideIn" />,
					slideDuration: <xsl:value-of select="$slideDuration" />,
					showHeading: <xsl:value-of select="$showHeading" />,
					headingText: "<xsl:value-of select="$headingText" />",
					showProfileLink: <xsl:value-of select="$showProfileLink" />,
					showTimestamp: <xsl:value-of select="$showTimestamp" />
					});
					});
					function twitterCallback<xsl:value-of select="/in:inputs/in:param[@name='TwitterReaderId']" />(twitters)
					{
					twitterCallback2(twitters, '<xsl:value-of select="/in:inputs/in:param[@name='TwitterReaderId']" />');
					}
				</script>
			</head>

			<body>
				<div id="{/in:inputs/in:param[@name='TwitterReaderId']}" />
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>