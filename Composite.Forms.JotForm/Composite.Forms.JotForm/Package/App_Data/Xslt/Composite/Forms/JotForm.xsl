<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f">
	<xsl:variable name="formTitle" select="/in:inputs/in:param[@name='FormTitle']" />
	<xsl:variable name="formId" select="/in:inputs/in:result[@name='GetFormIdFromUrl']" />
	<xsl:variable name="display" select="/in:inputs/in:param[@name='DisplayStyle']" />
	<xsl:template match="/">
		<html>
			<head>
				<!-- markup placed here will be shown in the head section of the rendered page -->
			</head>
			<body>
				<xsl:choose>
					<xsl:when test="$formId = ''" >
						<xsl:text>Incorrect Form URL</xsl:text>
					</xsl:when>
					<xsl:when test="$display = 'Embed'">
						<script type="text/javascript" src="http://form.jotform.com/jsform/{$formId}"></script>
					</xsl:when>
					<xsl:when test="$display = 'iFrame'">
						<iframe allowtransparency="true" src="http://form.jotform.com/form/{$formId}" frameborder="0" style="width:100%; height:628px; border:none;" scrolling="no"></iframe>
					</xsl:when>
					<xsl:when test="$display = 'Pop-up'">
						<a href="javascript:void( window.open('http://www.jotform.com/form/{$formId}', 'blank','scrollbars=yes,toolbar=no,width=700,height=500'))">
							<xsl:value-of select="$formTitle" />
						</a>
					</xsl:when>
					<xsl:when test="$display = 'Feedback Button'">
						<script src="http://www.jotform.com/min/g=feedback" type="text/javascript">
							new JotformFeedback({
							formId: '<xsl:value-of select="$formId" />',
							buttonText: '<xsl:value-of select="$formTitle" />',
							base: "http://www.jotform.com/",
							background: "#F59202",
							fontColor: "#FFFFFF",
							buttonSide: "right",
							buttonAlign: "center",
							type: false,
							width: 700,
							height: 500
							});
						</script>
					</xsl:when>
					<xsl:when test="$display = 'LightBox'">
						<script src="http://www.jotform.com/min/g=feedback" type="text/javascript">
							new JotformFeedback({
							formId: '<xsl:value-of select="$formId" />',
							base: 'http://www.jotform.com/',
							windowTitle: '<xsl:value-of select="$formTitle" />',
							background: '#FFA500',
							fontColor: '#FFFFFF',
							type: false,
							height: 500,
							width: 700
							});
						</script>
						<a class="lightbox-{$formId}" style="cursor:pointer;color:blue;text-decoration:underline;">
							<xsl:value-of select="$formTitle" />
						</a>
					</xsl:when>
					<xsl:otherwise></xsl:otherwise>
				</xsl:choose>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>