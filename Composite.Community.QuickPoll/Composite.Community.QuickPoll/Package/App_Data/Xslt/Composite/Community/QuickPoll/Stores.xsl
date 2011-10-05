<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	exclude-result-prefixes="xsl in">

	<xsl:param name="question" select="/in:inputs/in:param[@name='Question']" />
	<xsl:param name="chartType" select="/in:inputs/in:param[@name='ChartType']" />

	<xsl:template match="/" >
		<span>
			<f:function name="Composite.Community.QuickPoll.Results">
				<f:param name="Question" value="{$question}" />
				<f:param name="ChartType" value="{$chartType}" />
			</f:function>
		</span>
	</xsl:template>

</xsl:stylesheet>
