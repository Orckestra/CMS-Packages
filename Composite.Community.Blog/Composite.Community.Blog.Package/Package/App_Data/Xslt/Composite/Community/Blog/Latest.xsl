<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" xmlns:be="#BlogXsltExtensionsFunction" exclude-result-prefixes="xsl in lang f be">
	<xsl:template match="/">
		<html>
			<head>
				<!-- markup placed here will be shown in the head section of the rendered page -->
			</head>
			<body>
				<div class="BlogLatest">
					<ul>
						<xsl:for-each select="/in:inputs/in:result[@name='GetEntriesXml']/Entries">
							<li>
								<a href="~/page({@PageId}){be:GetBlogPath(@Date, @Title)}" title="{@Title}">
									<strong>
										<xsl:value-of select="@Title" />
									</strong>
								</a>
								<p>
									<xsl:value-of select="@Teaser" />
									<br />
									<a href="~/page({@PageId}){be:GetBlogPath(@Date, @Title)}">
										<lang:string key="Resource, Resources.Blog.readMoreText" />
									</a>
								</p>
							</li>
						</xsl:for-each>
					</ul>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>