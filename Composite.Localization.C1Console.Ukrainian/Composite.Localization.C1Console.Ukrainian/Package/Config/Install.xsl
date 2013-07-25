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
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.StandardFunctions.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Cultures']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Cultures.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Management']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Management.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.PageElementProvider.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.AllFunctionsElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.AllFunctionsElementProvider.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.MethodBasedFunctionProviderElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.MethodBasedFunctionProviderElementProvider.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.XsltBasedFunction']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.XsltBasedFunction.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.SecurityViolation']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.C1Console.SecurityViolation.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.C1Console.Users']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.C1Console.Users.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.GenericPublishProcessController']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.GenericPublishProcessController.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.GeneratedDataTypesElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.GeneratedDataTypesElementProvider.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.TypeFieldDesigner']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Web.FormControl.TypeFieldDesigner.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.FunctionParameterDesigner']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Web.FormControl.FunctionParameterDesigner.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.SqlFunction']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.SqlFunction.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.VisualFunction']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.VisualFunction.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageTemplateElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.PageTemplateElementProvider.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.FunctionCallsDesigner']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Web.FormControl.FunctionCallsDesigner.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Permissions']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Permissions.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.GeneratedTypes']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.GeneratedTypes.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.NameValidation']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.NameValidation.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.WebsiteFileElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.WebsiteFileElementProvider.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.EntityTokenLocked']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.EntityTokenLocked.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PackageElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.PackageElementProvider.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Core.PackageSystem.PackageFragmentInstallers']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Core.PackageSystem.PackageFragmentInstallers.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.LocalizationElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.LocalizationElementProvider.uk-UA.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.SEOAssistant']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Web.SEOAssistant.uk-UA.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.VisualEditor']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Web.VisualEditor.uk-UA.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.SourceEditor']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Web.SourceEditor.uk-UA.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.PageBrowser']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Web.PageBrowser.uk-UA.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.UserGroupElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.UserGroupElementProvider.uk-UA.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.C1Console.Trees']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.C1Console.Trees.uk-UA.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
  <xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.MasterPagePageTemplate']/Cultures">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="count(add[@cultureName='uk-UA'])=0">
        <add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.MasterPagePageTemplate.uk-UA.xml" monitorFileChanges="true" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageTemplateFeatureElementProvider']/Cultures">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="count(add[@cultureName='uk-UA'])=0">
        <add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.PageTemplateFeatureElementProvider.uk-UA.xml" monitorFileChanges="true" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.RazorFunction']/Cultures">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="count(add[@cultureName='uk-UA'])=0">
        <add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.RazorFunction.uk-UA.xml" monitorFileChanges="true" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.RazorPageTemplate']/Cultures">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="count(add[@cultureName='uk-UA'])=0">
        <add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.RazorPageTemplate.uk-UA.xml" monitorFileChanges="true" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.UserControlFunction']/Cultures">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="count(add[@cultureName='uk-UA'])=0">
        <add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.UserControlFunction.uk-UA.xml" monitorFileChanges="true" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>
  <xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageTypeElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Plugins.PageTypeElementProvider.uk-UA.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.UrlConfiguration']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='uk-UA'])=0">
				<add cultureName="uk-UA" xmlFile="~/App_Data/Composite/LanguagePacks/uk-UA/Composite.Web.UrlConfiguration.uk-UA.xml" monitorFileChanges="true" />
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