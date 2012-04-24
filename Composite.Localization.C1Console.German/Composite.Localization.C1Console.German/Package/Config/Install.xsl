<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.StandardFunctions']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Plugins.StandardFunctions.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Cultures']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Cultures.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Management']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Management.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Plugins.PageElementProvider.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.AllFunctionsElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Plugins.AllFunctionsElementProvider.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.MethodBasedFunctionProviderElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Plugins.MethodBasedFunctionProviderElementProvider.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.XsltBasedFunction']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Plugins.XsltBasedFunction.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.C1Console.Users']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.C1Console.Users.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.GenericPublishProcessController']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Plugins.GenericPublishProcessController.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.GeneratedDataTypesElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Plugins.GeneratedDataTypesElementProvider.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.TypeFieldDesigner']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Web.FormControl.TypeFieldDesigner.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.FunctionParameterDesigner']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Web.FormControl.FunctionParameterDesigner.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.SqlFunction']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Plugins.SqlFunction.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.VisualFunction']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Plugins.VisualFunction.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageTemplateElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Plugins.PageTemplateElementProvider.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.FunctionCallsDesigner']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Web.FormControl.FunctionCallsDesigner.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Permissions']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Permissions.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.GeneratedTypes']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.GeneratedTypes.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.NameValidation']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.NameValidation.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.WebsiteFileElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Plugins.WebsiteFileElementProvider.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.EntityTokenLocked']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.EntityTokenLocked.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PackageElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Plugins.PackageElementProvider.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Core.PackageSystem.PackageFragmentInstallers']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.PackageSystem.PackageFragmentInstallers.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.LocalizationElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Plugins.LocalizationElementProvider.de-DE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.SEOAssistant']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Web.SEOAssistant.de-DE.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.VisualEditor']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Web.VisualEditor.de-DE.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.SourceEditor']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Web.SourceEditor.de-DE.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.PageBrowser']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Web.PageBrowser.de-DE.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.UserGroupElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Plugins.UserGroupElementProvider.de-DE.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.C1Console.Trees']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.C1Console.Trees.de-DE.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageTypeElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Plugins.PageTypeElementProvider.de-DE.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.UrlConfiguration']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='de-DE'])=0">
				<add cultureName="de-DE" xmlFile="~/App_Data/Composite/LanguagePacks/de-DE/Composite.Web.UrlConfiguration.de-DE.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="configuration/Composite.Core.Configuration.Plugins.GlobalSettingsProviderConfiguration/GlobalSettingsProviderPlugins/add">
		<xsl:copy>
			<xsl:apply-templates select="@*" />
			<xsl:if test="not(contains(@applicationCultureNames, 'de-DE'))">
				<xsl:attribute name="applicationCultureNames">
					<xsl:value-of select="concat(@applicationCultureNames,',de-DE')" />
				</xsl:attribute>
			</xsl:if>
			<xsl:apply-templates select="node()" />
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>