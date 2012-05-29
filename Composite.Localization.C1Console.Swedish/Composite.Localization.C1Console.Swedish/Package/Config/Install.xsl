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
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Plugins.StandardFunctions.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Cultures']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Cultures.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Management']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Management.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Plugins.PageElementProvider.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.AllFunctionsElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Plugins.AllFunctionsElementProvider.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.MethodBasedFunctionProviderElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Plugins.MethodBasedFunctionProviderElementProvider.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.XsltBasedFunction']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Plugins.XsltBasedFunction.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.C1Console.Users']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.C1Console.Users.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.GenericPublishProcessController']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Plugins.GenericPublishProcessController.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.GeneratedDataTypesElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Plugins.GeneratedDataTypesElementProvider.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.TypeFieldDesigner']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Web.FormControl.TypeFieldDesigner.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.FunctionParameterDesigner']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Web.FormControl.FunctionParameterDesigner.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.SqlFunction']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Plugins.SqlFunction.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.VisualFunction']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Plugins.VisualFunction.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageTemplateElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Plugins.PageTemplateElementProvider.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.FunctionCallsDesigner']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Web.FormControl.FunctionCallsDesigner.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Permissions']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Permissions.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.GeneratedTypes']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.GeneratedTypes.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.NameValidation']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.NameValidation.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.WebsiteFileElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Plugins.WebsiteFileElementProvider.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.EntityTokenLocked']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.EntityTokenLocked.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.C1Console.SecurityViolation']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.SecurityViolation.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PackageElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Plugins.PackageElementProvider.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Core.PackageSystem.PackageFragmentInstallers']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.PackageSystem.PackageFragmentInstallers.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.LocalizationElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Plugins.LocalizationElementProvider.sv-SE.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.SEOAssistant']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Web.SEOAssistant.sv-SE.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.VisualEditor']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Web.VisualEditor.sv-SE.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.SourceEditor']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Web.SourceEditor.sv-SE.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.PageBrowser']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Web.PageBrowser.sv-SE.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.UserGroupElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Plugins.UserGroupElementProvider.sv-SE.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.C1Console.Trees']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.C1Console.Trees.sv-SE.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageTypeElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Plugins.PageTypeElementProvider.sv-SE.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.UrlConfiguration']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='sv-SE'])=0">
				<add cultureName="sv-SE" xmlFile="~/App_Data/Composite/LanguagePacks/sv-SE/Composite.Web.UrlConfiguration.sv-SE.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="configuration/Composite.Core.Configuration.Plugins.GlobalSettingsProviderConfiguration/GlobalSettingsProviderPlugins/add">
		<xsl:copy>
			<xsl:apply-templates select="@*" />
			<xsl:if test="not(contains(@applicationCultureNames, 'sv-SE'))">
				<xsl:attribute name="applicationCultureNames">
					<xsl:value-of select="concat(@applicationCultureNames,',sv-SE')" />
				</xsl:attribute>
			</xsl:if>
			<xsl:apply-templates select="node()" />
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>