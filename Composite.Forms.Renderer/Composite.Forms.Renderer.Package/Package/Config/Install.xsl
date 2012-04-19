<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.C1Console.Forms.Plugins.UiControlFactoryConfiguration/Channels">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(Channel[@name='AspNet.FormsRenderer'])=0">
				<Channel debugControlNamespace="http://www.composite.net/ns/management/bindingforms/internal.ui.controls.lib/1.0" debugControlName="Debug" name="AspNet.FormsRenderer">
					<Namespaces>
						<Namespace name="http://www.composite.net/ns/management/bindingforms/std.ui.controls.lib/1.0">
							<Factories>
								<add userControlVirtualPath="~/Frontend/Composite/Forms/Renderer/Widgets/DateTimeSelectors/DateSelector.ascx" cacheCompiledUserControlType="true" type="Composite.Plugins.Forms.WebChannel.UiControlFactories.TemplatedDateTimeSelectorUiControlFactory, Composite" showHours="false" name="DateSelector" />
								<add userControlVirtualPath="~/Frontend/Composite/Forms/Renderer/Widgets/DateTimeSelectors/DateSelector.ascx" cacheCompiledUserControlType="true" type="Composite.Plugins.Forms.WebChannel.UiControlFactories.TemplatedDateTimeSelectorUiControlFactory, Composite" showHours="true" name="DateTimeSelector" />
								<add userControlVirtualPath="~/Frontend/Composite/Forms/Renderer/Widgets/BoolSelectors/CheckBox.ascx" cacheCompiledUserControlType="true" type="Composite.Plugins.Forms.WebChannel.UiControlFactories.TemplatedCheckBoxUiControlFactory, Composite" name="CheckBox" />
								<add userControlVirtualPath="~/Frontend/Composite/Forms/Renderer/Widgets/BoolSelectors/BoolSelector.ascx" cacheCompiledUserControlType="true" type="Composite.Plugins.Forms.WebChannel.UiControlFactories.TemplatedBoolSelectorUiControlFactory, Composite" name="BoolSelector" />
								<add userControlVirtualPath="~/Frontend/Composite/Forms/Renderer/Widgets/Selectors/Selector.ascx" cacheCompiledUserControlType="true" BindingType="BindToKeyFieldValue" MultiSelector="false" type="Composite.Plugins.Forms.WebChannel.UiControlFactories.TemplatedSelectorUiControlFactory, Composite" name="KeySelector" />
								<add userControlVirtualPath="~/Frontend/Composite/Forms/Renderer/Widgets/Containers/FieldGroup.ascx" cacheCompiledUserControlType="true" type="Composite.Plugins.Forms.WebChannel.UiControlFactories.TemplatedContainerUiControlFactory, Composite" name="FieldGroup" IsTabbedContainer="false" IsFullWidthControl="false" />
								<add userControlVirtualPath="~/Frontend/Composite/Forms/Renderer/Widgets/TextInput/TextArea.ascx" cacheCompiledUserControlType="true" type="Composite.Plugins.Forms.WebChannel.UiControlFactories.TemplatedTextInputUiControlFactory, Composite" name="TextArea" />
								<add userControlVirtualPath="~/Frontend/Composite/Forms/Renderer/Widgets/TextInput/TextBox.ascx" cacheCompiledUserControlType="true" type="Composite.Plugins.Forms.WebChannel.UiControlFactories.TemplatedTextInputUiControlFactory, Composite" name="TextBox" />
								<add userControlVirtualPath="~/Frontend/Composite/Forms/Renderer/Widgets/RichContent/InlineXhtmlEditor.ascx" cacheCompiledUserControlType="true" ClassConfigurationName="common" type="Composite.Plugins.Forms.WebChannel.UiControlFactories.TemplatedXhtmlEditorUiControlFactory, Composite" name="InlineXhtmlEditor" />
								<add userControlVirtualPath="~/Frontend/Composite/Forms/Renderer/Widgets/Selectors/MultiKeySelector.ascx" cacheCompiledUserControlType="true" BindingType="BindToKeyFieldValue" MultiSelector="true" type="Composite.Plugins.Forms.WebChannel.UiControlFactories.TemplatedSelectorUiControlFactory, Composite" name="MultiKeySelector" />
								<add userControlVirtualPath="~/Frontend/Composite/Forms/Renderer/Widgets/Containers/TabPanels.ascx" cacheCompiledUserControlType="true" type="Composite.Plugins.Forms.WebChannel.UiControlFactories.TemplatedContainerUiControlFactory, Composite" name="TabPanels" IsTabbedContainer="true" IsFullWidthControl="true" />
								<add userControlVirtualPath="~/Frontend/Composite/Forms/Renderer/Widgets/Containers/PlaceHolder.ascx" cacheCompiledUserControlType="true" type="Composite.Plugins.Forms.WebChannel.UiControlFactories.TemplatedContainerUiControlFactory, Composite" name="PlaceHolder" IsTabbedContainer="false" IsFullWidthControl="false" />
							</Factories>
						</Namespace>
					</Namespaces>
				</Channel>
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Core.ResourceSystem.Plugins.ResourceProviderConfiguration/ResourceProviderPlugins">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='Composite.Plugins.FormsRenderer'])=0">
				<add name="Composite.Plugins.FormsRenderer" defaultCultureName="en-US" type="Composite.Plugins.ResourceSystem.XmlStringResourceProvider.XmlStringResourceProvider, Composite">
					<Cultures>
						<add cultureName="en-US" xmlFile="~/Frontend/Composite/Forms/Renderer/Localization/Composite.FormsRenderer.en-us.xml" monitorFileChanges="true" />
					</Cultures>
				</add>
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.Functions.Plugins.FunctionProviderConfiguration/FunctionProviderPlugins">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='FormsRendererFunctionProvider'])=0">
				<add type="Composite.Forms.Renderer.FormsRendererFunctionProvider.FormsRendererFunctionProvider, Composite.Forms.Renderer" name="FormsRendererFunctionProvider" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>