<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
	exclude-result-prefixes="msxsl">
	<xsl:output method="xml" indent="yes" />

	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.StandardFunctions']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Plugins.StandardFunctions.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Cultures']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Cultures.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Management']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Management.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageElementProvider']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Plugins.PageElementProvider.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.AllFunctionsElementProvider']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Plugins.AllFunctionsElementProvider.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.MethodBasedFunctionProviderElementProvider']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Plugins.MethodBasedFunctionProviderElementProvider.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.XsltBasedFunction']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Plugins.XsltBasedFunction.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.C1Console.Users']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.C1Console.Users.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.GenericPublishProcessController']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Plugins.GenericPublishProcessController.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.GeneratedDataTypesElementProvider']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Plugins.GeneratedDataTypesElementProvider.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.TypeFieldDesigner']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Web.FormControl.TypeFieldDesigner.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.FunctionParameterDesigner']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Web.FormControl.FunctionParameterDesigner.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.SqlFunction']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Plugins.SqlFunction.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.VisualFunction']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Plugins.VisualFunction.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageTemplateElementProvider']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Plugins.PageTemplateElementProvider.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.FunctionCallsDesigner']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Web.FormControl.FunctionCallsDesigner.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Permissions']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Permissions.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.GeneratedTypes']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.GeneratedTypes.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.NameValidation']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.NameValidation.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.WebsiteFileElementProvider']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Plugins.WebsiteFileElementProvider.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.EntityTokenLocked']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.EntityTokenLocked.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()"/>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PackageElementProvider']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Plugins.PackageElementProvider.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.PackageSystem.PackageFragmentInstallers']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.PackageSystem.PackageFragmentInstallers.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.SecurityViolation']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.SecurityViolation.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.SEOAssistant']/Cultures">
		<xsl:copy>
			<xsl:if test="count(add[@cultureName = 'nl-NL']) = 0">
				<add cultureName="nl-NL" xmlFile="~/App_Data/Composite/LanguagePacks/nl-NL/Composite.Web.SEOAssistant.nl-NL.xml" monitorFileChanges="true" />
			</xsl:if>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

</xsl:stylesheet>
