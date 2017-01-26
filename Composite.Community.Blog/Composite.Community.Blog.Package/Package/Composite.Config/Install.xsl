<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/configuration/Composite.Data.Plugins.DataProviderConfiguration/DataProviderPlugins">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="count(add[@name='InstalledPackagesFormFileProvider'])=0">
        <add rootDirectory="~/Composite/InstalledPackages/content/forms" fileSearchPattern="*.xml" topDirectoryOnly="false" fileInterfaceType="Composite.C1Console.Forms.DataServices.IFormDefinitionFile, Composite, Version=1.0.3037.13741, Culture=neutral, PublicKeyToken=null" type="Composite.Plugins.Data.DataProviders.FileSystemDataProvider.FileSystemDataProvider, Composite, Version=1.0.3037.13741, Culture=neutral, PublicKeyToken=null" name="InstalledPackagesFormFileProvider" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>

    <xsl:template match="/configuration/Composite.C1Console.Forms.Plugins.UiControlFactoryConfiguration/Channels/Channel[@name='AspNet.Management']/Namespaces/Namespace[@name='http://www.composite.net/ns/management/bindingforms/std.ui.controls.lib/1.0']/Factories">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()" />
            <xsl:if test="count(add[@name='BlogTagMultiSelector'])=0">
                <add userControlVirtualPath="~/Composite/InstalledPackages/controls/FormsControls/Composite.Community.Blog/BlogTagMultiSelector.ascx" name="BlogTagMultiSelector" cacheCompiledUserControlType="false" type="Composite.Plugins.Forms.WebChannel.UiControlFactories.UserControlBasedUiControlFactory, Composite" />
            </xsl:if>
        </xsl:copy>
    </xsl:template>
</xsl:stylesheet>