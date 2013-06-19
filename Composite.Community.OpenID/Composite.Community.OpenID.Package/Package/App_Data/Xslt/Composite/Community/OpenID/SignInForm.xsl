<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:cod="#OpenIDExtensions"
	exclude-result-prefixes="xsl in lang f cod">

  <xsl:variable name="signOut" select="/in:inputs/in:result[@name='signOut']" />
  <xsl:variable name="userDetailsPage" select="/in:inputs/in:param[@name='UserDetailsPage']" />

  <xsl:template match="/">
    <html>
      <head>
        <link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Community/OpenID/Styles.css" />
        <script id="jquery-js" src="//code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
        <script type="text/javascript" src="~/Frontend/Composite/Community/OpenID/Scripts/openid-jquery.js" />
        <script type="text/javascript">
          $(document).ready(function() {
          openid.init('openid_identifier');
          $("#openid_identifier").focus();
          });
        </script>
      </head>

      <body>
        <asp:form xmlns:asp="http://www.composite.net/ns/asp.net/controls">
          <xsl:if test="$signOut='true'">
            <xsl:value-of select="cod:SignOut()" />
          </xsl:if>
          <xsl:value-of select="cod:SignIn($userDetailsPage)" />
          <xsl:call-template name="OpenIdForm" />
        </asp:form>
      </body>
    </html>
  </xsl:template>

  <xsl:template name="OpenIdForm">
    <div id="Input">
      <div id="openid_choice">
        <p>
          <xsl:copy-of select="cod:GetLocalized('SignInForm','ClickYourOpenID')" />
        </p>
        <div id="openid_btns"></div>
      </div>
      <ul id="InputForm">
        <div id="openid_input_area"></div>
        <li>
          <label for="realName">
            <xsl:value-of select="cod:GetLocalized('SignInForm','ManuallyEnter')" />
          </label>
          <input id="openid_identifier" name="openid_identifier" class="openid-identifier" type="text" tabindex="100" />
        </li>
        <li>
          <input class="submit" id="submit-button" type="submit" value="{cod:GetLocalized('SignInForm','SignIn')}" tabindex="101" />
        </li>
      </ul>
    </div>
  </xsl:template>

</xsl:stylesheet>
