<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:csharp="http://c1.composite.net/csharp"
	exclude-result-prefixes="xsl in lang f csharp">

	<xsl:variable name="AlwaysStayOnHttps" select="/in:inputs/in:param[@name='AlwaysStayOnHttps']" />
	<xsl:variable name="RedirectC1Users" select="/in:inputs/in:param[@name='RedirectC1Users']" />

	<xsl:template match="/">
		<html>
			<head />
			<body>
				<xsl:if test="csharp:IsRedirectRequired($RedirectC1Users)">
					<xsl:apply-templates select="/in:inputs/in:result[@name='GetPageSettingsXml']" />
					<xsl:if test="count(/in:inputs/in:result[@name='GetPageSettingsXml']/PageSettings)=0 and $AlwaysStayOnHttps='false'">
						<xsl:value-of select="csharp:Redirect('false')" />
					</xsl:if>
				</xsl:if>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="PageSettings">
		<xsl:value-of select="csharp:Redirect('true')" />
	</xsl:template>

	<msxsl:script implements-prefix="csharp" language="C#" xmlns:msxsl="urn:schemas-microsoft-com:xslt">
		<msxsl:assembly name="System.Web" />
		<msxsl:using namespace="System.Web" />
		<msxsl:assembly name="Composite" />
		<msxsl:using namespace="Composite.Core.ResourceSystem" />
		<![CDATA[
		public void Redirect(string toSecureConnection)
		{
			string redirectUrl = HttpContext.Current.Request.Url.ToString();

			if(toSecureConnection == "true" && !HttpContext.Current.Request.IsSecureConnection)
				HttpContext.Current.Response.Redirect(redirectUrl.Replace("http:", "https:"), false);

			if(toSecureConnection == "false" && HttpContext.Current.Request.IsSecureConnection)
				HttpContext.Current.Response.Redirect(redirectUrl.Replace("https:", "http:"), false);
		}

		public bool IsRedirectRequired(string redirectC1Users)
		{
			return (Composite.C1Console.Security.UserValidationFacade.IsLoggedIn() && redirectC1Users == "false") ? false : true;
		}
		]]>
	</msxsl:script>

</xsl:stylesheet>
