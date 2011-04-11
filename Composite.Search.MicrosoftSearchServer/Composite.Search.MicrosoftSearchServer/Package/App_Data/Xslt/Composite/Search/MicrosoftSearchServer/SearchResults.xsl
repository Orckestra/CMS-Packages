<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:msr="urn:Microsoft.Search.Response"
	xmlns:msrd="urn:Microsoft.Search.Response.Document"
	xmlns:msrdd="urn:Microsoft.Search.Response.Document.Document"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:user="urn:my-scripts"
	xmlns:mp="#MarkupParserExtensions"
	exclude-result-prefixes="xsl in msr msrd msrdd msxsl user mp">

	<msxsl:script language="C#" implements-prefix="user">
		<msxsl:assembly name="System.Core" />
		<msxsl:assembly name="System.Web" />
		<msxsl:using namespace="System.Web" />
		<msxsl:assembly name="System.Xml.Linq" />
		<msxsl:using namespace="System.Linq" />
		<msxsl:using namespace="System.Xml.Linq" />
		<msxsl:using namespace="System.Collections.Generic" />

		<![CDATA[
			public  XPathNodeIterator GetHighLighted(string text)
			{
				text = text.Replace("<ddd/>", "…");

				for (int i = 0; i < 9; i++)
				{
					text = text.Replace("<c" + i + ">", "<strong>");
					text = text.Replace("</c" + i + ">", "</strong>");
				}
				
				XElement xml = XElement.Parse("<span xmlns=\"http://www.w3.org/1999/xhtml\">" + text + "</span>");
				
				return xml.CreateNavigator().Select(".");
			}

			public int GetStart(int pageSize)
			{
				int result = 1;
				if(System.Web.HttpContext.Current != null)
				{
					int pageCurrent = Convert.ToInt32(System.Web.HttpContext.Current.Request.QueryString["Page"]);
					
					if(pageCurrent > 0)
					{
						result = (pageCurrent - 1)*pageSize + 1;
					}
				}
				return result;
			}

			public string FixUrl(string url)
			{
				url = System.Text.RegularExpressions.Regex.Replace(url, @"(?<=\?.*)\?$", @"%3f");	
				return url;
			}

		]]>
	</msxsl:script>

	<xsl:param name="pageId" select="/in:inputs/in:result[@name='GetPageId']" />
	<xsl:param name="pageSize" select="/in:inputs/in:param[@name='PageSize']" />
	<xsl:param name="searchQuery" select="/in:inputs/in:param[@name='SearchQuery']" />

	<xsl:template match="/">
		<html>
			<head>
				<title>
					Results for "<xsl:value-of select="/in:inputs/in:param[@name='SearchQuery']" />"
				</title>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Search/MicrosoftSearchServer/Styles.css" />
			</head>
			<body>
				<xsl:apply-templates select="/in:inputs/in:result[@name='GetSearchResult']/msr:ResponsePacket/msr:Response/msr:Range" />
				<xsl:if test ="count(/in:inputs/in:result[@name='GetSearchResult']/msr:ResponsePacket/msr:Response/msr:Range) = 0">
					Your search - "<xsl:value-of select="/in:inputs/in:param[@name='SearchQuery']" />" - did not match any documents.
				</xsl:if>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="msr:Range">
		<div id="SearchResults">
			<p>
				Found <xsl:value-of select="msr:TotalAvailable" /> results for "<xsl:value-of select="$searchQuery" />"
			</p>
			<xsl:if test="count(msr:Results/msrd:Document)">
				<ol id="Results" start="{user:GetStart($pageSize)}">
					<xsl:apply-templates select="msr:Results/msrd:Document" />
				</ol>
			</xsl:if>
			<xsl:if test="msr:Count &lt; msr:TotalAvailable">
				<div class="Paging">
					<xsl:apply-templates select="." mode="PagingInfo" />
				</div>
			</xsl:if>
		</div>
	</xsl:template>

	<xsl:template match="msrd:Document">
		<li>
			<a href="{user:FixUrl(msrd:Action/msrd:LinkUrl)}">
				<xsl:value-of select="msrdd:Properties/msrdd:Property[msrdd:Name='TITLE']/msrdd:Value" />
			</a>
			<span class="Summary">
				<xsl:copy-of select="user:GetHighLighted(msrdd:Properties/msrdd:Property[msrdd:Name='HITHIGHLIGHTEDSUMMARY']/msrdd:Value)" />
			</span>

			<span class="Link">
				<xsl:value-of select="user:FixUrl(msrdd:Properties/msrdd:Property[msrdd:Name='PATH']/msrdd:Value)" />
			</span>

			<xsl:call-template name="DisplaySize">
				<xsl:with-param name="size" select="msrdd:Properties/msrdd:Property[msrdd:Name='SIZE']/msrdd:Value" />
			</xsl:call-template>
		</li>
	</xsl:template>

	<xsl:template name="DisplaySize">
		<xsl:param name="size" />
		<xsl:if test="string-length($size) > 0">
			<span class="Size">
				<xsl:if test="number($size) > 0">
					-
					<xsl:choose>
						<xsl:when test="round($size div 1024) &lt; 1">
							<xsl:value-of select="$size" /> Bytes
						</xsl:when>

						<xsl:when test="round($size div (1024 *1024)) &lt; 1">
							<xsl:value-of select="round($size div 1024)" />KB
						</xsl:when>

						<xsl:otherwise>
							<xsl:value-of select="round($size div (1024 * 1024))" />MB
						</xsl:otherwise>
					</xsl:choose>
				</xsl:if>
			</span>
		</xsl:if>
	</xsl:template>

	<xsl:template match="*" mode="PagingInfo">
		<xsl:param name="page" select="1" />
		<xsl:variable name="pagePart">
			<xsl:if test="not($page = 1)">
				<xsl:text>&amp;Page=</xsl:text>
				<xsl:value-of select="$page" />
			</xsl:if>
		</xsl:variable>
		<xsl:if test="$page &lt; (msr:TotalAvailable div $pageSize  + 1)">
			<xsl:if test="msr:StartAt &lt; $page*$pageSize and msr:StartAt &gt;  ($page - 1)*$pageSize">
				<span>
					<xsl:value-of select="$page" />
				</span>
			</xsl:if>
			<xsl:if test="not(msr:StartAt &lt; $page*$pageSize and msr:StartAt &gt;  ($page - 1)*$pageSize)">
				<a href="~/Renderers/Page.aspx?SearchQuery={$searchQuery}{$pagePart}&amp;pageId={$pageId}">
					<xsl:value-of select="$page" />
				</a>
			</xsl:if>
			<xsl:apply-templates select="." mode="PagingInfo">
				<xsl:with-param name="page" select="$page+1" />
			</xsl:apply-templates>
		</xsl:if>
	</xsl:template>

</xsl:stylesheet>