<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml" 
	xmlns:x="http://www.w3.org/1999/xhtml" 
	xmlns:date="#dateExtensions" 
	xmlns:parser="#MarkupParserExtensions" 
	xmlns:csharp="urn:CSHARP"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
	exclude-result-prefixes="xsl in lang f date parser csharp msxsl">
	
	<!-- these variables are defined on the "Function Calls" tab -->
	<xsl:variable name="news" select="/in:inputs/in:result[@name='GetNewsXml']"/>
	<xsl:variable name="story" select="/in:inputs/in:result[@name='QueryStringValue']"/>
	<xsl:variable name="id" select="/in:inputs/in:result[@name='GetPageId']"/>
	
	<!-- these variables are local to the stylesheet -->
	
	<!-- PageIDs can be found using the "Insert" > "Page URL" dropdown -->
	<xsl:variable name="newsid">398932b1-314b-4baf-b3f0-b1445f806aae</xsl:variable>
	<xsl:variable name="oldnewsid">fc84df32-5144-4602-9247-8f527f4bf15c</xsl:variable>
	
	<!-- display latest news or archived news? -->
	<xsl:variable name="isoldnews">
		<xsl:choose>
			<xsl:when test="$id=$oldnewsid">true</xsl:when>
			<xsl:otherwise>false</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
	
	<!-- output -->
	<xsl:template match="/">
		<html>
			<head>
				<!-- notice that Functions have access to the HEAD section -->
				<link rel="stylesheet" type="text/css" media="screen" href="~/Frontend/Styles/Functions/News.css"/>
			</head>
			<body>
				<xsl:choose>
					<xsl:when test="$story=''">
						<h1>
							<xsl:choose>
								<xsl:when test="$isoldnews='true'">Archived news</xsl:when>
								<xsl:otherwise>Latest news</xsl:otherwise>
							</xsl:choose>
						</h1>
						<ul id="news">
							<xsl:choose>
								<xsl:when test="$isoldnews='true'">
									<xsl:apply-templates select="$news/News[position()&gt;4]"/>
								</xsl:when>
								<xsl:otherwise>
									<xsl:apply-templates select="$news/News[position()&lt;=4]"/>
								</xsl:otherwise>
							</xsl:choose>
						</ul>
					</xsl:when>
					<xsl:otherwise>
						<xsl:call-template name="story"/>
					</xsl:otherwise>
				</xsl:choose>
			</body>
		</html>
	</xsl:template>
	
	<!-- output news list item -->
	<xsl:template match="News">
		<li>
			<a title="Read more">
				<xsl:attribute name="href">
					<xsl:call-template name="href"/>
				</xsl:attribute>
				<span>
					<xsl:call-template name="date"/>
					<xsl:text>: </xsl:text>
				</span>
				<strong>
					<xsl:value-of select="@Title"/>
				</strong>
				<br/>
				<span>
					<xsl:value-of select="@Summary"/>
				</span>
			</a>
		</li>
	</xsl:template>
	
	<!-- output full story (based on querystring) -->
	<xsl:template name="story">
		<xsl:variable name="title" select="substring-after($story,':')"/>
		<xsl:for-each select="$news/News[@Title=$title]">
			<xsl:variable name="date">
				<xsl:call-template name="date"/>
			</xsl:variable>
			<xsl:if test="$date=substring-before($story,':')">
				
				<!-- this will parse an attribute string into a document fragment -->
				<xsl:copy-of select="parser:ParseWellformedDocumentMarkup(@Content)/x:html/x:body/*"/>
				
				<ul class="links">
					<li>
						<a rel="back" href="?">
							<xsl:if test="$id=$newsid">Latest news</xsl:if>
							<xsl:if test="$id=$oldnewsid">News archive</xsl:if>
						</a>
					</li>
				</ul>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>
	
	<!-- compute nice URL -->
	<xsl:template name="href">
	
		<!-- when page is not placed in news section, output full URL path -->
		<xsl:choose>
			<xsl:when test="not($id=$newsid or $id=$oldnewsid)">
				<xsl:text>/Renderers/Page.aspx?pageId=</xsl:text>
				<xsl:value-of select="$newsid"/>
				<xsl:text>&amp;story=</xsl:text>
			</xsl:when>
			<xsl:otherwise>
				<xsl:text>?story=</xsl:text>
			</xsl:otherwise>
		</xsl:choose>
		
		<xsl:call-template name="date"/>
		<xsl:text>:</xsl:text>
		<xsl:value-of select="csharp:UrlEncodeUnicode(@Title)"/>
	</xsl:template>
	
	<!-- format nice date -->
	<xsl:template name="date">
		<xsl:value-of select="date:Year(@Date)"/>
		<xsl:text>.</xsl:text>
		<xsl:value-of select="date:Month(@Date)"/>
		<xsl:text>.</xsl:text>
		<xsl:value-of select="date:Day(@Date)"/>
	</xsl:template>
	
	<!-- inline C# to encode strings for use in URL -->
	<msxsl:script implements-prefix="csharp" language="C#">
		<msxsl:assembly name="System.Web"/>
		<msxsl:using namespace="System.Web"/>
		<![CDATA[
			public String UrlEncodeUnicode ( string source ) {
				return HttpUtility.UrlEncodeUnicode( source );
			}
		]]>
	</msxsl:script>

</xsl:stylesheet>
