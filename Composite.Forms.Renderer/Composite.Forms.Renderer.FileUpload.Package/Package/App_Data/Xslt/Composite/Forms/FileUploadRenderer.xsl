<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	exclude-result-prefixes="xsl in lang f">

	<xsl:template match="/">
		<html>
			<head>
				<style type="text/css" media="all">
					div.FormsRenderer fieldset.Fields input.FileBox {
						height:auto;
					}
				</style>

			</head>
			<body>
				<asp:form xmlns:asp="http://www.composite.net/ns/asp.net/controls">
					<f:function name="Composite.Forms.Renderer">
						<f:param name="DataType" value="{/in:inputs/in:param[@name='DataType']}" />
						<f:param name="IntroText" value="{/in:inputs/in:param[@name='IntroText']}" />
						<f:param name="ResponseText">
							<f:function name="Composite.Forms.Renderer.FileUpload.SendEmail">
								<f:param name="ResponseText" value="{/in:inputs/in:param[@name='ResponseText']}" />
								<f:param name="From" value="{/in:inputs/in:param[@name='EmailFrom']}" />
								<f:param name="To" value="{/in:inputs/in:param[@name='EmailTo']}" />
								<f:param name="Subject" value="{/in:inputs/in:param[@name='EmailSubject']}" />
							</f:function>
						</f:param>
						<f:param name="SendButtonLabel" value="{/in:inputs/in:param[@name='SendButtonLabel']}" />
						<f:param name="ResetButtonLabel" value="{/in:inputs/in:param[@name='ResetButtonLabel']}" />
						<f:param name="UseCaptcha" value="{/in:inputs/in:param[@name='UseCaptcha']}" />
					</f:function>
				</asp:form>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>
