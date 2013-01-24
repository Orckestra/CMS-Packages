<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns:csharp="http://c1.composite.net/csharp"
	xmlns:captcha="http://c1.composite.net/Captcha"
	xmlns:c1="http://c1.composite.net/StandardFunctions"
	xmlns:asp="http://www.composite.net/ns/asp.net/controls"
	exclude-result-prefixes="xsl in f csharp c1 captcha">

	<xsl:param name="useCaptcha" select="/in:inputs/in:param[@name='UseCaptcha']" />

	<xsl:template match="/">
		<html>
			<head>
				<link rel="stylesheet" type="text/css" href="~/Frontend/Composite/Community/PageComments/Styles.css" />
			</head>
			<body>
				<asp:form>
					<div id="PageComments">
						<xsl:if test="count(/in:inputs/in:result[@name='GetItemXml']/Item)>0">
							<fieldset class="CommentsList">
								<legend>
									Comments
								</legend>
								<ul>
									<xsl:for-each select="/in:inputs/in:result[@name='GetItemXml']/Item">
										<li>
											<div class="Title">
												<xsl:value-of select="@Title"/>
											</div>
											<div class="Author">
												<xsl:value-of select="@Name"/>
											</div>
											<div class="Date">
												<xsl:value-of select="csharp:CustomDateFormat(@Date, 'dd.MM.yyyy HH:mm')"/>
											</div>
											<div class="Comment">
												<xsl:call-template name="break">
													<xsl:with-param name="text" select="@Comment" />
												</xsl:call-template>
											</div>
										</li>
									</xsl:for-each>
								</ul>
							</fieldset>
						</xsl:if>
						<xsl:if test ="count(/in:inputs/in:result[@name='GetSettingsXml']/Settings[@Comment = 'true']) = 0">
							<xsl:call-template name="Form"/>
						</xsl:if>
					</div>
				</asp:form>
			</body>
		</html>
	</xsl:template>

	<xsl:template name="Form">
		<xsl:variable name="Submitted" select="c1:GetFormData('CommentSubmit')" />
		<xsl:variable name="cssFieldError" select="'FieldError'" />

		<!-- Captcha -->
		<xsl:variable name="captchaEncryptedValue" select="captcha:GetEncryptedValue(c1:GetFormData('captcha'))" />
		<xsl:variable name="captchaValue" select="c1:GetFormData('txtCaptcha')" />
		<xsl:variable name="captchaIsValid">
			<xsl:choose>
				<xsl:when test="$useCaptcha = 'true'">
					<xsl:value-of select="captcha:IsValid($captchaValue, $captchaEncryptedValue)" />
				</xsl:when>
				<xsl:otherwise>true</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<xsl:variable name="SaveCommentMarkup">
			<f:function name="Composite.Community.PageComments.SaveComment">
				<f:param name="name" value="{c1:GetFormData('Name')}" />
				<f:param name="email" value="{c1:GetFormData('Email')}" />
				<f:param name="commentTitle" value="{c1:GetFormData('Title')}" />
				<f:param name="commentText" value="{c1:GetFormData('Comment')}" />
				<f:param name="Captcha" value="{$captchaIsValid}" />
			</f:function>
		</xsl:variable>

		<xsl:variable name="SaveComment" select="c1:CallFunction($SaveCommentMarkup)" />
		<xsl:variable name="SaveActionErrors" select="$SaveComment/Error" />
		<xsl:variable name="SaveActionData" select="$SaveComment/SubmittedData" />

		<fieldset class="CommentsForm">
			<input type="hidden" name="CommentSubmit" value="true"/>
			<legend>
				Write Your Comment
			</legend>
			<xsl:if test="$Submitted = 'true'">
				<div class="ErrorBox">
					<xsl:choose>
						<xsl:when test="count($SaveActionErrors)&gt;0">
							<h4>
								Please correct the errors below:
							</h4>
							<ul>
								<xsl:for-each select="$SaveActionErrors">
									<li>
										<xsl:value-of select="@ErrorDescription" />
									</li>
								</xsl:for-each>
							</ul>
						</xsl:when>
						<xsl:otherwise>
							<xsl:if test="$useCaptcha = 'true'">
								<xsl:value-of select="captcha:RegisterUsage($captchaEncryptedValue)" />
							</xsl:if>
						</xsl:otherwise>
					</xsl:choose>
				</div>
			</xsl:if>

			<ol>
				<li>
					<xsl:if test="count($SaveActionErrors[@Fieldname='Name'])>0 and $Submitted = 'true'">
						<xsl:attribute name="class">
							<xsl:value-of select="$cssFieldError" />
						</xsl:attribute>
					</xsl:if>
					<label for="Name">
						Name
					</label>
					<input type="text" name="Name" id="Name" value="{$SaveActionData[@Fieldname = 'Name']/@Value}" maxlength="64" />
				</li>
				<li>
					<xsl:if test="count($SaveActionErrors[@Fieldname='Email'])>0 and $Submitted = 'true'">
						<xsl:attribute name="class">
							<xsl:value-of select="$cssFieldError" />
						</xsl:attribute>
					</xsl:if>
					<label for="Email">
						Email
					</label>
					<input type="text" name="Email" id="Email" value="{$SaveActionData[@Fieldname = 'Email']/@Value}" maxlength="128" />
				</li>
				<li>
					<xsl:if test="count($SaveActionErrors[@Fieldname='Title'])>0 and $Submitted = 'true'">
						<xsl:attribute name="class">
							<xsl:value-of select="$cssFieldError" />
						</xsl:attribute>
					</xsl:if>
					<label for="Title">
						Title
					</label>
					<input type="text" name="Title" id="Title" value="{$SaveActionData[@Fieldname = 'CommentTitle']/@Value}" maxlength="128" />
				</li>
				<li>
					<xsl:if test="count($SaveActionErrors[@Fieldname='Comment'])>0 and $Submitted = 'true'">
						<xsl:attribute name="class">
							<xsl:value-of select="$cssFieldError" />
						</xsl:attribute>
					</xsl:if>
					<label for="Comment">
						Comment
					</label>
					<textarea name="Comment" id="Comment">
						<xsl:value-of select="$SaveActionData[@Fieldname = 'CommentText']/@Value" />
					</textarea>
				</li>
				<xsl:if test="$useCaptcha = 'true'">
					<li>
					<xsl:if test="count($SaveActionErrors[@Fieldname='Captcha'])>0 and $Submitted = 'true'">
						<xsl:attribute name="class">
							<xsl:value-of select="$cssFieldError" />
						</xsl:attribute>
					</xsl:if>
					<label for="CaptchaUserInput">
						Write the text from the image
					</label>
					<img src="{captcha:GetImageUrl($captchaEncryptedValue)}" alt="Captcha image" />
					<input name="captcha" type="hidden" value="{$captchaEncryptedValue}" />
					<input type="text" name="txtCaptcha" value="{$captchaValue}" />
				</li>
				</xsl:if>
				<li>
					<input type="submit" value="Send" onclick="this.form.action=this.form.action+'#newcomment'" />
					<a name="newcomment"></a>
				</li>
			</ol>
		</fieldset>
	</xsl:template>

	<xsl:template name="break">
		<xsl:param name="text" select="."/>
		<xsl:choose>
			<xsl:when test="contains($text, '&#xa;')">
				<xsl:value-of select="substring-before($text, '&#xa;')" />
				<br/>
				<xsl:call-template name="break">
					<xsl:with-param name="text" select="substring-after($text, '&#xa;')" />
				</xsl:call-template>
			</xsl:when>
			<xsl:otherwise>
				<xsl:value-of select="$text" />
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>

	<msxsl:script implements-prefix="csharp" language="C#" xmlns:msxsl="urn:schemas-microsoft-com:xslt">
		<msxsl:assembly name="System.Web" />
		<msxsl:using namespace="System.Web" />
		<![CDATA[
		public string CustomDateFormat(DateTime Date, string DateFormat)
		{
			return Date.ToString(DateFormat);
		}
	]]>
	</msxsl:script>

</xsl:stylesheet>
