<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
xmlns:lang="http://www.composite.net/ns/localization/1.0"
xmlns:f="http://www.composite.net/ns/function/1.0"
xmlns="http://www.w3.org/1999/xhtml"
xmlns:msxsl="urn:schemas-microsoft-com:xslt"
xmlns:csharp="http://c1.composite.net/sample/csharp"
exclude-result-prefixes="xsl in lang f csharp msxsl">
  <xsl:param name="content" select="/in:inputs/in:param[@name='Content']" />
  <xsl:param name="placeholderId" select="/in:inputs/in:param[@name='PlaceholderId']" />
  <xsl:param name="editingEnabled" select="/in:inputs/in:param[@name='EditingEnabled']" />
  <xsl:param name="contentCss" select="/in:inputs/in:param[@name='ContentCss']" />
  <xsl:variable name="currPageId" select="/in:inputs/in:result[@name='GetPageId']" />
  <xsl:template match="/">
    <html>
      <head>
        <xsl:if test="$editingEnabled='true'">
		<link id="simplewili-css" media="screen" type="text/css" href="/Frontend/Composite/Community/Wiki/SimpleWiki/Styles/Styles.css" rel="stylesheet" />
		<script id="jquery-js" src="//code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
		<script id="tiny_mce-js" type="text/javascript" src="/Frontend/Composite/Community/Wiki/SimpleWiki/tinymce/jscripts/tiny_mce/tiny_mce.js"></script>
		<script id="simplewiki-js" type="text/javascript" src="/Frontend/Composite/Community/Wiki/SimpleWiki/Scripts/simplewiki.js"></script>
		<script id="init-simplewiki" type="text/javascript">
            $().ready(function() {
            init('<xsl:value-of select="$contentCss"/>');
            })
          </script>
        </xsl:if>
      </head>
      <body>
        <xsl:choose>
          <xsl:when test="$editingEnabled='true'">
            <div id="contentReal">
              <div>
                <xsl:copy-of select="/in:inputs/in:param[@name='Content']/*" />
              </div>
              <a class="editthisPage" href="javascript:setup();">EDIT THIS PAGE</a>
              <xsl:value-of select="csharp:NoCache()" />
            </div>
            <div id="contentMceEditor" style="display:none;">
              <textarea id="editablePageContent" name="editablePageContent" rows="45" cols="80" style="width: 95%;" class="mceEditor">
              </textarea>
              <br />
              <a class="savethisPage" href="javascript:saveThisPage('{$currPageId}', '{$placeholderId}');">SAVE</a>
              <a class="cancelSavePage" href="javascript:cancelSave();">CANCEL</a>
            </div>
          </xsl:when>
          <xsl:otherwise>
            <xsl:copy-of select="/in:inputs/in:param[@name='Content']/*" />
          </xsl:otherwise>
        </xsl:choose>
      </body>
    </html>
  </xsl:template>
  <msxsl:script implements-prefix="csharp" language="C#">
    <msxsl:assembly name="System.Web" />
    <msxsl:using namespace="System.Web" />
    public void NoCache()
    {
      HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
    }
  </msxsl:script>
</xsl:stylesheet>