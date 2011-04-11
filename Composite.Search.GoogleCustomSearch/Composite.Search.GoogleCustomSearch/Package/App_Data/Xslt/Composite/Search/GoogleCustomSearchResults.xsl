<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
	<xsl:template match="/">
		<html>
			<head></head>
			<body>
				<div id="cse-search-results"></div>
				<script type="text/javascript">
					var googleSearchIframeName = "cse-search-results";
					var googleSearchFormName = "cse-search-box";
					var googleSearchFrameWidth = 600;
					var googleSearchDomain = "www.google.com";
					var googleSearchPath = "/cse";
				</script>
				<script type="text/javascript" src="http://www.google.com/afsonline/show_afs_search.js"></script>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>