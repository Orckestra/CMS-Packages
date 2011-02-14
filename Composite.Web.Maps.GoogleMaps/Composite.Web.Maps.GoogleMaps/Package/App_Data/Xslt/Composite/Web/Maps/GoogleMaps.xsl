<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:csharp="http://c1.composite.net/csharp"
	exclude-result-prefixes="xsl in lang f msxsl csharp">

	<xsl:param name="address" select="/in:inputs/in:param[@name='Address']" />
	<xsl:param name="encodedAddress" select="csharp:UrlEncode($address)" />
	<xsl:param name="zoom" select="/in:inputs/in:param[@name='Zoom']" />
	<xsl:param name="height" select="/in:inputs/in:param[@name='Height']" />
	<xsl:param name="width" select="/in:inputs/in:param[@name='Width']" />
	<xsl:template match="/">
		<html>
			<head></head>
			<body>
				<a href="http://maps.google.com/maps?q={$encodedAddress}&amp;z={$zoom}" target="_blank">
					<img src="http://maps.google.com/maps/api/staticmap?center={$encodedAddress}&amp;zoom={$zoom}&amp;size={$width}x{$height}&amp;sensor=false&amp;markers=color:blue|{$address}" border="0"/>
				</a>
			</body>
		</html>
	</xsl:template>
	<msxsl:script language="C#" implements-prefix="csharp">
		<msxsl:assembly name="System.Web" />
		<msxsl:using namespace="System.Web" />
		<![CDATA[
			public string UrlEncode(string text)
			{
				return HttpUtility.UrlEncode(text);
			}
		]]>
	</msxsl:script>
</xsl:stylesheet>
