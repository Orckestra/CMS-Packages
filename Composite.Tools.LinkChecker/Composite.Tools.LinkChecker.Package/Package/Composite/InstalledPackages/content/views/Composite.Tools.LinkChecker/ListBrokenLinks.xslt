<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:ui="http://www.w3.org/1999/xhtml">

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
      
      <ui:treenode open="true" label="{@Title}" image="${{icon:page}}">
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

                &quot; <xsl:value-of select="@originalLink" /> &quot;

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

</xsl:stylesheet>