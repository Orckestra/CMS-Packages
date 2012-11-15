<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:in="http://www.composite.net/ns/transformation/input/1.0" xmlns:lang="http://www.composite.net/ns/localization/1.0" xmlns:f="http://www.composite.net/ns/function/1.0" xmlns:c1="http://c1.composite.net/StandardFunctions" xmlns:captcha="http://c1.composite.net/Captcha" xmlns="http://www.w3.org/1999/xhtml" exclude-result-prefixes="xsl in lang f c1 captcha">
	<xsl:param name="toEmail" select="/in:inputs/in:param[@name='Email']" />
	<xsl:param name="useCaptcha" select="/in:inputs/in:param[@name='UseCaptcha']" />
	<xsl:param name="optionalFields" select="/in:inputs/in:param[@name='OptionalFields']" />
	<xsl:param name="emailTemplate" select="/in:inputs/in:param[@name='EmailTemplate']" />

	<xsl:variable name="Submitted" select="c1:GetFormData('contactFormSubmit')" />
	<xsl:variable name="captcha" select="c1:GetFormData('captcha')" />
	<xsl:variable name="captchaEncryptedValue">
		<xsl:if test="$useCaptcha = 'true'">
			<xsl:value-of select="captcha:GetEncryptedValue(c1:GetFormData('captchaEncryptedValue'))" />
		</xsl:if>
	</xsl:variable>
	<xsl:variable name="SendContactFormMarkup">
		<f:function name="Composite.Forms.ContactForm.Functions.Send">
			<f:param name="toEmail" value="{$toEmail}" />
			<f:param name="fromName" value="{c1:GetFormData('fromName')}" />
			<f:param name="fromEmail" value="{c1:GetFormData('fromEmail')}" />
			<f:param name="messageSubject" value="{c1:GetFormData('messageSubject')}" />
			<f:param name="message" value="{c1:GetFormData('message')}" />
			<f:param name="company" value="{c1:GetFormData('company')}" />
			<f:param name="website" value="{c1:GetFormData('website')}" />
			<f:param name="address" value="{c1:GetFormData('address')}" />
			<f:param name="phonenumber" value="{c1:GetFormData('phonenumber')}" />
			<f:param name="useCaptcha" value="{$useCaptcha}" />
			<f:param name="captcha" value="{$captcha}" />
			<f:param name="captchaEncryptedValue" value="{$captchaEncryptedValue}" />
			<f:param name="emailTemplateId" value="{$emailTemplate}" />
		</f:function>
	</xsl:variable>
	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Forms/ContactForm/Styles.css" />
				<script id="jquery-js" src="//code.jquery.com/jquery-latest.min.js" type="text/javascript"></script>
				<script src="~/Frontend/Composite/Forms/ContactForm/Scripts/contactform_validation.js" type="text/javascript"></script>
			</head>
			<body>
				<xsl:variable name="SendContactForm" select="c1:CallFunction($SendContactFormMarkup)" />
				<xsl:variable name="SendActionErrors" select="$SendContactForm/Error" />
				<xsl:variable name="SendActionData" select="$SendContactForm/SubmittedData" />
				<xsl:choose>
					<xsl:when test="count($SendActionErrors) = 0 and $Submitted = 'true'">
						<p>
							<lang:string key="Resource, Resources.ContactForm.ResponseText" />
						</p>
						<xsl:if test="$useCaptcha = 'true'">
							<xsl:value-of select="captcha:RegisterUsage($captchaEncryptedValue)" />
						</xsl:if>
					</xsl:when>
					<xsl:otherwise>
						<p>
							<lang:string key="Resource, Resources.ContactForm.RequestText" />
						</p>
						<xsl:if test="$Submitted='true' and count($SendActionErrors)&gt;0">
							<ul id="errors_contactform">
								<xsl:for-each select="$SendActionErrors">
									<li>
										<xsl:value-of select="@ErrorDescription" />
									</li>
								</xsl:for-each>
							</ul>
						</xsl:if>
						<asp:form xmlns:asp="http://www.composite.net/ns/asp.net/controls">
							<input type="hidden" name="contactFormSubmit" id="contactFormSubmit" value="true" />
							<ul id="contact_form">
								<li class="required">
									<label for="fromName">
										<lang:string key="Resource, Resources.ContactForm.Name" />
									</label>
									<input name="fromName" id="fromName" type="text" class="text" maxlength="128" value="{$SendActionData[@Fieldname = 'fromName']/@Value}" />
								</li>
								<li class="required">
									<label for="fromEmail">
										<lang:string key="Resource, Resources.ContactForm.Email" />
									</label>
									<input name="fromEmail" id="fromEmail" type="text" class="text Email_Input" maxlength="128" value="{$SendActionData[@Fieldname = 'fromEmail']/@Value}" />
								</li>
								<xsl:if test="contains($optionalFields,'company')">
									<li>
										<label for="company">
											<lang:string key="Resource, Resources.ContactForm.Company" />
										</label>
										<input name="company" id="company" type="text" class="text" maxlength="128" value="{$SendActionData[@Fieldname = 'company']/@Value}" />
									</li>
								</xsl:if>
								<xsl:if test="contains($optionalFields,'website')">
									<li>
										<label for="website">
											<lang:string key="Resource, Resources.ContactForm.WebSite" />
										</label>
										<input name="website" id="website" type="text" class="text" maxlength="256" value="{$SendActionData[@Fieldname = 'website']/@Value}" />
									</li>
								</xsl:if>
								<xsl:if test="contains($optionalFields,'address')">
									<li>
										<label for="address">
											<lang:string key="Resource, Resources.ContactForm.Address" />
										</label>
										<input name="address" id="address" type="text" class="text" maxlength="256" value="{$SendActionData[@Fieldname = 'address']/@Value}" />
									</li>
								</xsl:if>
								<xsl:if test="contains($optionalFields,'phone number')">
									<li>
										<label for="phonenumber">
											<lang:string key="Resource, Resources.ContactForm.PhoneNumber" />
										</label>
										<input name="phonenumber" id="phonenumber" type="text" class="text" maxlength="128" value="{$SendActionData[@Fieldname = 'phonenumber']/@Value}" />
									</li>
								</xsl:if>
								<xsl:if test="contains($optionalFields,'subject')">
									<li>
										<label for="messageSubject">
											<lang:string key="Resource, Resources.ContactForm.Subject" />
										</label>
										<input name="messageSubject" id="messageSubject" type="text" class="text" maxlength="128" value="{$SendActionData[@Fieldname = 'messageSubject']/@Value}" />
									</li>
								</xsl:if>
								<li class="required">
									<label for="message">
										<lang:string key="Resource, Resources.ContactForm.Message" />
									</label>
									<textarea name="message" id="message" class="text" cols="50" rows="8">
										<xsl:value-of select="$SendActionData[@Fieldname = 'message']/@Value" />
									</textarea>
								</li>
								<xsl:if test="$useCaptcha = 'true'">
									<li class="required">
										<label for="captcha">
											<lang:string key="Resource, Resources.ContactForm.Captcha" />
										</label>
										<input type="hidden" name="captchaEncryptedValue" id="captchaEncryptedValue" value="{$captchaEncryptedValue}" />
										<input type="text" name="captcha" id="captcha" class="text" value="{$captcha}" />
										<br />
										<img class="captchaImg" src="{captcha:GetImageUrl($captchaEncryptedValue)}" alt="Captcha image" />
									</li>
								</xsl:if>
								<li class="clearfix">
									<xsl:variable name="SendText">
										<xsl:text><lang:string key="Resource, Resources.ContactForm.SendButton" /></xsl:text>
									</xsl:variable>
									<input name="submitContactForm" id="submitContactForm" type="submit" value="{$SendText}" />
								</li>
							</ul>
						</asp:form>
					</xsl:otherwise>
				</xsl:choose>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>