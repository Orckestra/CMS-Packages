<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
  <xsl:variable name="IdDivSilverlightControlHost" select="/in:inputs/in:param[@name='IdDivSilverlightControlHost']/text()" />
  <xsl:variable name="IdObjectSilverlightControlHost" select="/in:inputs/in:param[@name='IdObjectSilverlightControlHost']/text()" />
  <xsl:variable name="SourceXAP" select="/in:inputs/in:param[@name='SourceXAP']/text()" />
  <xsl:variable name="Data" select="/in:inputs/in:param[@name='Data']/text()" />
  <xsl:variable name="Type" select="/in:inputs/in:param[@name='Type']/text()" />
  <xsl:variable name="Width" select="/in:inputs/in:param[@name='Width']/text()" />
  <xsl:variable name="Height" select="/in:inputs/in:param[@name='Height']/text()" />
  <xsl:variable name="JSErrorHandler" select="/in:inputs/in:param[@name='JSErrorHandler']/text()" />
  <xsl:variable name="Background" select="/in:inputs/in:param[@name='Background']/text()" />
  <xsl:variable name="RuntimeMin" select="/in:inputs/in:param[@name='RuntimeMin']/text()" />
  <xsl:variable name="UpgradeAutomatic" select="/in:inputs/in:param[@name='UpgradeAutomatic']/text()" />
  <xsl:variable name="AllowHtmlPopupWindow" select="/in:inputs/in:param[@name='AllowHtmlPopupWindow']/text()" />
  <xsl:variable name="EnableHTMLAccess" select="/in:inputs/in:param[@name='EnableHTMLAccess']/text()" />
  <xsl:variable name="InitParams" select="/in:inputs/in:param[@name='InitParams']/text()" />
  <xsl:variable name="MaxFrameRate" select="/in:inputs/in:param[@name='MaxFrameRate']/text()" />
  <xsl:variable name="JSOnFullScreenChanged" select="/in:inputs/in:param[@name='JSOnFullScreenChanged']/text()" />
  <xsl:variable name="JSOnLoad" select="/in:inputs/in:param[@name='JSOnLoad']/text()" />
  <xsl:variable name="JSOnResize" select="/in:inputs/in:param[@name='JSOnResize']/text()" />
  <xsl:variable name="JSOnSourceDownloadComplete" select="/in:inputs/in:param[@name='JSOnSourceDownloadComplete']/text()" />
  <xsl:variable name="JSOnSourceDownloadProgressChanged" select="/in:inputs/in:param[@name='JSOnSourceDownloadProgressChanged']/text()" />
  <xsl:variable name="SplashScreenSource" select="/in:inputs/in:param[@name='SplashScreenSource']/text()" />
  <xsl:variable name="Windowless" select="/in:inputs/in:param[@name='Windowless']/text()" />
  <xsl:template match="/">
    <html>
      <head>
    <!-- markup placed here will be shown in the head section of the rendered page -->
        <!-- 
        Silverlight javascript can be loaded at the end of the html (just before closing body tag)
        in order to avoid blocking.
        It is recomde
        1. Put it (merge) in a sinlge javascript file to reduce requests post script 
        2. minify-obfuscate-compress it for 
        3. add some kind of tag in the filename (timestamp) to allow caching
     -->
        <script id="silverlight-errorHandling" type="text/javascript" src="/Frontend/Contribution/HolisticWare/SilverlightPlayer/Silverlight.ErrorHandling.uncompressed.20110318.js"></script>
        <script id="silverlight-js" type="text/javascript" src="/Frontend/Contribution/HolisticWare/SilverlightPlayer/Silverlight.js"></script>
      </head>
      <body>
        <div id="{$IdDivSilverlightControlHost}">
          <object id="{$IdObjectSilverlightControlHost}" data="{$Data}" type="{$Type}" width="{$Width}" height="{$Height}">
            <param name="source" value="{$SourceXAP}" />
            <param name="onError" value="{$JSErrorHandler}" />
            <param name="background" value="{$Background}" />
            <param name="minRuntimeVersion" value="{$RuntimeMin}" />
            <param name="autoUpgrade" value="{$UpgradeAutomatic}" />
            <param name="allowHtmlPopupWindow" value="{$AllowHtmlPopupWindow}" />
            <param name="enablehtmlaccess" value="{$EnableHTMLAccess}" />
            <param name="initParams" value="{$InitParams}" />
            <param name="maxframerate" value="{$MaxFrameRate}" />
            <param name="onFullScreenChanged" value="{$JSOnFullScreenChanged}" />
            <param name="onLoad" value="{$JSOnLoad}" />
            <param name="onResize" value="{$JSOnResize}" />
            <param name="onSourceDownloadComplete" value="{$JSOnSourceDownloadComplete}" />
            <param name="onSourceDownloadProgressChanged" value="{$JSOnSourceDownloadProgressChanged}" />
            <xsl:if test="$SplashScreenSource != ''">
              <param name="splashScreenSource" value="{$SplashScreenSource}" />
            </xsl:if>
            <param name="windowless" value="{$Windowless}" />
            <a href="http://go.microsoft.com/fwlink/?LinkID=149156&amp;v=4.0.50401.0" style="text-decoration: none;">
              <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" style="border-style: none" />
            </a>
          </object>
          <iframe id="_sl_historyFrame" style="visibility:hidden;height:0px;width:0px;border:0px"></iframe>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>