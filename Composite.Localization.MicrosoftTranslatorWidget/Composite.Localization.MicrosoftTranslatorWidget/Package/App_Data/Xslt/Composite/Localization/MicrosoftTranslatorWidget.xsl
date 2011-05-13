<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
  xmlns:lang="http://www.composite.net/ns/localization/1.0"
  xmlns:f="http://www.composite.net/ns/function/1.0"
  xmlns="http://www.w3.org/1999/xhtml"
  xmlns:csharp="http://c1.composite.net/csharp"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  exclude-result-prefixes="xsl in lang f csharp msxsl">

	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="/Frontend/Composite/Localization/MicrosoftTranslatorWidget/Styles.css" />
			</head>
			<body>
				<div id="MicrosoftTranslatorWidget">
					<noscript>
						<a href="http://www.microsofttranslator.com/bv.aspx?a={csharp:GetCurrentUrl()}">Translate this page</a><br />Powered by <a href="http://www.microsofttranslator.com">Microsoft® Translator</a>
					</noscript>
				</div>
				<script type="text/javascript"> /* <![CDATA[ */ setTimeout(function() { var s = document.createElement("script"); s.type = "text/javascript"; s.charset = "UTF-8"; s.src = (!(!location || !location.href || location.href.indexOf('https') != 0) ? "https://ssl.microsofttranslator.com" : "http://www.microsofttranslator.com" ) + "/ajax/v2/widget.aspx?mode=manual&from=en&layout=ts"; var p = document.getElementsByTagName('head')[0] || document.documentElement; p.insertBefore(s, p.firstChild); }, 0); /* ]]> */ </script>
			</body>
		</html>
	</xsl:template>

	<msxsl:script implements-prefix="csharp" language="C#">
		<msxsl:assembly name="System.Web"/>
		<msxsl:using namespace="System.Web"/>
		<![CDATA[
		public string GetCurrentUrl()
		{
			string url = HttpContext.Current.Request.Url.ToString();
			return HttpUtility.UrlEncode(url);
		}
	]]>
	</msxsl:script>

</xsl:stylesheet>
