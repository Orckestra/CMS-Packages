<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns:parser="#MarkupParserExtensions"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:csharp="urn:CSharp"
	xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="xsl in lang f csharp msxsl parser">

	<xsl:variable name="suggestionElement" select="/in:inputs/in:result[@name='Google']/GSP/Spelling/Suggestion" />
	<xsl:variable name="resultsElement" select="/in:inputs/in:result[@name='Google']/GSP/RES" />
	<xsl:variable name="query" select="/in:inputs/in:param[@name='SearchTerm']" />
	<xsl:variable name="page" select="/in:inputs/in:result[@name='Page']" />
	<xsl:variable name="pageId" select="/in:inputs/in:result[@name='GetPageId']" />

	<xsl:template match="/">
		<html>
			<head>
				<xsl:if test="$query!=''">
					<title>
						<xsl:value-of select="csharp:Format(/in:inputs/in:param[@name='TextTitle'], $query)" />
					</title>
				</xsl:if>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Search/GoogleSiteSearch/Styles.css" />
			</head>
			<body>
				<form action="~/Renderers/Page.aspx?PageId={/in:inputs/in:result[@name='GetPageId']}" >
					<input type="text" size="40" name="q" value="{/in:inputs/in:param[@name='SearchTerm']}" />
					<input type="submit" value="{/in:inputs/in:param[@name='TextButtonSearch']}" />
				</form>
				<xsl:if test="$query!=''">
					<h1>
						<xsl:choose>
							<xsl:when test="$resultsElement">
								<xsl:copy-of select="parser:ParseXhtmlBodyFragment(csharp:Format(/in:inputs/in:param[@name='TextResults'],$resultsElement/M,$query))" />
							</xsl:when>
							<xsl:otherwise>
								<xsl:copy-of select="parser:ParseXhtmlBodyFragment(csharp:Format(/in:inputs/in:param[@name='TextNoResults'],$query))" />
							</xsl:otherwise>
						</xsl:choose>
					</h1>
					<xsl:apply-templates select="$suggestionElement" />
					<xsl:apply-templates select="$resultsElement" />
				</xsl:if>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="Suggestion">
		<p>
			<a href="~/Renderers/Page.aspx?PageId={$pageId}&amp;q={@q}">
				<xsl:value-of select="parser:ParseXhtmlBodyFragment(csharp:Format(/in:inputs/in:param[@name='TextSuggestion'],@q))" />
			</a>
		</p>
	</xsl:template>

	<xsl:template match="RES">
		<div id="results">
			<ul>
				<xsl:apply-templates select="R"/>
			</ul>
			<xsl:if test="NB/NU">
				<a href="~/Renderers/Page.aspx?PageId={$pageId}&amp;q={$query}&amp;Page={$page + 1}">
					<xsl:value-of select="/in:inputs/in:param[@name='TextButtonNext']" />
				</a>
			</xsl:if>
		</div>
	</xsl:template>

	<xsl:template match="R">
		<li>
			<a href="{U}" >
				<span class="title" title="{U}">
					<xsl:copy-of select="parser:ParseXhtmlBodyFragment(csharp:FixGooHtml(T))" />
				</span>
				<br />
				<span class="snippet">
					<xsl:value-of select="parser:ParseXhtmlBodyFragment(csharp:FixGooHtml(S))" />
				</span>
			</a>
		</li>
	</xsl:template>

	<msxsl:script language="C#" implements-prefix="csharp">
	<![CDATA[
		public string Format(string format, string arg0, string arg1)
		{
		return string.Format(format, arg0, arg1);
		}

		public string Format(string format, string arg0)
		{
		return string.Format(format, arg0);
		}

		public string FixGooHtml(string googleHtml)
		{
		return googleHtml.Replace("b>","strong>").Replace("<br>","").Replace("&middot;","-");
		}
	]]>
	</msxsl:script>

</xsl:stylesheet>
