<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.C1Console.Elements.Plugins.ElementProviderConfiguration/ElementProviderPlugins">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='Composite.Tools.PackageCreator'])=0">
				<add type="Composite.Tools.PackageCreator.ElementProvider.PackageCreatorElementProvider, Composite.Tools.PackageCreator" name="Composite.Tools.PackageCreator" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.C1Console.Elements.Plugins.ElementActionProviderConfiguration/ElementActionProviderPlugins">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='Composite.Tools.PackageCreator'])=0">
				<add name="Composite.Tools.PackageCreator" type="Composite.Tools.PackageCreator.PackageCreatorElementActionProvider, Composite.Tools.PackageCreator" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.C1Console.Elements.Plugins.ElementProviderConfiguration/ElementProviderPlugins/add[@name='VirtualElementProvider']/VirtualElements">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='Composite.Tools.PackageCreator'])=0">
				<add id="Composite.Tools.PackageCreator" order="100" parentId="ID01" providerName="Composite.Tools.PackageCreator" label="Package Creator" closeFolderIconName="Composite.Icons.tools" openFolderIconName="Composite.Icons.tools" type="Composite.Plugins.Elements.ElementProviders.VirtualElementProvider.ProviderHookingElementConfigurationElement, Composite" name="Composite.Tools.PackageCreator" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='Composite.Tools.PackageCreator'])=0">
				<add name="Composite.Tools.PackageCreator" defaultCultureName="en-US" type="Composite.Plugins.ResourceSystem.XmlStringResourceProvider.XmlStringResourceProvider, Composite">
					<Cultures>
						<add cultureName="en-US" xmlFile="~/Composite/InstalledPackages/localization/Composite.Tools.PackageCreator.en-us.xml" monitorFileChanges="true" />
					</Cultures>
				</add>
			</xsl:if>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>