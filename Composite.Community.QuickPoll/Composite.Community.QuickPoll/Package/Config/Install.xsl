<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.C1Console.Forms.Plugins.ProducerMediatorConfiguration/Mediators">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='http://www.composite.net/ns/Composite/Community/QuickPoll/1.0'])=0">
				<add type="Composite.C1Console.Forms.StandardProducerMediators.UiControlProducerMediator, Composite" name="http://www.composite.net/ns/Composite/Community/QuickPoll/1.0" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.C1Console.Forms.Plugins.UiControlFactoryConfiguration/Channels/Channel[@name='AspNet.Management']/Namespaces">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(Namespace[@name='http://www.composite.net/ns/Composite/Community/QuickPoll/1.0'])=0">
				<Namespace name="http://www.composite.net/ns/Composite/Community/QuickPoll/1.0">
					<Factories>
						<add userControlVirtualPath="~/Frontend/Composite/Community/QuickPoll/Widgets/QuickPollResults.ascx" name="QuickPollResults" cacheCompiledUserControlType="false" type="Composite.Plugins.Forms.WebChannel.UiControlFactories.UserControlBasedUiControlFactory, Composite" />
					</Factories>
				</Namespace>
			</xsl:if>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>