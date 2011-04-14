<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="xsl in">

	<xsl:param name="submitTitle" select="/in:inputs/in:param[@name='SubmitTitle']" />
	<xsl:param name="defaultQuestion" select="'00000000-0000-0000-0000-000000000000'" />
	<xsl:param name="selectedQuestion" select="/in:inputs/in:param[@name='Question']" />
	<xsl:variable name="question" select="/in:inputs/in:result[@name='GetQuickPollQuestionXml']/Question[$defaultQuestion = $selectedQuestion or @Id = $selectedQuestion][position() = last()]" />
	<xsl:variable name="isPieChartResult" select="/in:inputs/in:param[@name='PieChartResult']" />
	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Community/QuickPoll/Styles.css" />
			</head>
			<body>
				<xsl:if test="string($question/@Id) != ''">
					<f:function name="Composite.Community.QuickPoll.Master">
						<f:param name="Question" value="{$question/@Id}" />
						<f:param name="SubmitTitle" value="{$submitTitle}" />
						<f:param name="PieChartResult" value="{$isPieChartResult}" />
					</f:function>
				</xsl:if>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>