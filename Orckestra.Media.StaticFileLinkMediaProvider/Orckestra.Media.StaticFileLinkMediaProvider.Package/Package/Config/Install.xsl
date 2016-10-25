<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()" />
        </xsl:copy>
    </xsl:template>
    <xsl:template match="/configuration/Composite.Data.Plugins.DataProviderConfiguration/DataProviderPlugins">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()" />
            <xsl:if test="count(add[@name='StaticFileLinkMediaProvider'])=0">
                <add basePath="~/download" storeTitle="Static files from ~/download" storeDescription="" storeId="download" name="StaticFileLinkMediaProvider" type="Orckestra.Media.StaticFileLinkMediaProvider.MediaProvider, Orckestra.Media.StaticFileLinkMediaProvider" />
            </xsl:if>
        </xsl:copy>
    </xsl:template>
</xsl:stylesheet>