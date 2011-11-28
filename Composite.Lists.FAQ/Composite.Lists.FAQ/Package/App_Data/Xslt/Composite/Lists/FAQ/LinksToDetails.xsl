<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" xmlns:mp="#MarkupParserExtensions" exclude-result-prefixes="xsl in lang f mp">
	<xsl:variable name="q" select="/in:inputs/in:result[@name='SelectedQuestion']" />
	<xsl:variable name="style" select="/in:inputs/in:param[@name='FAQStyle']" />
	<xsl:variable name="faq" select="/in:inputs/in:result[@name='GetFAQXml']" />
	<xsl:key name="by-Category" match="FAQ" use="@QuestionCategory.Name" />
	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Lists/FAQ/Styles.css" />
			</head>
			<body>
				<div class="FAQ">
				<xsl:choose>
					<xsl:when test="$q/text() and $style='Links'">
						<xsl:apply-templates select="$faq/FAQ[@QuestionHeading=$q]" mode="Answer" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:for-each select="$faq/FAQ[generate-id(.)=generate-id(key('by-Category',@QuestionCategory.Name))]/@QuestionCategory.Name">
							<xsl:sort />
							<h3 class="category">
								<xsl:value-of select="." />
							</h3>
							<ul class="questions">
								<xsl:for-each select="key('by-Category',.)">
									<xsl:sort select="@DateAdded" order="ascending" />
									<li>
										<a>
										    <xsl:call-template name="Href">
												<xsl:with-param name="ref" select="@QuestionHeading" />
											</xsl:call-template>
											<xsl:value-of select="@QuestionHeading" />
										</a>
									</li>
								</xsl:for-each>
							</ul>
						</xsl:for-each>
						<xsl:if test="$style='Anchors'">
						  <xsl:apply-templates select="$faq/FAQ" mode="Answer" />
						</xsl:if>
					</xsl:otherwise>
				</xsl:choose>
				</div>
			</body>
		</html>
	</xsl:template>
	<xsl:template name="Href">
		<xsl:param name="ref" />
		<xsl:attribute name="href">
			<xsl:choose>
				<xsl:when test="$style='Links'">?q=<xsl:value-of select="$ref"/></xsl:when>
				<xsl:when test="$style='Anchors'">#<xsl:value-of select="$ref"/></xsl:when>
			</xsl:choose>
		</xsl:attribute>
	</xsl:template>
	<xsl:template match="FAQ" mode="Answer">
		<div class="answer">
		<a id="{@QuestionHeading}" name="{@QuestionHeading}" />
		<h1>
			<xsl:value-of select="@QuestionHeading" />
		</h1>
		<div class="question">
			<xsl:copy-of select="mp:ParseXhtmlBodyFragment(@QuestionDetails)" />
		</div>
		<b>Answer:</b>
		<div class="answer">
			<xsl:copy-of select="mp:ParseXhtmlBodyFragment(@Answer)" />
		</div>
		<xsl:text>◄</xsl:text>
		<a href="~/page({@PageId})" rel="previous">Back to FAQ</a>
		</div>
		<hr />
	</xsl:template>
</xsl:stylesheet>