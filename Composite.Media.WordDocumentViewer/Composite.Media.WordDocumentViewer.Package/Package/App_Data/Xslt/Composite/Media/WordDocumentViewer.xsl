<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="xsl in" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns="http://www.w3.org/1999/xhtml" xmlns:x="http://www.w3.org/1999/xhtml" xmlns:f="http://www.composite.net/ns/function/1.0">
	<xsl:param name="downloadMedia" select="/in:inputs/in:param[@name='DownloadMedia']" />
	<xsl:param name="mediaFile" select="/in:inputs/in:param[@name='MediaFile']" />
	<xsl:template match="/">
		<html>
			<head>
				<style type="text/css" media="all">
<xsl:comment>

.Navigation {
	font-size: 8pt;
}

.Navigation .Prev {
	vertical-align: middle;
	font-style: italic;
	text-align: right;
	border-width: 0px;
}
.Navigation .Top {
	vertical-align: middle;
	font-weight: bold;
	border-width: 0px;
	border-style: dotted;
	border-left-width: 1px;
	border-right-width: 1px;
	border-color: #CC071E;
	text-align: center;
}
.Navigation .Next {
	vertical-align: middle;
	font-style: italic;
	text-align: left;
	border-width: 0px;
}


.TOC0 {
	padding-left: 0px;
}

.TOC1 {
	padding-left: 20px;
}

.TOC2 {
	padding-left: 40px;
}

.TOC3 {
	padding-left: 60px;
}


</xsl:comment>
				</style>
			</head>
			<body>
				<div id="WordDocumentViewer">
					<xsl:if test="$downloadMedia='true'">
						<div id="Download"><a href="~/Renderers/ShowMedia.ashx?i={$mediaFile}">Download document</a></div>
					</xsl:if>
					<xsl:apply-templates select="/in:inputs/in:result[@name='Render']/x:html" />
				</div>
			</body>
		</html>
	</xsl:template>
	
	<xsl:template match="x:head">
		<head />
	</xsl:template>
	
	<xsl:template match="x:p[@class='Title']">
		<!--
			<h1><xsl:apply-templates select="node()" /></h1>
		-->
	</xsl:template>

	<xsl:template match="x:p[@class='StyleHeading1Left0Firstline0' or @class='Heading1']">
		<h4><xsl:apply-templates select="node()" /></h4>
	</xsl:template>

	<xsl:template match="x:p[@class='Heading2']">
		<p><strong><xsl:apply-templates select="node()" /></strong></p>
	</xsl:template>
	
	<xsl:template match="x:p[@class='Heading3']">
		<p><strong><xsl:apply-templates select="node()" /></strong></p>
	</xsl:template>

	<xsl:template match="x:p[@class='Heading4']">
		<p><strong><xsl:apply-templates select="node()" /></strong></p>
	</xsl:template>

	<xsl:template match="x:p[@class='BilledtekstComposite']">
		<p><sup><xsl:apply-templates select="node()" /></sup></p>
	</xsl:template>
	
	<xsl:template match="x:p[@class='TOC1' or @class='TOC2' or @class='TOC3' or @class='TOC4']">
		<xsl:apply-templates select="node()" />
		<br />
	</xsl:template>
	
	<xsl:template match="x:a[@class='TOC0' or @class='TOC1' or @class='TOC2' or @class='TOC3']">
		<xsl:copy-of select="." />
	</xsl:template>

	<xsl:template match="x:div[@class='Navigation']">
			<table width="100%" class="Navigation">
			<tr>
				<td width="45%" class="Prev">

					<a href="{node()[1]/@href}">
						<strong>
						&#171;&#160;</strong>
						<xsl:value-of select="substring(node()[1]/text(),1,32)" />
						<xsl:if test="string-length(node()[1]/text()) > 32">...</xsl:if>
					</a>
				</td>
				<td class="Top">
					<xsl:copy-of select="node()[2]" />
				</td>
				<td width="45%" class="Next">
					<!--<xsl:copy-of select="node()[3]" />-->
					<a href="{node()[3]/@href}">

						<xsl:value-of select="substring(node()[3]/text(),1,32)" />
						<xsl:if test="string-length(node()[3]/text()) > 32">...</xsl:if><strong>&#160;&#187;</strong>
					</a>
				</td>
			</tr>
		</table>
	</xsl:template>
	
	<xsl:template match="@style | @class">
		<!-- throw away ms word style and class goo -->
		<xsl:if test="string(.) = 'selected'">
			<xsl:copy />
		</xsl:if>
	</xsl:template>

	<xsl:template match="x:span">
		<!-- throw away ms word span goo -->
		<xsl:apply-templates select="node()" />
	</xsl:template>

	<xsl:template match="node() | @*">
		<xsl:copy>
			<xsl:apply-templates select="node() | @*" />
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>