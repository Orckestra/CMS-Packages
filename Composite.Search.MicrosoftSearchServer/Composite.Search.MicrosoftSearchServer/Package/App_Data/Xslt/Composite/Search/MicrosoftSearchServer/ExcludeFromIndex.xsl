<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" exclude-result-prefixes="xsl in mp csharp"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:csharp="http://c1.composite.net/sample/csharp"	
	xmlns:mp="#MarkupParserExtensions">

	<xsl:param name="content" select="/in:inputs/in:param[@name='Content']" />
	
	<xsl:template match="/">
		<html>
			<head>
				<!-- markup placed here will be shown in the head section of the rendered page -->
			</head>
			<body>
				<xsl:if test="csharp:IsNotBot()">
					<xsl:copy-of select="mp:ParseWellformedDocumentMarkup($content)" />
				</xsl:if>
			</body>
		</html>
	</xsl:template>

	<msxsl:script implements-prefix="csharp" language="C#">
	<msxsl:using namespace="System.Web"/>
	<msxsl:assembly name="System.Web"/>
		<![CDATA[
			public bool IsNotBot()
			{
				bool isNotBot = true;

				if(HttpContext.Current.Request.UserAgent != null)
				{
					string userAgent = HttpContext.Current.Request.UserAgent.ToLower();
					string[] botKeywords = new string[] { "robot" };

					foreach (string bot in botKeywords)
					{
						if (userAgent.Contains(bot))
						{
							isNotBot = false;
							break;
						}
					}
				}
				return isNotBot;
			}
		]]>
	</msxsl:script>
</xsl:stylesheet>
