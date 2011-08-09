<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
	<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Navigation/LanguageSwitcher/Styles.css" />
	<xsl:param name="showCurrent" select="/in:inputs/in:param[@name='ShowCurrent']" />
	<xsl:param name="navigationId" select="/in:inputs/in:param[@name='NavigationId']" />
	<xsl:param name="description" select="/in:inputs/in:param[@name='Description']" />
	<xsl:param name="links" select="/in:inputs/in:result[@name='GetPagesInfo']/*[$showCurrent='true' or $showCurrent != 'true' and @CurrentCulture != 'true']" />

	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Navigation/LanguageSwitcher/Styles.css" />
				<xsl:if test="$navigationId = 'LanguageSwitcherImage'">
					<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Navigation/LanguageSwitcher/Images.css" />
				</xsl:if>
			</head>
			<body>
				<div id="{$navigationId}">
					<xsl:if test="count($links) &gt; 0 and $description!=''">
						<span class="description">
							<xsl:value-of select="$description" />
						</span>
					</xsl:if>
					<xsl:for-each select="$links">
						<a href="{@URL}" class="{@Culture}">
							<xsl:attribute name="lang">
								<xsl:value-of select="substring-before(@Culture, '-')" />
							</xsl:attribute>
							<xsl:apply-templates select="node()" />
						</a>
					</xsl:for-each>
				</div>
			</body>
		</html>
	</xsl:template>
	<xsl:template match="*">
		<xsl:element name="{name()}">
			<xsl:apply-templates select="@* | node()" />
		</xsl:element>
	</xsl:template>
	<xsl:template match="@*">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>