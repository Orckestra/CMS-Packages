<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:df="#dateExtensions"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:user="urn:my-scripts"
	exclude-result-prefixes="xsl in df msxsl user">

	<xsl:param name="displayMode" select="/in:inputs/in:param[@name='DisplayMode']" />

	<msxsl:script language="C#" implements-prefix="user">
	<msxsl:assembly name="System.Web"/>
	<![CDATA[
		public int CompareDate(string date)
		{
			return DateTime.Compare(DateTime.Parse(date),  DateTime.Now);
		}
	]]>
	</msxsl:script>

	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Lists/EventCalendar/Style.css" />
			</head>
			<body>
				<xsl:variable name="eventlist">
					<xsl:choose>
						<xsl:when test="$displayMode='true'">
							<xsl:copy-of select="/in:inputs/in:result[@name='GetEventCalendarXml']/*[user:CompareDate(@EndDate) &gt; 0]" />
						</xsl:when>
						<xsl:otherwise>
							<xsl:copy-of select="/in:inputs/in:result[@name='GetEventCalendarXml']/*[user:CompareDate(@EndDate) &lt; 0]" />
						</xsl:otherwise>
					</xsl:choose>
				</xsl:variable>

				<xsl:choose>
					<xsl:when test="count(msxsl:node-set($eventlist)/*)&gt;0">
						<xsl:apply-templates select="msxsl:node-set($eventlist)/*" />
					</xsl:when>
					<xsl:otherwise>
						<p>There are no Event entries published yet.</p>
					</xsl:otherwise>
				</xsl:choose>
			</body>
		</html>
	</xsl:template>

	<xsl:template match="EventCalendar">
		<div class="ListItem">
			<div>
				<a href="~/Renderers/Page.aspx?DateView={@Id}&amp;pageId={@PageId}">
					<xsl:value-of select="@Title" />
				</a>
			</div>
			<small class="StartDate">
				<xsl:value-of select="df:ShortDateFormat(@StartDate)" />
			</small>
			 - 
			<small class="EndDate">
				<xsl:value-of select="df:ShortDateFormat(@EndDate)" />
			</small>
		</div>
		<div>
			<xsl:value-of select="@ShortDescription" />
		</div>
	</xsl:template>

</xsl:stylesheet>