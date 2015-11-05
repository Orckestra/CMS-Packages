<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
	<xsl:variable name="profileUrl" select="/in:inputs/in:param[@name='ProfileUrl']" />
	<xsl:variable name="btnStyle" select="/in:inputs/in:param[@name='ButtonStyle']" />
	<xsl:variable name="title" select="/in:inputs/in:param[@name='ButtonTitle']" />
	<xsl:template match="/">
		<html>
			<head>
				<!-- markup placed here will be shown in the head section of the rendered page -->
			</head>
			<body>
				<a href="{$profileUrl}" title="{$title}">
					<xsl:choose>
						<xsl:when test="$btnStyle = 'ViewMy_160x33'">
							<img src="http://www.linkedin.com/img/webpromo/btn_viewmy_160x33.png" width="160" height="33" border="0" alt="{$title}" />
						</xsl:when>
						<xsl:when test="$btnStyle = 'MyProfile_160x33'">
							<img src="http://www.linkedin.com/img/webpromo/btn_myprofile_160x33.png" width="160" height="33" border="0" alt="{$title}" />
						</xsl:when>
						<xsl:when test="$btnStyle = 'ViewMy_160x25'">
							<img src="http://www.linkedin.com/img/webpromo/btn_viewmy_160x25.png" width="160" height="25" border="0" alt="{$title}" />
						</xsl:when>
						<xsl:when test="$btnStyle = 'ViewMy_120x33'">
							<img src="http://www.linkedin.com/img/webpromo/btn_viewmy_120x33.png" width="120" height="33" border="0" alt="{$title}" />
						</xsl:when>
						<xsl:when test="$btnStyle = 'ProfileGrey_80x15'">
							<img src="http://www.linkedin.com/img/webpromo/btn_profile_greytxt_80x15.png" width="80" height="15" border="0" alt="{$title}" />
						</xsl:when>
						<xsl:when test="$btnStyle = 'ProfileBlue_80x15'">
							<img src="http://www.linkedin.com/img/webpromo/btn_profile_bluetxt_80x15.png" width="80" height="15" border="0" alt="{$title}" />
						</xsl:when>
						<xsl:when test="$btnStyle = 'In_20x15'">
							<img src="http://www.linkedin.com/img/webpromo/btn_in_20x15.png" width="20" height="15" alt="{$title}" style="vertical-align:middle" border="0" />
						</xsl:when>
					</xsl:choose>
				</a>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>