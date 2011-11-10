<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:date="#dateExtensions" 
	exclude-result-prefixes="xsl in lang f date">
	
	<!-- these variables are inserted on the "Function Calls" tab -->
	<xsl:variable name="now" select="/in:inputs/in:result[@name='Now']"/>
	<xsl:variable name="root" select="/in:inputs/in:result[@name='SitemapXml']"/>
	<xsl:variable name="culture" select="/in:inputs/in:result[@name='CurrentCulture']"/>
	<xsl:variable name="cultures" select="/in:inputs/in:result[@name='GetForeignPageInfo']"/>
	
	<!-- output -->
	<xsl:template match="/">
		<html>
			<head/>
			<body>
				<p>
					<xsl:choose>
						<xsl:when test="$culture='da-DK'">Dags dato er </xsl:when>
						<xsl:otherwise>Today is </xsl:otherwise>
					</xsl:choose>
					<xsl:call-template name="date"/>
				</p>
				<ul>
					<li>
						<a href="javascript:window.print()">Print</a>
					</li>
				
					<!-- target pages with no MenuTitle! -->
					<xsl:apply-templates select="$root/Page/Page[not(@MenuTitle)]"/>
					
					<!-- offer alternative language (if any) -->
					<xsl:variable name="alt" select="$cultures/LanguageVersion[@Culture!=$culture]"/>
					<xsl:if test="$alt">
						<li>
							<strong>
								<a href="{$alt/@URL}" title="Change language">
									<xsl:choose>
										<xsl:when test="$culture='da-DK'">In English</xsl:when>
										<xsl:otherwise>In Danish</xsl:otherwise>
									</xsl:choose>
								</a>
								<xsl:text> &gt;</xsl:text>
							</strong>
						</li>
					</xsl:if>
				</ul>
			</body>
		</html>
	</xsl:template>
	
	<!-- formatted date -->
	<xsl:template name="date">
		<xsl:choose>
			<xsl:when test="$culture='da-DK'">
				<xsl:value-of select="date:Day($now)"/>
				<xsl:text>. </xsl:text>
				<xsl:value-of select="date:LongMonthName(date:Month($now))"/>
				<xsl:text>, </xsl:text>
				<xsl:value-of select="date:Year($now)"/>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="date:LongMonthName(date:Month($now))"/>
				<xsl:text> </xsl:text>
				<xsl:value-of select="date:Day($now)"/>
				<xsl:text>, </xsl:text>
				<xsl:value-of select="date:Year($now)"/>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<!-- note that we use the document title for link text, not the menu title -->		
	<xsl:template match="Page/Page">
		<li>
			<a href="{@URL}">
				<xsl:if test="@isopen='true'">
					<xsl:attribute name="class">selected</xsl:attribute>
				</xsl:if>
				<xsl:value-of select="@Title"/>
			</a>
		</li>
	</xsl:template>

</xsl:stylesheet>
