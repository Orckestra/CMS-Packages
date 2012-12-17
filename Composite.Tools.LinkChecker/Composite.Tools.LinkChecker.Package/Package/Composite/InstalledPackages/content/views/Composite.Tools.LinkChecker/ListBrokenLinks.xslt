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
        <ui:treenode open="true">
          <xsl:if test="count(descendant::invalidContent) = 0">
            <xsl:attribute name="open">false</xsl:attribute>
          </xsl:if>
          
            <xsl:variable name="iconname">
                <xsl:choose>
                    <xsl:when test="@Status='published'">page</xsl:when>
                    <xsl:when test="@Status='awaitingApproval'">page-awaiting-approval</xsl:when>
                    <xsl:when test="@Status='awaitingPublication'">page-awaiting-publication</xsl:when>
                    <xsl:when test="@Status='draft'">page-draft</xsl:when>
                </xsl:choose>
            </xsl:variable>
            <xsl:attribute name="label">
                <xsl:value-of select="@Title"/>
            </xsl:attribute>
            <xsl:attribute name="image">
                <xsl:text>${icon:</xsl:text>
                <xsl:value-of select="$iconname"/>
                <xsl:text>}</xsl:text>
            </xsl:attribute>
            <xsl:if test="invalidContent">
                <ui:ul class="linkCheckInvalid">
                    <xsl:apply-templates mode="Render" select="invalidContent" />
                </ui:ul>
            </xsl:if>
            <xsl:if test="PageFolder">
                <xsl:apply-templates select="PageFolder"/>
            </xsl:if>
            <xsl:if test="Page">
                <xsl:apply-templates select="Page"/>
            </xsl:if>
        </ui:treenode>
    </xsl:template>
    
    <xsl:template match="PageFolder">
        <ui:treenode open="true" label="{@Title}">
            <xsl:attribute name="image">${icon:generated-interface-open}</xsl:attribute>
        </ui:treenode>
    </xsl:template>

    <xsl:template mode="Render" match="*">
        <ui:li class="invalidContent">
            <span class="previousNode">
                <xsl:value-of select="@previousNode"/>
            </span>
            <xsl:element name="a">
                <xsl:attribute name="href">
                    <xsl:text>#</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="title">
                    <xsl:value-of select="@originalLink"/>
                </xsl:attribute>
                <xsl:attribute name="class">
                    <xsl:text>invalidLink</xsl:text>
                </xsl:attribute>
                <xsl:value-of select="@originalText"/>
            </xsl:element>
            <span class="nextNode">
                <xsl:value-of select="@nextNode"/>
            </span>
            <span class="errorType">
                (<xsl:value-of select="@errorType"/>)
            </span>
        </ui:li>
    </xsl:template>

</xsl:stylesheet>