<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="xsl in df mp"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:df="#dateExtensions"
	xmlns:mp="#MarkupParserExtensions"
	xmlns:csharp="http://c1.composite.net/maw/csharp"
	xmlns:temp="urn:TEMP"
>

	<!-- the questystring -->
	<xsl:variable name="q" select="/in:inputs/in:result[@name='QueryStringValue']" />

	<!-- shortcut main data bulks -->
	<xsl:variable name="faqtarget" select="/in:inputs/in:param[@name='FAQTarget']/text()" />
	<xsl:variable name="faqtypes" select="/in:inputs/in:result[@name='GetFAQTypeXml']" />
	<xsl:variable name="faqquestions" select="/in:inputs/in:result[@name='GetFAQXml']" />
	<xsl:variable name="pageId" select="/in:inputs/in:result[@name='GetPageId']" />

	<!-- sort questions categorized -->
	<xsl:variable name="faqcategorized">
		<xsl:for-each select="$faqtypes/FAQType">
			<xsl:variable name="id" select="@Id" />
			<xsl:if test="$faqquestions/FAQ[@QuestionType=$id and @QuestionTarget=$faqtarget]">
				<temp:Category Name="{@Name}">
					<xsl:for-each select="$faqquestions/FAQ[@QuestionType=$id and @QuestionTarget=$faqtarget]">
						<xsl:copy-of select="current()" />
					</xsl:for-each>
				</temp:Category>
			</xsl:if>
		</xsl:for-each>
	</xsl:variable>

	<!-- out -->
	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Lists/FAQ/Styles.css" />
				<xsl:if test="$q/text()">
					<title>
						<xsl:value-of select="$q" />
					</title>
				</xsl:if>
			</head>
			<body>
				<div class="FAQ">
					<xsl:choose>
						<xsl:when test="$q/text()">
							<xsl:call-template name="outputanswer">
								<xsl:with-param name="q">
									<xsl:value-of select="q" />
								</xsl:with-param>
							</xsl:call-template>
						</xsl:when>
						<xsl:otherwise>
							<xsl:call-template name="outputquestions" />
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</body>
		</html>
	</xsl:template>

	<!-- OUTPUT QUESTIONS -->

	<xsl:template name="outputquestions">
		<xsl:variable name="categories" select="msxsl:node-set($faqcategorized)" />
		<xsl:apply-templates select="$categories/temp:Category">
			<xsl:sort select="@Name" />
		</xsl:apply-templates>
	</xsl:template>

	<xsl:template match="temp:Category">
		<h4><xsl:value-of select="@Name" /></h4>
		<ul class="nav">
			<xsl:apply-templates select="FAQ">
				<xsl:sort select="@DateAdded" order="ascending" />
			</xsl:apply-templates>
		</ul>
	</xsl:template>

	<xsl:template match="temp:Category/FAQ">
		<li>
			<xsl:text>► </xsl:text>
			<a href="~/Renderers/Page.aspx?pageId={$pageId}&amp;q={csharp:UrlEncodeUnicode(@QuestionHeading)}">
				<xsl:value-of select="@QuestionHeading" />
			</a>
		</li>
	</xsl:template>

	<!-- OUTPUT ANSWER -->

	<xsl:template name="outputanswer">
		<xsl:apply-templates select="$faqquestions/FAQ[@QuestionHeading=$q]" />
	</xsl:template>

	<xsl:template match="in:result/FAQ">
		<h1><xsl:copy-of select="mp:ParseXhtmlBodyFragment(@QuestionHeading)" /></h1>
		<div class="question">
			<xsl:copy-of select="mp:ParseXhtmlBodyFragment(@QuestionDetails)" />
		</div>
		<h4>Answer:</h4>
		<div class="answer">
			<xsl:copy-of select="mp:ParseXhtmlBodyFragment(@Answer)" />
		</div>
		<ul class="nav">
			<li>
				<xsl:text>◄ </xsl:text>
				<a href="~/Renderers/Page.aspx?pageId={$pageId}" rel="previous">Back to FAQ</a>
			</li>
		</ul>
	</xsl:template>

	<msxsl:script implements-prefix="csharp" language="C#">
		<msxsl:assembly name="System.Web" />
		<msxsl:using namespace="System.Web" />

		<![CDATA[
			public String UrlEncodeUnicode( string source )
			{
				return HttpUtility.UrlEncodeUnicode( source );
			}
		]]>
	</msxsl:script>

</xsl:stylesheet>
