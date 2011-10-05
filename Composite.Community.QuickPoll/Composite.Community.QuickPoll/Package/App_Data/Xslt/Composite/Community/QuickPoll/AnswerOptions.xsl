<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="xsl in">
	<xsl:param name="submitTitle" select="/in:inputs/in:param[@name='SubmitTitle']" />
	<xsl:template match="/">
		<html>
			<head>
			</head>
			<body>
				<h1>
					<xsl:value-of select="/in:inputs/in:result[@name='GetQuestionXml']/Question/@QuestionText"/>
				</h1>
				<xsl:variable name="sumTotalVotes" select="sum(/in:inputs/in:result[@name='GetQuickPollAnswersXml']/Answers/@TotalVotes)" />
				<xsl:for-each select="/in:inputs/in:result[@name='GetQuickPollAnswersXml']/Answers">
					<div>
						<input type="radio" value="{@Id}" name="QuickPollVote" id="option{@Id}" />
						<label for="option{@Id}">
							<xsl:value-of select="@AnswerText" />
						</label>
					</div>
				</xsl:for-each>
				<input id="CompositeQuickPoll" type="submit" value="{$submitTitle}" />
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>