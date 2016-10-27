<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:df="#dateExtensions"
	xmlns:mp="#MarkupParserExtensions"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:user="urn:my-scripts"
	exclude-result-prefixes="xsl in df msxsl user mp">

	<xsl:param name="DateView" select="/in:inputs/in:param[@name='DateView']" />
	<xsl:param name="past" select="/in:inputs/in:result[@name='QueryStringValue']" />
	<xsl:param name="NumberOfEvents" select="/in:inputs/in:param[@name='NumberOfEvents']" />

	<msxsl:script language="C#" implements-prefix="user">
	<msxsl:assembly name="System.Web"/>
	<![CDATA[
		public int CompareDate(string date)
		{
			return DateTime.Compare(DateTime.Parse(date), DateTime.Now);
		}
		]]>
	</msxsl:script>

	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Lists/EventCalendar/Style.css" />
			</head>
			<body>
				<div class="EventCalendar">
					<xsl:choose>
						<xsl:when test="$DateView=''">
							<xsl:variable name="eventlist">
								<xsl:choose>
									<xsl:when test="$past='true'">
										<xsl:copy-of select="/in:inputs/in:result[@name='GetEventCalendarXml']/*[user:CompareDate(@EndDate) &lt; 0]" />
									</xsl:when>
									<xsl:otherwise>
										<xsl:copy-of select="/in:inputs/in:result[@name	='GetEventCalendarXml']/*[not(user:CompareDate(@EndDate) &lt; 0)]" />
									</xsl:otherwise>
								</xsl:choose>
							</xsl:variable>

							<xsl:choose>
								<xsl:when test="count(msxsl:node-set($eventlist)/*)&gt;0">
									<xsl:if test="$NumberOfEvents=0">
										<xsl:apply-templates  select="msxsl:node-set($eventlist)/*" />
									</xsl:if>
									<xsl:if test="not($NumberOfEvents=0)">
										<xsl:apply-templates  select="msxsl:node-set($eventlist)/*[(position()&lt;$NumberOfEvents+1)]" />
									</xsl:if>
								</xsl:when>
								<xsl:otherwise>
									<p>There are no Event entries published yet.</p>
								</xsl:otherwise>
							</xsl:choose>

							<xsl:choose>
								<xsl:when test="$past='true'">
									<a class="Archive" href="?past=false">Current and Coming Events</a>
								</xsl:when>
								<xsl:otherwise>
									<a class="Archive" href="?past=true">Past Events</a>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:when>
						<xsl:otherwise>
							<xsl:apply-templates select="/in:inputs/in:result[@name='GetEventCalendarXml']/*[@Id = $DateView]" mode="Description" />
						</xsl:otherwise>
					</xsl:choose>
				</div>

			</body>
		</html>
	</xsl:template>

	<xsl:template match="EventCalendar" mode="Description">
		<div class="Item">
			<h2 class="Title">
				<xsl:value-of select="@Title" />
			</h2>
			<small class="StartDate">
				<xsl:value-of select="df:ShortDateFormat(@StartDate)" />
			</small>
			 - 
			<small class="EndDate">
				<xsl:value-of select="df:ShortDateFormat(@EndDate)" />
			</small>
			<div>
				<xsl:value-of select="@ShortDescription" />
			</div>
			<div>
				<xsl:copy-of select="mp:ParseXhtmlBodyFragment(@Description)" />
			</div>
		</div>
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