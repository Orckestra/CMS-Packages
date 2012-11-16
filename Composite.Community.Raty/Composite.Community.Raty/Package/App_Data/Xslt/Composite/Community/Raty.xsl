<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
xmlns:lang="http://www.composite.net/ns/localization/1.0"
xmlns:f="http://www.composite.net/ns/function/1.0"
xmlns="http://www.w3.org/1999/xhtml"
xmlns:msxsl="urn:schemas-microsoft-com:xslt"
xmlns:csharp="http://c1.composite.net/sample/csharp"
exclude-result-prefixes="xsl in lang f msxsl csharp">

	<xsl:param name="ratyId" select="/in:inputs/in:param[@name='RatyId']" />
	<xsl:param name="results" select="/in:inputs/in:result[@name='GetResultsXml']/Results" />
	<xsl:param name="half" select="/in:inputs/in:param[@name='Half']" />
	<xsl:param name="number" select="/in:inputs/in:param[@name='Number']" />
	<xsl:param name="readOnly" select="/in:inputs/in:param[@name='ReadOnly']" />
	<xsl:param name="cookie" select="/in:inputs/in:result[@name='CookieValue']" />

	<xsl:template match="/">
		<html>
			<head>
				<script id="jquery-js" src="//code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
				<script id="json2" src="/Frontend/Composite/Community/Raty/Scripts/json2.js" type="text/javascript"></script>
				<script id="jquery-raty-1-4-0" type="text/javascript" src="/Frontend/Composite/Community/Raty/Scripts/jquery.raty.min.js"></script>
				<link rel="stylesheet" type="text/css" href="/Frontend/Composite/Community/Raty/Styles/Raty.css" />
			</head>
			<body>
				<div class="rating">
					<div id="id{$ratyId}" class="results"></div>
					<div>
						<xsl:if test="count($results) = 0">
							<xsl:attribute name="style">display:none</xsl:attribute>
						</xsl:if>
						<lang:switch>
							<lang:when culture="ru-RU">
								<b class="count">
									<xsl:value-of select="$results/@Count" />
								</b> голосов, средняя оценка:
								<b class="avr"></b>
								<xsl:if test="contains($cookie,$ratyId)">
									&#160;<em>оценено</em>
								</xsl:if>
							</lang:when>
							<lang:default>
								based on
								<b class="count">
									<xsl:value-of select="$results/@Count" />
								</b> ratings, average:
								<b class="avr"></b>
								<xsl:if test="contains($cookie,$ratyId)">
									&#160;<em>voted</em>
								</xsl:if>
							</lang:default>
						</lang:switch>
					</div>
					<br class="clear" />
					<xsl:value-of select="csharp:NoCache()" />
				</div>
				<script type="text/javascript">
					$(document).ready(function(){
					var ratyId = '<xsl:value-of select="$ratyId" />';
					var count = '<xsl:value-of select="$results/@Count" />';
					var total= '<xsl:value-of select="$results/@TotalValue" />';

					$('#id' + ratyId ).raty({
					hintList: ['1', '2', '3', '4', '5'],
					half:  <xsl:value-of select="$half" />,
					number: <xsl:value-of select="$number" />,
					<xsl:choose>
						<xsl:when test="contains($cookie,$ratyId)">
							readOnly: true,
						</xsl:when>
						<xsl:otherwise>
							readOnly: <xsl:value-of select="$readOnly" />,
						</xsl:otherwise>
					</xsl:choose>
					path: '/Frontend/Composite/Community/Raty/jquery.raty-1.4.0/img',
					click: function(score,evt){rate(score, ratyId);}
					});
					setAvr(ratyId, total, count);
					});
				</script>
			</body>
		</html>
	</xsl:template>
	<msxsl:script implements-prefix="csharp" language="C#">
		<msxsl:assembly name="System.Web" />
		<msxsl:using namespace="System.Web" />
		public void NoCache()
		{
		HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}
	</msxsl:script>
</xsl:stylesheet>