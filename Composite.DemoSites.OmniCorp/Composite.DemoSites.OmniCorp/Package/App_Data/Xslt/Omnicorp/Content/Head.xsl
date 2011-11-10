<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="xsl in lang f">
	
	<!-- This variable is inserted on the "Function Calls" tab -->
	<xsl:variable name="query" select="/in:inputs/in:result[@name='QueryStringValue']"/>
	
	<xsl:template match="/">
		<html>
			<head/>
			<body>
				<div id="banner">
					<h2>
						<a title="Go to the front page" href="/Renderers/Page.aspx?pageId=69f9e0f1-e2ce-4b99-a1ba-5117964b0793">Omnicorp</a>
					</h2>
					<p>The Composite Demo company</p>
				</div>
				<div id="search">
					<form action="/Renderers/Page.aspx?pageId=f95261d2-dc5e-4304-9a2d-ef27a18e63b8" method="get">
						<!-- while navigation the Search section, search queries are shown in the search field -->
						<input type="text" class="text" name="SearchQuery" value=""/>
						<input type="submit" class="submit" value="Search"/>
					</form>
				</div>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>
