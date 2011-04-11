<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:user="urn:my-scripts"
	exclude-result-prefixes="xsl in msxsl user">

	<xsl:param name="images" select="/in:inputs/in:result[@name='GetIMediaFileXml']/IMediaFile" />

	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Media/ImageRotator/Styles.css" />
			</head>
			<body>
				<div class="ImageRotator">
					<xsl:if test="count($images)>0">
						<xsl:apply-templates select="user:GetRandomNode($images)" />
					</xsl:if>
				</div>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="IMediaFile[starts-with(@MimeType,'image')]">
		<img alt="image" src="~/Renderers/ShowMedia.ashx?id={@Id}" class="ImageRotator" />
	</xsl:template>

	<msxsl:script language="C#" implements-prefix="user">
		<msxsl:assembly name="System.Web" />
		<![CDATA[
			public XPathNavigator GetRandomNode(XPathNodeIterator nav)
			{
				int randomValue = (new Random()).Next(nav.Count);
				while (nav.MoveNext())
					if (nav.CurrentPosition > randomValue)
						break;
				return nav.Current;
			}
		]]>
	</msxsl:script>

</xsl:stylesheet>
