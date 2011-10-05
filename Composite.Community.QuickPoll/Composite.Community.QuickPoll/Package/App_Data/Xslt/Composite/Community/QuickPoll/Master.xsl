<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:asp="http://www.composite.net/ns/asp.net/controls"
	xmlns:msxsl="urn:schemas-microsoft-com:xslt"
	xmlns:csharp="http://c1.composite.net/sample/csharp"
	exclude-result-prefixes="xsl in msxsl csharp">

	<xsl:param name="submitTitle" select="/in:inputs/in:param[@name='SubmitTitle']" />
	<xsl:param name="question" select="/in:inputs/in:param[@name='Question']" />
	<xsl:param name="userVote" select="string(/in:inputs/in:result[@name='GetQuickPollAnswersXml']/Answers[@Id = /in:inputs/in:result[@name='UserVote']]/@Id)" />
	<xsl:param name="userHasVoted" select="/in:inputs/in:result[@name='UserHasVoted']" />
	<xsl:param name="questionText" select="/in:inputs/in:result[@name='GetQuickPollQuestionXml']/Question/@QuestionText" />
	<xsl:param name="isPieChartResult" select="/in:inputs/in:param[@name='PieChartResult']" />
	<xsl:template match="/">
		<html>
			<head>
			</head>
			<body>
				<asp:form>
					<div class="QuickPoll">
						<h1>
							<xsl:value-of select="$questionText" />
						</h1>
						<xsl:value-of select="csharp:SetNoCache()" />
						<xsl:choose>
							<xsl:when test="$userHasVoted='0'">
								<xsl:choose>
									<xsl:when test="$userVote=''">
										<f:function name="Composite.Community.QuickPoll.AnswerOptions">
											<f:param name="Question" value="{$question}" />
											<f:param name="SubmitTitle" value="{$submitTitle}" />
										</f:function>
									</xsl:when>
									<xsl:otherwise>
										<f:function name="Composite.Community.QuickPoll.Stores">
											<f:param name="Question" value="{$question}" />
											<f:param name="Answer" value="{$userVote}" />
											<f:param name="PieChartResult" value="{$isPieChartResult}" />
										</f:function>
									</xsl:otherwise>
								</xsl:choose>
							</xsl:when>
							<xsl:otherwise>
								<f:function name="Composite.Community.QuickPoll.Results">
									<f:param name="Question" value="{$question}" />
									<f:param name="PieChartResult" value="{$isPieChartResult}" />
								</f:function>
							</xsl:otherwise>
						</xsl:choose>
					</div>
				</asp:form>
			</body>
		</html>
	</xsl:template>
	<msxsl:script implements-prefix="csharp" language="C#">
		<msxsl:assembly name="System.Web" />
		<msxsl:using namespace="System.Web" />
		public void SetNoCache()
		{
		HttpContext.Current.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
		}
	</msxsl:script>
</xsl:stylesheet>