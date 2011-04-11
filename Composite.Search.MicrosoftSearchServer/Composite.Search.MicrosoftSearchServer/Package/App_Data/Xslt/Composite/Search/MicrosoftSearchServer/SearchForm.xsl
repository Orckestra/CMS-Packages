<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:user="urn:my-scripts"
	exclude-result-prefixes="xsl in msxsl user">

	<xsl:param name="searchButtonLabel" select="/in:inputs/in:param[@name='SearchButtonLabel']" />
	<xsl:param name="searchButtonTitle" select="/in:inputs/in:param[@name='SearchButtontTitle']" />
	<xsl:param name="searchResultPage" select="/in:inputs/in:param[@name='SearchResultPage']" />
	<xsl:param name="searchQuery" select="/in:inputs/in:result[@name='SearchQuery']" />
	<xsl:param name="uniqueId" select="translate(user:generateId(),'-','')" />

	<msxsl:script language="C#" implements-prefix="user">
	<![CDATA[
		public string generateId()
		{
			return Guid.NewGuid().ToString();
		}
	]]>
	</msxsl:script>

	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Search/MicrosoftSearchServer/Styles.css" />
				<script type="text/javascript">
					function DoSearch<xsl:value-of select="$uniqueId" />OnEnter(e)
					{
						var keynum;
						var keychar;
						var numcheck;
						if(window.event) // IE
						{
							keynum = e.keyCode;
						}
						else if(e.which) // Netscape/Firefox/Opera
						{
							keynum = e.which;
						}
						if(keynum==13)
						{
							DoSearch<xsl:value-of select="$uniqueId" />();
							return false;
						}
						return true;
					}
					function DoSearch<xsl:value-of select="$uniqueId" />()
					{
						var param=document.getElementById('CompositeSearch<xsl:value-of select="$uniqueId" />').value;
						if (window.encodeURIComponent)
							param=encodeURIComponent(param);
						else
							param=escape(param);
						var searchUrl = '/Renderers/Page.aspx?pageId=<xsl:value-of select="$searchResultPage" />&amp;SearchQuery='+param;
						window.location = searchUrl;
					}
				</script>
			</head>
			<body>
				<p id="Search">
					<input id="CompositeSearch{$uniqueId}" type="text" value="Search" name="SearchQuery" onclick="if (this.value=='Search') {'{'}this.value=''{'}'}" onblur="if (this.value==''){'{'}this.value='Search'{'}'}" onkeydown="return DoSearch{$uniqueId}OnEnter(event);"  maxlength="1000">
						<xsl:if test="not($searchQuery='')">
							<xsl:attribute name="value">
								<xsl:value-of select="$searchQuery" />
							</xsl:attribute>
						</xsl:if>
					</input>
					<a id="Submit" href="javascript:DoSearch{$uniqueId}();" title="{$searchButtonTitle}">
						<xsl:value-of select="$searchButtonLabel" />
					</a>
				</p>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>
