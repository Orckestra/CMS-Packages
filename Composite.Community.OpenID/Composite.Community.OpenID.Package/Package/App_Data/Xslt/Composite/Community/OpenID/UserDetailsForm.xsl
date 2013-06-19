<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:c1="http://c1.composite.net/StandardFunctions"
	xmlns:cod="#OpenIDExtensions"
	exclude-result-prefixes="xsl in lang f c1 cod">

  <xsl:variable name="signInPage" select="/in:inputs/in:param[@name='SignInPage']" />
  <xsl:variable name="isAuthenticated" select="cod:IsAuthenticated()" />

  <xsl:template match="/">
    <html>
      <head>
        <link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Community/OpenID/Styles.css" />
      </head>

      <body>
        <asp:form xmlns:asp="http://www.composite.net/ns/asp.net/controls">
          <xsl:if test="$isAuthenticated != 'true'">
            <xsl:value-of select="cod:Redirect($signInPage)" />
          </xsl:if>
          <xsl:call-template name="UserDetails" />
        </asp:form>
      </body>
    </html>
  </xsl:template>

  <xsl:template name="UserDetails">
    <xsl:variable name="submitted" select="c1:GetFormData('submit')" />
    <xsl:variable name="cssFieldError" select="'FieldError'" />
    <xsl:variable name="save" select="cod:UserDetailsHelper()" />
    <xsl:variable name="error" select="$save/Error" />
    <xsl:variable name="data" select="$save/Data" />

    <xsl:choose>
      <xsl:when test="count($error) = 0 and $submitted = 'true'">
        <xsl:value-of select="cod:GetLocalized('UserDetailsForm','Updated')" />
      </xsl:when>
      <xsl:otherwise>
        <div id="Input">
          <input type="hidden" name="submit" id="submit" value="true" />
          <h4>
            <xsl:value-of select="cod:GetLocalized('UserDetailsForm','userDetails')" />
          </h4>
          <xsl:if test="count($error)>0">
            <div class="ErrorsHeader">
              <xsl:value-of select="cod:GetLocalized('UserDetailsForm','ErrorsHeader')" />
            </div>
            <ul id="ErrorBox">
              <xsl:for-each select="$error">
                <li>
                  <xsl:value-of select="@*" />
                </li>
              </xsl:for-each>
            </ul>
          </xsl:if>

          <ul id="InputForm">
            <li>
              <label for="displayNamer">
                <xsl:value-of select="cod:GetLocalized('UserDetailsForm','DisplayName')" />
              </label>
              <input type="text" name="displayName" id="displayName" value="{$data/@DisplayName}" maxlength="64">
                <xsl:if test="count($error/@DisplayName)>0">
                  <xsl:attribute name="class">
                    <xsl:value-of select="$cssFieldError" />
                  </xsl:attribute>
                </xsl:if>
              </input>
            </li>
            <li>
              <label for="fromEmail">
                <xsl:value-of select="cod:GetLocalized('UserDetailsForm','Email')" />
              </label>
              <input type="text" name="email" id="email" value="{$data/@Email}" maxlength="128">
                <xsl:if test="count($error/@Email)>0">
                  <xsl:attribute name="class">
                    <xsl:value-of select="$cssFieldError" />
                  </xsl:attribute>
                </xsl:if>
              </input>
            </li>
            <li>
              <label for="realName">
                <xsl:value-of select="cod:GetLocalized('UserDetailsForm','RealName')" />
              </label>
              <input type="text" name="realName" id="realName" value="{$data/@RealName}" maxlength="64">
                <xsl:if test="count($error/@RealName)>0">
                  <xsl:attribute name="class">
                    <xsl:value-of select="$cssFieldError" />
                  </xsl:attribute>
                </xsl:if>
              </input>
            </li>
            <li>
              <label for="website">
                <xsl:value-of select="cod:GetLocalized('UserDetailsForm','Website')" />
              </label>
              <input type="text" name="website" id="website" value="{$data/@Website}" maxlength="128">
                <xsl:if test="count($error/@Website)>0">
                  <xsl:attribute name="class">
                    <xsl:value-of select="$cssFieldError" />
                  </xsl:attribute>
                </xsl:if>
              </input>
            </li>
            <li>
              <label for="location">
                <xsl:value-of select="cod:GetLocalized('UserDetailsForm','Location')" />
              </label>
              <input type="text" name="location" id="location" value="{$data/@Location}" maxlength="128">
                <xsl:if test="count($error/@Location)>0">
                  <xsl:attribute name="class">
                    <xsl:value-of select="$cssFieldError" />
                  </xsl:attribute>
                </xsl:if>
              </input>
            </li>
            <li>
              <label for="aboutMe">
                <xsl:value-of select="cod:GetLocalized('UserDetailsForm','AboutMe')" />
              </label>
              <textarea name="aboutMe" id="aboutMe">
                <xsl:value-of select="$data/@AboutMe" />
              </textarea>
            </li>
            <li>
              <input class="submit" type="submit" value="{cod:GetLocalized('UserDetailsForm','Submit')}" />
            </li>
          </ul>
        </div>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>
