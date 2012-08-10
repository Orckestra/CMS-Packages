<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns:rendering="http://www.composite.net/ns/rendering/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
	<xsl:template match="/">
		<html>
			<head>
				<title>
					<xsl:choose>
						<xsl:when test="/in:inputs/in:param[@name='CustomTitle']/node()">
							<xsl:copy-of select="/in:inputs/in:param[@name='CustomTitle']/node()" />
						</xsl:when>
						<xsl:otherwise>
							<rendering:page.title />
						</xsl:otherwise>
					</xsl:choose>
				</title>
				<rendering:page.metatag.description />
				<f:function name="Composite.Web.Html.Template.CommonMetaTags">
					<f:param name="Designer" value="Composite C1 starter site" />
				</f:function>
				<!-- The 1140px Grid -->
				<link rel="stylesheet" href="~/Frontend/Styles/Core/1140.css" type="text/css" media="screen" />
				<xsl:comment><![CDATA[[if lte IE 9]><link rel="stylesheet" href="/Frontend/Styles/Core/ie.css" type="text/css" media="screen" /><![endif]]]></xsl:comment>
				<!-- Put your styles here -->
				<link id="skinstyles" rel="stylesheet" href="~/Frontend/Styles/styles.less" type="text/css" media="screen" />
			</head>
			<body>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>