<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ui="http://www.w3.org/1999/xhtml">

    <!-- Autor: JamBo - nu.Faqtz.com -->
    <xsl:template match="/">
        <ui:tree focusable="false">
            <ui:treebody>
                <xsl:if test="ActionItems/Page">
                    <xsl:apply-templates select="ActionItems/Page"/>
                </xsl:if>
            </ui:treebody>
        </ui:tree>
    </xsl:template>
    
    <xsl:template match="Page">
      
      <xsl:variable name="iconname">
        <xsl:choose>
          <xsl:when test="@Status='published'">page</xsl:when>
          <xsl:when test="@Status='awaitingApproval'">page-awaiting-approval</xsl:when>
          <xsl:when test="@Status='awaitingPublication'">page-awaiting-publication</xsl:when>
          <xsl:when test="@Status='draft'">page-draft</xsl:when>
        </xsl:choose>
      </xsl:variable>

      <ui:treenode open="true" label="{@Title}" image="${{icon:{$iconname}}}">
        <xsl:if test="count(descendant::invalidContent|renderingError) = 0">
          <xsl:attribute name="open">false</xsl:attribute>
        </xsl:if>

        <xsl:if test="invalidContent">


          <xsl:for-each select="invalidContent">
            <ui:treenode image="${{icon:unlink}}" >
              <xsl:attribute name="label">
                <xsl:value-of select="@originalText" />
                
                <xsl:if test="@errorType != ''">
                  ( <xsl:value-of select="@errorType" /> )
                </xsl:if>

              </xsl:attribute>

            </ui:treenode>

          </xsl:for-each>

        </xsl:if>

        <xsl:if test="renderingError">
          <ui:treenode label="{renderingError/@message}" image="${{icon:warning}}" />
        </xsl:if>

        <xsl:if test="Page">
          <xsl:apply-templates select="Page"/>
        </xsl:if>
        </ui:treenode>
    </xsl:template>

  <!--xsl:template mode="Render" match="*">
        <ui:li class="invalidContent">
          <img src="../../../../services/Icon/GetIcon.ashx?resourceNamespace=Composite.Icons&amp;resourceName=unlink" />

          <span class="previousNode">
                <xsl:value-of select="@previousNode"/>
            </span>
            <a href="{@originalLink}" target="blank" title="{@originalLink}" class="invalidLink">
              <xsl:value-of select="@originalText"/>
            </a>
            <span class="nextNode">
                <xsl:value-of select="@nextNode"/>
            </span>
            <span class="errorType">
                (<xsl:value-of select="@errorType"/>)
            </span>
        </ui:li>
    </xsl:template-->

</xsl:stylesheet>