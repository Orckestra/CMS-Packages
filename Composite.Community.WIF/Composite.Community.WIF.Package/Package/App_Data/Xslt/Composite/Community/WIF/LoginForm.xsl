<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
  xmlns:lang="http://www.composite.net/ns/localization/1.0"
  xmlns:f="http://www.composite.net/ns/function/1.0"
  xmlns:c1="http://c1.composite.net/StandardFunctions"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
  xmlns:csharp="http://c1.composite.net/sample/csharp"
  xmlns="http://www.w3.org/1999/xhtml"
  exclude-result-prefixes="xsl in lang f msxsl csharp">
    
  <xsl:variable name="IsLoggedIn" select="count(/in:inputs/in:result[@name='IsLoggedIn'][text()='true']) > 0" />
  <xsl:variable name="UserName" select="/in:inputs/in:result[@name='GetName']/text()" />
    
  <xsl:template match="/">
    <html>
      <head>
      </head>

      <body>
        <form method="post">
          
              <xsl:choose>
                <xsl:when test="$IsLoggedIn">
                   Logged in as <xsl:value-of select="$UserName" />
                  
                   <input type="submit" name="wif_logout" value="Log out" />
                </xsl:when>
                <xsl:otherwise>
        
                   <input type="submit" name="wif_login" value="Log in" />
                </xsl:otherwise>
              </xsl:choose>
          
         </form>
        
        <xsl:if test="c1:GetFormData('wif_login') != ''">
          <xsl:copy-of select="csharp:RedirectToLogin()" />
        </xsl:if>
        
        <xsl:if test="c1:GetFormData('wif_logout') != ''">
          <xsl:copy-of select="csharp:LogOut()" />
        </xsl:if>

        <xsl:copy-of select="csharp:DisableAspNetCache()" />
      </body>
    </html>
  </xsl:template>

  <msxsl:script implements-prefix="csharp" language="C#"> 
     
    <msxsl:assembly name="App_Code" /> 
      
    <![CDATA[
        public static string RedirectToLogin()
        {
            Composite.Community.WIF.IdentityFunctions.RedirectToLogin();
            return string.Empty;
        }

        public static string LogOut()
        {
            Composite.Community.WIF.IdentityFunctions.LogOut();
            return string.Empty;
        }

        public static string DisableAspNetCache()
        {
            Composite.Community.WIF.IdentityFunctions.DisableAspNetCache();
            return string.Empty;
        }
    ]]>
   </msxsl:script> 
    
   
    
</xsl:stylesheet>
