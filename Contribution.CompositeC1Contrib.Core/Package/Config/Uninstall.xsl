<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<!--Uninstall Master Pages-->
	<xsl:template match="/configuration/system.web/pages/controls/add[@assembly='CompositeC1Contrib']" />
	<xsl:template match="/configuration/system.web/httpHandlers/add[@type='CompositeC1Contrib.Web.UI.CompositeC1Page, CompositeC1Contrib']" />
	<xsl:template match="/configuration/system.webServer/handlers/add[@type='CompositeC1Contrib.Web.UI.CompositeC1Page, CompositeC1Contrib']" />
	<!--Uninstall Sitemap Provider-->
	<xsl:template match="/configuration/system.web/siteMap/providers/add[@type='CompositeC1Contrib.Web.CompositeC1SiteMapProvider, CompositeC1Contrib']" />
	<!--Uninstall Nicer URls-->
	<xsl:template match="/configuration/system.web/httpModules/add[@type='CompositeC1Contrib.Web.UrlFilterModule, CompositeC1Contrib']" />
	<xsl:template match="/configuration/system.webServer/modules/add[@type='CompositeC1Contrib.Web.UrlFilterModule, CompositeC1Contrib']" />
	<!--Uninstall Sitemap Protocol-->
	<xsl:template match="/configuration/system.web/httpHandlers/add[@type='CompositeC1Contrib.Web.SiteMapHandler, CompositeC1Contrib']" />
	<xsl:template match="/configuration/system.webServer/handlers/add[@type='CompositeC1Contrib.Web.SiteMapHandler, CompositeC1Contrib']" />
	<xsl:template match="/configuration/system.web/httpModules/add[@type='CompositeC1Contrib.Web.GlobalizationModule, CompositeC1Contrib']" />
	<xsl:template match="/configuration/system.webServer/modules/add[@type='CompositeC1Contrib.Web.GlobalizationModule, CompositeC1Contrib']" />
</xsl:stylesheet>