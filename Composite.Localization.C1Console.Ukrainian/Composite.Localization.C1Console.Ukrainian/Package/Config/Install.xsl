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
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Plugins.StandardFunctions.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Cultures']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Cultures.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Management']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Management.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Plugins.PageElementProvider.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.AllFunctionsElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Plugins.AllFunctionsElementProvider.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.MethodBasedFunctionProviderElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Plugins.MethodBasedFunctionProviderElementProvider.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.XsltBasedFunction']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Plugins.XsltBasedFunction.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.SecurityViolation']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.SecurityViolation.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.C1Console.Users']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.C1Console.Users.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.GenericPublishProcessController']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Plugins.GenericPublishProcessController.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.GeneratedDataTypesElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Plugins.GeneratedDataTypesElementProvider.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.TypeFieldDesigner']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Web.FormControl.TypeFieldDesigner.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.FunctionParameterDesigner']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Web.FormControl.FunctionParameterDesigner.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.SqlFunction']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Plugins.SqlFunction.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.VisualFunction']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Plugins.VisualFunction.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageTemplateElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Plugins.PageTemplateElementProvider.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.FunctionCallsDesigner']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Web.FormControl.FunctionCallsDesigner.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Permissions']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Permissions.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.GeneratedTypes']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.GeneratedTypes.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.NameValidation']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.NameValidation.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.WebsiteFileElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Plugins.WebsiteFileElementProvider.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.EntityTokenLocked']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.EntityTokenLocked.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PackageElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Plugins.PackageElementProvider.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Core.PackageSystem.PackageFragmentInstallers']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.PackageSystem.PackageFragmentInstallers.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.LocalizationElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Plugins.LocalizationElementProvider.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.SEOAssistant']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Web.SEOAssistant.uk-UA.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.VisualEditor']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Web.VisualEditor.uk-UA.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.SourceEditor']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Web.SourceEditor.uk-UA.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.PageBrowser']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Web.PageBrowser.uk-UA.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.UserGroupElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Plugins.UserGroupElementProvider.uk-UA.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.C1Console.Trees']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.C1Console.Trees.uk-UA.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageTypeElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/Composite/localization/Composite.Plugins.PageTypeElementProvider.uk-UA.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="configuration/Composite.Core.Configuration.Plugins.GlobalSettingsProviderConfiguration/GlobalSettingsProviderPlugins/add">
		<xsl:copy>
			<xsl:apply-templates select="@*" />
			<xsl:if test="not(contains(@applicationCultureNames, 'uk-UA'))">
				<xsl:attribute name="applicationCultureNames">
					<xsl:value-of select="concat(@applicationCultureNames,',uk-UA')" />
				</xsl:attribute>
			</xsl:if>
			<xsl:apply-templates select="node()" />
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>