<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:in="http://www.composite.net/ns/transformation/input/1.0"
	xmlns="http://www.w3.org/1999/xhtml"
	xmlns:f="http://www.composite.net/ns/function/1.0"
	xmlns:captcha="http://c1.composite.net/Captcha"
	xmlns:c1="http://c1.composite.net/StandardFunctions"
	xmlns:be="#BlogXsltExtensionsFunction"
	exclude-result-prefixes="xsl in f c1 captcha be">

	<xsl:template match="/">
		<html>
			<head />
			<body>
				<asp:form xmlns:asp="http://www.composite.net/ns/asp.net/controls" >
					<div id="BlogComments">
						<xsl:if test="count(in:inputs/in:result[@name='GetCommentsXml']/Comments)>0">
							<fieldset>
								<legend>
									<xsl:value-of select="be:GetLocalized('Blog','commentsText')" />
								</legend>
								<ul id="CommentsList">
									<xsl:for-each select="/in:inputs/in:result[@name='GetCommentsXml']/Comments">
										<xsl:sort select="@Date" />
										<li>
											<div class="Title">
												<xsl:value-of select="@Title" />
											</div>
											<div class="Author">
												<xsl:value-of select="@Name" />
											</div>
											<div class="Date">
												<xsl:value-of select="be:CustomDateFormat(@Date, 'dd.MM.yyyy HH:mm')" />
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
						<xsl:if test ="/in:inputs/in:param[@name='AllowNewComments'] = 'true'">
							<a name="newcomment"></a>
							<xsl:call-template name="CommentsForm" />
						</xsl:if>
					</div>
				</asp:form>
			</body>
		</html>
	</xsl:template>

	<xsl:template name="CommentsForm">
		<xsl:variable name="Submitted" select="c1:GetFormData('CommentSubmit')" />
		<xsl:variable name="cssFieldError" select="'FieldError'" />
		<xsl:variable name="captchaEncryptedValue" select="captcha:GetEncryptedValue(c1:GetFormData('captcha'))" />
		<xsl:variable name="captchaValue" select="c1:GetFormData('txtCaptcha')" />
		<xsl:variable name="captchaIsValid" select="captcha:IsValid($captchaValue, $captchaEncryptedValue)" />
		<xsl:variable name="SaveCommentMarkup">
			<f:function name="Composite.Community.Blog.SaveComment">
				<f:param name="name" value="{c1:GetFormData('Name')}" />
				<f:param name="email" value="{c1:GetFormData('Email')}" />
				<f:param name="commentTitle" value="{c1:GetFormData('Title')}" />
				<f:param name="commentText" value="{c1:GetFormData('Comment')}" />
				<f:param name="captcha" value="{$captchaIsValid}" />
				<f:param name="blogEntryGuid" value="{/in:inputs/in:param[@name='BlogEntryGuid']}" />
			</f:function>
		</xsl:variable>
		<xsl:variable name="SaveComment" select="c1:CallFunction($SaveCommentMarkup)" />
		<xsl:variable name="SaveActionErrors" select="$SaveComment/Error" />
		<xsl:variable name="SaveActionData" select="$SaveComment/SubmittedData" />

		<fieldset>
			<input type="hidden" name="CommentSubmit" value="true" />
			<legend>
				<xsl:value-of select="be:GetLocalized('Blog','commentsTitleText')" />
			</legend>
      <xsl:if test="$Submitted = 'true'">
        <div id="ErrorBox">
          <xsl:choose>
            <xsl:when test="count($SaveActionErrors)>0">
              <h4>
                <xsl:value-of select="be:GetLocalized('Blog','correctErrorsText')" />
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
              <xsl:value-of select="captcha:RegisterUsage($captchaEncryptedValue)" />
            </xsl:otherwise>
          </xsl:choose>
        </div>
			</xsl:if>

			<ul id="CommentsForm">
				<li>
					<xsl:if test="count($SaveActionErrors[@Fieldname='Name'])>0 and $Submitted = 'true'">
						<xsl:attribute name="class">
							<xsl:value-of select="$cssFieldError" />
						</xsl:attribute>
					</xsl:if>
					<label for="Name">
						<xsl:value-of select="be:GetLocalized('Blog','commentNameFieldText')" />
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
						<xsl:value-of select="be:GetLocalized('Blog','commentEmailFieldText')" />
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
						<xsl:value-of select="be:GetLocalized('Blog','commentTitleFieldText')" />
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
						<xsl:value-of select="be:GetLocalized('Blog','commentCommentFieldText')" />
					</label>
					<textarea name="Comment" id="Comment">
						<xsl:value-of select="$SaveActionData[@Fieldname = 'CommentText']/@Value" />
					</textarea>
				</li>
				<li>
					<xsl:if test="count($SaveActionErrors[@Fieldname='Captcha'])>0 and $Submitted = 'true'">
						<xsl:attribute name="class">
							<xsl:value-of select="$cssFieldError" />
						</xsl:attribute>
					</xsl:if>
					<label for="CaptchaUserInput">
						<xsl:value-of select="be:GetLocalized('Blog','commentCaptchaFiledText')" />
					</label>
					<img src="{captcha:GetImageUrl($captchaEncryptedValue)}" alt="Captcha image" />
					<input name="captcha" type="hidden" value="{$captchaEncryptedValue}" />
					<input type="text" name="txtCaptcha" value="{$captchaValue}" />
				</li>
				<li>
					<input type="submit" onclick="this.form.action=this.form.action+'#newcomment'">
						<xsl:attribute name="value">
							<xsl:value-of select="be:GetLocalized('Blog','SendButton')" />
						</xsl:attribute>
					</input>
				</li>
			</ul>
		</fieldset>
	</xsl:template>

	<xsl:template name="break">
		<xsl:param name="text" select="." />
		<xsl:choose>
			<xsl:when test="contains($text, '&#xa;')">
				<xsl:value-of select="substring-before($text, '&#xa;') " />
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

</xsl:stylesheet>
