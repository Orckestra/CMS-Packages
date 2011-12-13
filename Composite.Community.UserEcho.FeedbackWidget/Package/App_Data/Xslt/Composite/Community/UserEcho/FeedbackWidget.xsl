<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
xmlns:lang="http://www.composite.net/ns/localization/1.0"
xmlns:f="http://www.composite.net/ns/function/1.0"
xmlns="http://www.w3.org/1999/xhtml"
xmlns:csharp="http://c1.composite.net/sample/csharp"
exclude-result-prefixes="xsl in lang f csharp">
	<xsl:param name="setup" select="/in:inputs/in:result[@name='GetUserEchoSetupXml']/UserEchoSetup" />
	<xsl:variable name="host" select="/in:inputs/in:param[@name='URL']" />
	<xsl:variable name="forum" select="/in:inputs/in:param[@name='ForumID']" />
	<xsl:variable name="widgetType" select="/in:inputs/in:param[@name='WidgetType']" />
	<xsl:variable name="openForumByClick" select="/in:inputs/in:param[@name='OpenForumByClick']" />
	<xsl:variable name="linkText" select="/in:inputs/in:param[@name='LinkText']" />
	<xsl:variable name="linkTextEncoded" select="csharp:EncodeToBase64($linkText)" />
	<xsl:variable name="textColor" select="/in:inputs/in:param[@name='TextColor']" />
	<xsl:variable name="fontSize" select="/in:inputs/in:param[@name='FontSize']" />
	<xsl:variable name="hoverColor" select="/in:inputs/in:param[@name='HoverColor']" />
	<xsl:variable name="bgColor" select="/in:inputs/in:param[@name='BackgroundColor']" />
	<xsl:variable name="aligment" select="/in:inputs/in:param[@name='Alignment']" />
	<xsl:variable name="showIcon" select="/in:inputs/in:param[@name='ShowIcon']" />
	<xsl:variable name="cornerRadius" select="/in:inputs/in:param[@name='CornerRadius']" />
	<xsl:variable name="currentCulture" select="/in:inputs/in:result[@name='CurrentCulture']" />
	<xsl:variable name="lang">
		<xsl:choose>
			<xsl:when test="/in:inputs/in:param[@name='Language'] = ' '">
				<xsl:value-of select="substring-before($currentCulture, '-')" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="/in:inputs/in:param[@name='Language']" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	<xsl:template match="/">
		<html>
			<head></head>
			<body>
				<xsl:if test="contains($widgetType,'link')">
					<xsl:choose>
						<xsl:when test="$openForumByClick = 'true'">
							<a onmouseover="UE.Popin.preload();" href="http://{$host}" target="_blank" >
								<xsl:value-of select="$linkText" />
							</a>
						</xsl:when>
						<xsl:otherwise>
							<a onmouseover="UE.Popin.preload();" href="#" onclick="UE.Popin.show(); return false;">
								<xsl:value-of select="$linkText" />
							</a>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:if>
				<script type="text/javascript">
					var _ues = {
					host:'<xsl:value-of select="$host" />',
					forum:'<xsl:value-of select="$forum" />',
					lang:'<xsl:value-of select="$lang" />',
					no_dialog: <xsl:value-of select="$openForumByClick" />,
					<xsl:choose>
						<xsl:when test="contains($widgetType,'tab')">
							tab_corner_radius:<xsl:value-of select="$cornerRadius" />,
							tab_font_size: <xsl:value-of select="$fontSize" />,
							tab_icon_show: <xsl:value-of select="$showIcon" />,
							tab_image_hash: encodeURIComponent('<xsl:value-of select="$linkTextEncoded" />'),
							tab_alignment:'<xsl:value-of select="$aligment" />',
							tab_text_color:'<xsl:value-of select="$textColor" />',
							tab_bg_color:'<xsl:value-of select="$bgColor" />',
							tab_hover_color:'<xsl:value-of select="$hoverColor" />',
							tab_top: '220px'
						</xsl:when>
						<xsl:otherwise>
							tab_show: false
						</xsl:otherwise>
					</xsl:choose>
					};
					(function() {
					var _ue = document.createElement('script'); _ue.type = 'text/javascript'; _ue.async = true;
					_ue.src = ('https:' == document.location.protocol ? 'https://s3.amazonaws.com/' : 'http://') + 'cdn.userecho.com/js/widget-1.4.gz.js';
					var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(_ue, s);
					})();
				</script>
			</body>
		</html>
	</xsl:template>
	<msxsl:script implements-prefix="csharp" language="C#" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:csharp="http://c1.composite.net/sample/csharp">
		public static string  EncodeToBase64(string input)
		{
		return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(input));
		}
	</msxsl:script>
</xsl:stylesheet>