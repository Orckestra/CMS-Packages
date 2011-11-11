<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Functions.Plugins.WidgetFunctionProviderConfiguration/WidgetFunctionProviderPlugins">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='FileUploadWidgetFunctionProviderSettingsNode'])=0">
				<add type="Composite.Forms.Renderer.FileUpload.FileUploadWidgetFunctionProvider, Composite.Forms.Renderer.FileUpload" name="FileUploadWidgetFunctionProviderSettingsNode" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.C1Console.Forms.Plugins.UiControlFactoryConfiguration/Channels/Channel[@name='AspNet.FormsRenderer']/Namespaces/Namespace[@name='http://www.composite.net/ns/management/bindingforms/std.ui.controls.lib/1.0']/Factories">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='FrontendFileUpload'])=0">
				<add userControlVirtualPath="~/Composite/InstalledPackages/controls/FormsControls/Composite.Forms.Renderer.FileUpload/AspNet.FormsRenderer/FrontendFileUpload.ascx" cacheCompiledUserControlType="true" type="Composite.Plugins.Forms.WebChannel.UiControlFactories.TemplatedTextInputUiControlFactory, Composite" name="FrontendFileUpload" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.C1Console.Forms.Plugins.UiControlFactoryConfiguration/Channels/Channel[@name='AspNet.Management']/Namespaces/Namespace[@name='http://www.composite.net/ns/management/bindingforms/std.ui.controls.lib/1.0']/Factories">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='FrontendFileUpload'])=0">
				<add userControlVirtualPath="~/Composite/InstalledPackages/controls/FormsControls/Composite.Forms.Renderer.FileUpload/AspNet.Management/FrontendFileUpload.ascx" cacheCompiledUserControlType="true" type="Composite.Plugins.Forms.WebChannel.UiControlFactories.TemplatedTextInputUiControlFactory, Composite" name="FrontendFileUpload" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>