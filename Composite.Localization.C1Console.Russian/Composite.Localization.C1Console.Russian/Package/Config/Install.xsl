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
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.StandardFunctions.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Cultures']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Cultures.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Management']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Management.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.PageElementProvider.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.AllFunctionsElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.AllFunctionsElementProvider.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.MethodBasedFunctionProviderElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.MethodBasedFunctionProviderElementProvider.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.XsltBasedFunction']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.XsltBasedFunction.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.C1Console.SecurityViolation']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.C1Console.SecurityViolation.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.C1Console.Users']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.C1Console.Users.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.GeneratedDataTypesElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.GeneratedDataTypesElementProvider.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.GenericPublishProcessController']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.GenericPublishProcessController.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.TypeFieldDesigner']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Web.FormControl.TypeFieldDesigner.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.FunctionParameterDesigner']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Web.FormControl.FunctionParameterDesigner.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.SqlFunction']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.SqlFunction.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.VisualFunction']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.VisualFunction.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageTemplateElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.PageTemplateElementProvider.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.FunctionCallsDesigner']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Web.FormControl.FunctionCallsDesigner.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Permissions']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Permissions.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.GeneratedTypes']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.GeneratedTypes.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.NameValidation']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.NameValidation.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.WebsiteFileElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.WebsiteFileElementProvider.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.EntityTokenLocked']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.EntityTokenLocked.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PackageElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.PackageElementProvider.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Core.PackageSystem.PackageFragmentInstallers']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Core.PackageSystem.PackageFragmentInstallers.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.LocalizationElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.LocalizationElementProvider.ru-RU.xml" monitorFileChanges="false" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.SEOAssistant']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Web.SEOAssistant.ru-RU.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.VisualEditor']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Web.VisualEditor.ru-RU.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.SourceEditor']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Web.SourceEditor.ru-RU.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.PageBrowser']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Web.PageBrowser.ru-RU.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.UserGroupElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.UserGroupElementProvider.ru-RU.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.C1Console.Trees']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.C1Console.Trees.ru-RU.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
  <xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.MasterPagePageTemplate']/Cultures">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="count(add[@cultureName='ru-RU'])=0">
        <add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.MasterPagePageTemplate.ru-RU.xml" monitorFileChanges="true" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageTemplateFeatureElementProvider']/Cultures">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="count(add[@cultureName='ru-RU'])=0">
        <add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.PageTemplateFeatureElementProvider.ru-RU.xml" monitorFileChanges="true" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.RazorFunction']/Cultures">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="count(add[@cultureName='ru-RU'])=0">
        <add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.RazorFunction.ru-RU.xml" monitorFileChanges="true" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.RazorPageTemplate']/Cultures">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="count(add[@cultureName='ru-RU'])=0">
        <add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.RazorPageTemplate.ru-RU.xml" monitorFileChanges="true" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.UserControlFunction']/Cultures">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="count(add[@cultureName='ru-RU'])=0">
        <add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.UserControlFunction.ru-RU.xml" monitorFileChanges="true" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>
  <xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageTypeElementProvider']/Cultures">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@cultureName='ru-RU'])=0">
				<add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Plugins.PageTypeElementProvider.ru-RU.xml" monitorFileChanges="true" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
  <xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.UrlConfiguration']/Cultures">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="count(add[@cultureName='ru-RU'])=0">
        <add cultureName="ru-RU" xmlFile="~/App_Data/Composite/LanguagePacks/ru-RU/Composite.Web.UrlConfiguration.ru-RU.xml" monitorFileChanges="true" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>
	<xsl:template match="configuration/Composite.Core.Configuration.Plugins.GlobalSettingsProviderConfiguration/GlobalSettingsProviderPlugins/add">
		<xsl:copy>
			<xsl:apply-templates select="@*" />
			<xsl:if test="not(contains(@applicationCultureNames, 'ru-RU'))">
				<xsl:attribute name="applicationCultureNames">
					<xsl:value-of select="concat(@applicationCultureNames,',ru-RU')" />
				</xsl:attribute>
			</xsl:if>
			<xsl:apply-templates select="node()" />
		</xsl:copy>
	</xsl:template>

</xsl:stylesheet>