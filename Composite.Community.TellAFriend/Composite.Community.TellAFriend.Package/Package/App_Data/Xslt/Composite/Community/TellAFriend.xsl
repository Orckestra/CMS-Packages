<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns:lang="http://www.composite.net/ns/localization/1.0"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:c1="http://c1.composite.net/StandardFunctions"
	xmlns:captcha="http://c1.composite.net/Captcha"
	xmlns:taf="#TellAFriendXsltExtensionsFunction"
	exclude-result-prefixes="xsl in lang f c1 captcha taf">

	<xsl:param name="popUp" select="/in:inputs/in:param[@name='PopUp']" />
	<xsl:param name="useCaptcha" select="/in:inputs/in:param[@name='UseCaptcha']" />
	<xsl:variable name="website" select="taf:GetWebsite()" />
	<xsl:variable name="url" select="taf:GetUrl()" />
	<xsl:variable name="culture" select="taf:GetCurrentCulture()" />

	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Community/TellAFriend/Styles.css" />
				<xsl:if test="$popUp='true'">
					<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" type="text/javascript"></script>
					<script src="~/Frontend/Composite/Community/TellAFriend/Scripts/TellAFriend.js" type="text/javascript"></script>
				</xsl:if>
			</head>
			<body>
				<asp:form xmlns:asp="http://www.composite.net/ns/asp.net/controls">
					<xsl:choose>
						<xsl:when test="c1:GetQueryStringValue('TellAFriend') !='1'">
							<a class="email" title="{taf:GetLocalized('TellAFriend','tellAFriend')}">
								<xsl:attribute name="href">
									<xsl:choose>
										<xsl:when test="$popUp='true'">#emailpopup</xsl:when>
										<xsl:otherwise>
											<xsl:value-of select="$url" />
										</xsl:otherwise>
									</xsl:choose>
								</xsl:attribute>
								<xsl:value-of select="taf:GetLocalized('TellAFriend','tellAFriend')" />
							</a>
						</xsl:when>
					</xsl:choose>
					<xsl:if test="$popUp='true' or c1:GetQueryStringValue('TellAFriend')='1'">
						<xsl:call-template name="TellAFriendForm" />
					</xsl:if>
				</asp:form>
			</body>
		</html>
	</xsl:template>

	<xsl:template name="TellAFriendForm">
		<xsl:variable name="Submitted" select="c1:GetFormData('TellAFriendSubmit')" />
		<xsl:variable name="cssFieldError" select="'FieldError'" />
		<xsl:variable name="captchaEncryptedValue" select="captcha:GetEncryptedValue(c1:GetFormData('captchaEncryptedValue'))" />
		<xsl:variable name="captcha" select="c1:GetFormData('captcha')" />
		<xsl:variable name="SaveCommentMarkup">
			<f:function name="Composite.Community.TellAFriend.Send">
				<f:param name="fromName" value="{c1:GetFormData('fromName')}" />
				<f:param name="fromEmail" value="{c1:GetFormData('fromEmail')}" />
				<f:param name="toName" value="{c1:GetFormData('toName')}" />
				<f:param name="toEmail" value="{c1:GetFormData('toEmail')}" />
				<f:param name="description" value="{c1:GetFormData('description')}" />
				<f:param name="captcha" value="{$captcha}" />
				<f:param name="captchaEncryptedValue" value="{$captchaEncryptedValue}" />
				<f:param name="useCaptcha" value="{$useCaptcha}" />
				<f:param name="website" value="{$website}" />
				<f:param name="url" value="{$url}" />
			</f:function>
		</xsl:variable>
		<xsl:variable name="SaveComment" select="c1:CallFunction($SaveCommentMarkup)" />
		<xsl:variable name="SaveActionErrors" select="$SaveComment/Error" />
		<xsl:variable name="SaveActionData" select="$SaveComment/SubmittedData" />

		<xsl:choose>
			<xsl:when test="count($SaveActionErrors) = 0 and $Submitted = 'true'">
				<xsl:value-of select="taf:GetLocalized('TellAFriend','thankYou')" />
				<xsl:value-of select="captcha:RegisterUsage($captchaEncryptedValue)" />
			</xsl:when>
			<xsl:otherwise>
				<div id="TellAFriend">
					<xsl:if test="$popUp='true'">
						<xsl:attribute name="class">dynamic</xsl:attribute>
						<a class="close" href="#close" >Close</a>
					</xsl:if>
					<input type="hidden" name="TellAFriendSubmit" id="TellAFriendSubmit" value="true" />
					<input type="hidden" name="website" id="website" value="{$website}" />
					<input type="hidden" name="url" id="url" value="{$url}" />
					<input type="hidden" name="culture" id="culture" value="{$culture}" />
					<input type="hidden" name="useCaptcha" id="useCaptcha" value="{$useCaptcha}" />
					<h4>
						<xsl:value-of select="taf:GetLocalized('TellAFriend','tellAFriend')" />
					</h4>
					<ul id="ErrorBox">
						<xsl:if test="$Submitted = 'true'">
							<xsl:if test="count($SaveActionErrors)>0">
								<xsl:for-each select="$SaveActionErrors">
									<li>
										<xsl:value-of select="@ErrorDescription" />
									</li>
								</xsl:for-each>
							</xsl:if>
						</xsl:if>
					</ul>
					<ul id="TellAFriendForm">
						<li>
							<label for="fromName">
								<xsl:value-of select="taf:GetLocalized('TellAFriend','fromName')" />
							</label>
							<input type="text" name="fromName" id="fromName" value="{$SaveActionData[@Fieldname = 'fromName']/@Value}" maxlength="64">
								<xsl:if test="count($SaveActionErrors[@Fieldname='fromName'])>0 and $Submitted = 'true'">
									<xsl:attribute name="class">
										<xsl:value-of select="$cssFieldError" />
									</xsl:attribute>
								</xsl:if>
							</input>
						</li>
						<li>
							<label for="fromEmail">
								<xsl:value-of select="taf:GetLocalized('TellAFriend','fromEmail')" />
							</label>
							<input type="text" name="fromEmail" id="fromEmail" value="{$SaveActionData[@Fieldname = 'fromEmail']/@Value}" maxlength="128">
								<xsl:if test="count($SaveActionErrors[@Fieldname='fromEmail'])>0 and $Submitted = 'true'">
									<xsl:attribute name="class">
										<xsl:value-of select="$cssFieldError" />
									</xsl:attribute>
								</xsl:if>
							</input>
						</li>
						<li>
							<label for="toName">
								<xsl:value-of select="taf:GetLocalized('TellAFriend','toName')" />
							</label>
							<input type="text" name="toName" id="toName" value="{$SaveActionData[@Fieldname = 'toName']/@Value}" maxlength="64">
								<xsl:if test="count($SaveActionErrors[@Fieldname='toName'])>0 and $Submitted = 'true'">
									<xsl:attribute name="class">
										<xsl:value-of select="$cssFieldError" />
									</xsl:attribute>
								</xsl:if>
							</input>
						</li>
						<li>
							<label for="toEmail">
								<xsl:value-of select="taf:GetLocalized('TellAFriend','toEmail')" />
							</label>
							<input type="text" name="toEmail" id="toEmail" value="{$SaveActionData[@Fieldname = 'toEmail']/@Value}" maxlength="128">
								<xsl:if test="count($SaveActionErrors[@Fieldname='toEmail'])>0 and $Submitted = 'true'">
									<xsl:attribute name="class">
										<xsl:value-of select="$cssFieldError" />
									</xsl:attribute>
								</xsl:if>
							</input>
						</li>
						<li>
							<label for="description">
								<xsl:value-of select="taf:GetLocalized('TellAFriend','description')" />
							</label>
							<textarea name="description" id="description">
								<xsl:value-of select="$SaveActionData[@Fieldname = 'description']/@Value" />
							</textarea>
						</li>
						<xsl:if test="$useCaptcha = 'true'">
							<li>
							<label for="CaptchaUserInput">
								<xsl:value-of select="taf:GetLocalized('TellAFriend','captcha')" />
							</label>
							<img src="{captcha:GetImageUrl($captchaEncryptedValue)}" alt="Captcha image" />
							<input type="hidden" name="captchaEncryptedValue" id="captchaEncryptedValue" value="{$captchaEncryptedValue}" />
							<input type="text" name="captcha" id ="captcha" value="{$captcha}">
								<xsl:if test="count($SaveActionErrors[@Fieldname='captcha'])>0 and $Submitted = 'true'">
									<xsl:attribute name="class">
										<xsl:value-of select="$cssFieldError" />
									</xsl:attribute>
								</xsl:if>
							</input>
						</li>
						</xsl:if>
						<li>
							<input type="submit" name="submitTellAFriend" id="submitTellAFriend" value="{taf:GetLocalized('TellAFriend','submit')}" />
						</li>
					</ul>
				</div>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

</xsl:stylesheet>
