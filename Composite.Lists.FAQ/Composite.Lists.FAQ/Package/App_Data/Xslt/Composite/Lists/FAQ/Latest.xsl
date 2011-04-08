<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="xsl in msxsl csharp mp"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:csharp="http://c1.composite.net/sample/csharp"
	xmlns:xhtml="http://www.w3.org/1999/xhtml"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:mp="#MarkupParserExtensions"
>

	<xsl:template match="/">
		<html>
			<head><link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Lists/FAQ/Styles.css" /></head>
			<body>
				<xsl:variable name="MaxItems" select="/in:inputs/in:param[@name='MaxItems']" />
				<xsl:variable name="TargetElements" select="/in:inputs/in:result[@name='GetFAQTargetXml']/FAQTarget" />
				<xsl:variable name="Types" select="/in:inputs/in:result[@name='GetFAQTypeXml']/FAQType" />
				<xsl:variable name="Faq" select="/in:inputs/in:result[@name='GetFAQXml']/FAQ[position()&gt;last()-$MaxItems]" />

				<div class="FAQ">
					<xsl:for-each select="$Faq">
					<xsl:sort select="@DateAdded" order="descending" />
						<xsl:variable name="TargetElement" select="$TargetElements[@Id=current()/@QuestionTarget]" />
						<xsl:variable name="Type" select="$Types[@Id=current()/@QuestionType]" />
						<xsl:variable name="PresentationPage" select="$TargetElement/@PresentationPage" />
						<xsl:variable name="PresentationUrl" select="concat('~/Renderers/Page.aspx?pageId=',$PresentationPage,'&amp;q=',csharp:urlEncode(@QuestionHeading) )" />
						<h4>
							<a href="{$PresentationUrl}">
								<span>
									<xsl:value-of select="$Type/@Name" />
								</span> :
								<xsl:value-of select="@QuestionHeading" />
							</a>
						</h4>
						<xsl:choose>
						<xsl:when test="@QuestionDetails">
							<xsl:copy-of select="mp:ParseXhtmlBodyFragment(@QuestionDetails)" />
						</xsl:when>
						<xsl:otherwise>
							<p/>
						</xsl:otherwise>
						</xsl:choose>
					<ul class="nav">
						<li><a href="{$PresentationUrl}">Read more</a> ►</li>
					</ul>
				</xsl:for-each>
				</div>
			</body>
		</html>
	</xsl:template>

	<msxsl:script implements-prefix="csharp" language="C#">
		<msxsl:assembly name="System.Web"/>
		<msxsl:using namespace="System.Web"/>
		<![CDATA[
			public string urlEncode(string str){
				if (str!=null)
					return HttpUtility.UrlEncodeUnicode(str);
				else
					return "";
			}
		]]>
	</msxsl:script>

</xsl:stylesheet>
