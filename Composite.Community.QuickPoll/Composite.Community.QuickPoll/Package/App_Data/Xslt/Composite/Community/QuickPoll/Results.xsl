<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="xsl in">

	<xsl:param name="questionId" select="/in:inputs/in:param[@name='Question']" />
	<xsl:param name="items" select="/in:inputs/in:result[@name='GetQuickPollAnswersXml']/Answers" />
	<xsl:param name="barLength" select="/in:inputs/in:param[@name='BarLength']" />
	<xsl:param name="isPieChartResult" select="/in:inputs/in:param[@name='PieChartResult']" />
	<!--PieChart Setting-->
	<xsl:variable name="is3D" select="1" />
	<xsl:variable name="width" select="400" />
	<xsl:variable name="height" select="300" />

	<xsl:template match="/">
		<html>
			<head>
				<xsl:if test="$isPieChartResult='true'">
					<script id="google.com-jsapi" type="text/javascript" src="https://www.google.com/jsapi"></script>
					<script type="text/javascript">

						// Load the Visualization API and the piechart package.
						google.load('visualization', '1.0', {'packages':['corechart']});

						// Set a callback to run when the Google Visualization API is loaded.
						google.setOnLoadCallback(drawChart);

						// Callback that creates and populates a data table,
						// instantiates the pie chart, passes in the data and
						// draws it.
						function drawChart() {

						// Create the data table.
						var data = new google.visualization.DataTable();
						data.addColumn('string', 'Topping');
						data.addColumn('number', 'Slices');
						data.addRows([
						<xsl:for-each select="$items">
							['<xsl:value-of select="@AnswerText"/>', <xsl:value-of select="@TotalVotes"/>]<xsl:if test="position()!=last()" >,</xsl:if>
						</xsl:for-each>
						]);

						// Set chart options
						var options = {'is3D':<xsl:value-of select="$is3D" />,
						'width':<xsl:value-of select="$width"/>,
						'height':<xsl:value-of select="$height"/>};

						// Instantiate and draw our chart, passing in some options.
						var chart_div_id = 'chart_div'  + '<xsl:value-of select="$questionId" />';
						var chart = new google.visualization.PieChart(document.getElementById(chart_div_id));
						chart.draw(data, options);
						}
					</script>

				</xsl:if>
			</head>
			<body>
				<xsl:choose>
					<xsl:when test="$isPieChartResult='true'">
						<!--Div that will hold the pie chart-->
						<div id="chart_div{$questionId}"></div>
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