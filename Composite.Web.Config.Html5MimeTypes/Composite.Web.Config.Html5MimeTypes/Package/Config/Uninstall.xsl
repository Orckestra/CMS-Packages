<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/system.webServer/staticContent/remove[@fileExtension='.eot']" />

	<xsl:template match="/configuration/system.webServer/staticContent/mimeMap[@fileExtension='.mp4']" />
	<xsl:template match="/configuration/system.webServer/staticContent/mimeMap[@fileExtension='.m4v']" />
	<xsl:template match="/configuration/system.webServer/staticContent/mimeMap[@fileExtension='.ogg']" />
	<xsl:template match="/configuration/system.webServer/staticContent/mimeMap[@fileExtension='.ogv']" />
	<xsl:template match="/configuration/system.webServer/staticContent/mimeMap[@fileExtension='.oga']" />
	<xsl:template match="/configuration/system.webServer/staticContent/mimeMap[@fileExtension='.spx']" />
	<xsl:template match="/configuration/system.webServer/staticContent/mimeMap[@fileExtension='.svg']" />
	<xsl:template match="/configuration/system.webServer/staticContent/mimeMap[@fileExtension='.svgz']" />
	<xsl:template match="/configuration/system.webServer/staticContent/mimeMap[@fileExtension='.eot']" />
	<xsl:template match="/configuration/system.webServer/staticContent/mimeMap[@fileExtension='.otf']" />
	<xsl:template match="/configuration/system.webServer/staticContent/mimeMap[@fileExtension='.woff']" />
	<xsl:template match="/configuration/system.webServer/staticContent/mimeMap[@fileExtension='.appcache']" />
	<xsl:template match="/configuration/system.webServer/staticContent/mimeMap[@fileExtension='.less']" />
</xsl:stylesheet>