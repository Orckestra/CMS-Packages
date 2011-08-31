<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
	<xsl:param name="toplinks" select="/in:inputs/in:result[@name='GetTopLinkXml']/TopLink" />
	<xsl:param name="sublinks" select="/in:inputs/in:result[@name='GetSubLinkXml']/SubLink" />
	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Navigation/Quickmap/Styles.css" />
			</head>
			<body>
				<div id="Quickmap">
					<xsl:if test="$toplinks">
						<ul>
							<xsl:for-each select="$toplinks">
								<li>
									<xsl:apply-templates select="." />
									<xsl:if test="$sublinks[@TopLink.Id=current()/@Id]">
										<ul>
											<xsl:for-each select="$sublinks[@TopLink.Id=current()/@Id]">
												<li>
													<xsl:apply-templates select="." />
												</li>
											</xsl:for-each>
										</ul>
									</xsl:if>
								</li>
							</xsl:for-each>
						</ul>
					</xsl:if>
					<div class="Clear" />
				</div>
			</body>
		</html>
	</xsl:template>
	<xsl:template match="TopLink | SubLink">
		<a href="~/Renderers/Page.aspx?pageId={@Page}">
			<xsl:choose>
				<xsl:when test="@Page.MenuTitle!=''">
					<xsl:value-of select="@Page.MenuTitle" />
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="@Page.Title" />
				</xsl:otherwise>
			</xsl:choose>
		</a>
	</xsl:template>
</xsl:stylesheet>