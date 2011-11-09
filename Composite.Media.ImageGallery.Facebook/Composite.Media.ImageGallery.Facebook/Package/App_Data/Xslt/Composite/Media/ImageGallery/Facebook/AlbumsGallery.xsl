<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
	<xsl:variable name="albumId" select="/in:inputs/in:result[@name='AlbumID']" />
	<xsl:variable name="albumTypes" select="/in:inputs/in:param[@name='AlbumTypes']" />
	<xsl:variable name="objectUniqueID" select="/in:inputs/in:param[@name='ObjectUniqueID']" />
	<xsl:variable name="id" select="/in:inputs/in:result[@name='ID']" />
	<xsl:variable name="accessToken" select="/in:inputs/in:param[@name='AccessToken']" />
	<xsl:variable name="showFacebookLinksToTheAlbum" select="/in:inputs/in:param[@name='UseFacebookLinksToAlbums']" />
	<xsl:template match="/">
		<html>
			<head></head>
			<body>
				<xsl:choose>
					<xsl:when test="$albumId != '' and $id = $objectUniqueID">
						<f:function name="Composite.Media.ImageGallery.Facebook.PhotosList">
							<f:param name="AlbumID" value="{$albumId}" />
							<f:param name="AccessToken" value="{$accessToken}" />
						</f:function>
					</xsl:when>
					<xsl:otherwise>
						<f:function name="Composite.Media.ImageGallery.Facebook.AlbumsList">
							<f:param name="ObjectUniqueID" value="{$objectUniqueID}" />
							<f:param name="AccessToken" value="{$accessToken}" />
							<f:param name="AlbumTypes" value="{$albumTypes}" />
							<f:param name="ShowFacebookLinksToTheAlbum" value="{$showFacebookLinksToTheAlbum}" />
						</f:function>
					</xsl:otherwise>
				</xsl:choose>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>