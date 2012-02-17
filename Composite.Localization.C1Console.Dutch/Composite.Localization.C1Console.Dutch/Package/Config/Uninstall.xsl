<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	exclude-result-prefixes="msxsl">
	<xsl:output method="xml" indent="yes"/>
	
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.StandardFunctions']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Cultures']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Management']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageElementProvider']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.AllFunctionsElementProvider']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.MethodBasedFunctionProviderElementProvider']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.XsltBasedFunction']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.C1Console.Users']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.GenericPublishProcessController']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.GeneratedDataTypesElementProvider']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.TypeFieldDesigner']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.FunctionParameterDesigner']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.SqlFunction']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.VisualFunction']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageTemplateElementProvider']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.FormControl.FunctionCallsDesigner']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Permissions']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.GeneratedTypes']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.NameValidation']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.WebsiteFileElementProvider']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.EntityTokenLocked']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PackageElementProvider']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.PackageSystem.PackageFragmentInstallers']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.SecurityViolation']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

	<xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.SEOAssistant']/Cultures/add[@cultureName = 'nl-NL']">
	</xsl:template>

  <xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.C1Console.Trees']/Cultures/add[@cultureName = 'nl-NL']">
  </xsl:template>

  <xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Plugins.PageTypeElementProvider']/Cultures/add[@cultureName = 'nl-NL']">
  </xsl:template>

  <xsl:template match="configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins/add[@name='Composite.Web.VisualEditor']/Cultures/add[@cultureName = 'nl-NL']">
  </xsl:template>
  
</xsl:stylesheet>
