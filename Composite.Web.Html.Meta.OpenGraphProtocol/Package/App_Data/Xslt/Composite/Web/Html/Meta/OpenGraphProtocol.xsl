<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" xmlns:og="http://ogp.me/ns#" exclude-result-prefixes="xsl in lang f">
	<xsl:variable name="currentPage" select="/in:inputs/in:result[@name='SitemapXml']//Page[@iscurrent='true']" />
	<xsl:variable name="graph" select="/in:inputs/in:result[@name='GetOpenGraphProtocolXml']/OpenGraphProtocol[@PageId=$currentPage/@Id]" />

	<xsl:param name="sitename" select="/in:inputs/in:param[@name='SiteName']" />
	<xsl:param name="admins" select="/in:inputs/in:param[@name='FacebookAdmins']" />
	<xsl:param name="appids" select="/in:inputs/in:param[@name='FacebookApplicationIDs']" />

	<xsl:variable name="host" select="/in:inputs/in:result[@name='Host']" />
	<xsl:variable name="appPath" select="/in:inputs/in:result[@name='ApplicationPath']" />
	<xsl:template match="/">
		<html>
			<head>
				<xsl:choose>
					<xsl:when test="$graph/@Title = ''">
						<meta property="og:title" content="{$currentPage/@Title}" />
					</xsl:when>
					<xsl:otherwise>
						<meta property="og:title" content="{$graph/@Title}" />
					</xsl:otherwise>
				</xsl:choose>
				<xsl:choose>
					<xsl:when test="$graph/@Type != ''">
						<meta property="og:type" content="{$graph/@Type}" />
					</xsl:when>
				</xsl:choose>
				<meta property="og:url" content="http://{$host}{$appPath}{$currentPage/@URL}" />
				<xsl:call-template name="Image">
					<xsl:with-param name="id" select="$currentPage/@Id" />
				</xsl:call-template>
				<xsl:choose>
					<xsl:when test="$graph/@Description != ''">
						<meta property="og:description" content="{$graph/@Description}" />
					</xsl:when>
					<xsl:otherwise>
						<xsl:if test="$currentPage/@Description != ''">
							<meta property="og:description" content="{$currentPage/@Description}" />
						</xsl:if>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:if test="$sitename != ''">
					<meta property="og:site_name" content="{$sitename}" />
				</xsl:if>
				<xsl:if test="$admins != ''">
					<meta property="fb:admins" content="{$admins}" xmlns:fb="http://www.facebook.com/2008/fbml" />
				</xsl:if>
				<xsl:if test="$appids != ''">
					<meta property="fb:app_id" content="{$appids}" xmlns:fb="http://www.facebook.com/2008/fbml" />
				</xsl:if>
			</head>
			<body></body>
		</html>
	</xsl:template>
	<xsl:template match="*" name="Image">
		<xsl:param name="id" />
		<xsl:param name="graph1" select="/in:inputs/in:result[@name='GetOpenGraphProtocolXml']/OpenGraphProtocol[@PageId=$id]" />
		<xsl:choose>
			<xsl:when test="$graph1/@Image != ''">
				<meta property="og:image" content="http://{$host}{$appPath}/media({$graph1/@Image})" />
			</xsl:when>
			<xsl:otherwise>
				<xsl:variable name="parentId" select="/in:inputs/in:result[@name='SitemapXml']//Page[Page[@Id = $id]]/@Id" />
				<xsl:if test="$parentId != ''">
					<xsl:call-template name="Image">
						<xsl:with-param name="id" select="$parentId" />
					</xsl:call-template>
				</xsl:if>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>