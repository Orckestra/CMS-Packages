<?xml version="1.0"?>
<xsl:stylesheet version="1.0"
								xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
								xmlns="http://www.w3.org/1999/xhtml"
								xmlns:x="http://www.w3.org/1999/xhtml"
								exclude-result-prefixes="x"
								xmlns:f="http://www.composite.net/ns/function/1.0"
								>
  <xsl:output indent="yes"  method="html" omit-xml-declaration="yes"/>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/x:html/x:body">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()" />
      <xsl:if test="count(f:function[@name='Composite.Web.Html.CountrySpecificContent'])=0">
        <f:function name="Composite.Web.Html.CountrySpecificContent" xmlns:f="http://www.composite.net/ns/function/1.0" >
          <f:param name="Countries">
            <f:paramelement value="GB" />
            <f:paramelement value="SE" />
            <f:paramelement value="ES" />
            <f:paramelement value="SI" />
            <f:paramelement value="SK" />
            <f:paramelement value="RO" />
            <f:paramelement value="PT" />
            <f:paramelement value="PL" />
            <f:paramelement value="NL" />
            <f:paramelement value="MT" />
            <f:paramelement value="LU" />
            <f:paramelement value="LT" />
            <f:paramelement value="LV" />
            <f:paramelement value="IT" />
            <f:paramelement value="IE" />
            <f:paramelement value="HU" />
            <f:paramelement value="GR" />
            <f:paramelement value="DE" />
            <f:paramelement value="FR" />
            <f:paramelement value="FI" />
            <f:paramelement value="EE" />
            <f:paramelement value="DK" />
            <f:paramelement value="CZ" />
            <f:paramelement value="HR" />
            <f:paramelement value="BG" />
            <f:paramelement value="BE" />
            <f:paramelement value="AT" />
          </f:param>
          <f:param name="Content">
            <f:function name="Composite.Web.Html.AcceptAlert" xmlns:f="http://www.composite.net/ns/function/1.0">>
              <f:param name="Content">
                <html>
                  <head>
                  </head>
                  <body>
                    <h2>
                      <span class="glyphicon glyphicon-warning-sign text-primary"></span> We use cookies
                    </h2>
                    <p>
                      We place cookies on your computer to remember your settings and understand how people use our website. By using this site you accept this.&#160;<a href="~/page(51aef503-816d-469f-9a54-cfb8b0033628)">
                        <u>Read more</u>
                      </a>.
                    </p>
                  </body>
                </html>
              </f:param>
            </f:function>
          </f:param>
        </f:function>
      </xsl:if>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>