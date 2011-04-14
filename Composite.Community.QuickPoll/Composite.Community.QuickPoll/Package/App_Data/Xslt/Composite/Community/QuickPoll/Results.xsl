<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="xsl in">

	<xsl:param name="questionId" select="/in:inputs/in:param[@name='Question']" />
	<xsl:param name="items" select="/in:inputs/in:result[@name='GetQuickPollAnswersXml']/Answers" />
	<xsl:param name="barLength" select="/in:inputs/in:param[@name='BarLength']" />
	<xsl:param name="isPieChartResult" select="/in:inputs/in:param[@name='PieChartResult']" />

	<xsl:template match="/">
		<html>
			<head>
			</head>
			<body>
				<xsl:choose>
					<xsl:when test="$isPieChartResult='true'">
						<f:function xmlns:f="http://www.composite.net/ns/function/1.0" name="Composite.Community.QuickPoll.LoadPieChart">
							<f:param name="questionId" value="{$questionId}" />
						</f:function>
					</xsl:when>
					<xsl:otherwise>
						<xsl:variable name="votes" select="sum($items/@TotalVotes)" />
						<ul class="Answers">
							<xsl:for-each select="$items">
								<xsl:variable name="percent">
									<xsl:if test="$votes &gt; 0">
										<xsl:value-of select="(100*@TotalVotes - (100*@TotalVotes mod $votes)) div $votes" />
									</xsl:if>
									<xsl:if test="$votes = 0">
										0
									</xsl:if>
								</xsl:variable>
								<li>
									<xsl:value-of select="@AnswerText" />
									<div class='PercentageElement' style='width:{(($percent*$barLength - ($percent*$barLength mod 100)) div 100)+1}px;height:20px'></div>
									<xsl:value-of select="$percent" /> %
								</li>
							</xsl:for-each>
						</ul>
						Voters: <xsl:value-of select="$votes" />
					</xsl:otherwise>
				</xsl:choose>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>