using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Composite.Functions;
using System.Globalization;
using System.Text;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.ResourceSystem;
using System.Text.RegularExpressions;
using Composite.Core;
using Composite.Forms.Renderer;

public partial class FormsRenderer_FormsRender : System.Web.UI.UserControl
{
	private string errorMessage = "";
	private bool useCaptcha = true;
	public ParameterList parameters { get; set; }

	static readonly string[] i18n = { "af", "ar", "az", "bg", "bs", "ca", "cs", "da", "de", "el", "en-GB", "eo", "es", "et", "eu", "fa", "fi", "fo", "fr", "fr-CH", "gl", "he", "hr", "hu", "hy", "id", "is", "it", "ja", "ko", "kz", "lt", "lv", "ms", "nl", "no", "pl", "pt", "pt-BR", "rm", "ro", "ru", "sk", "sl", "sq", "sr", "sr-SR", "sv", "ta", "th", "tr", "uk", "vi", "zh-CN", "zh-HK", "zh-TW" };
	static readonly string[] jquery_validate_localizations = {"cn", "cs", "da", "de", "es", "fr", "hu", "it", "kk", "nl", "no", "pl", "ptbr", "ro", "ru", "se", "sk", "tr", "tw", "ua"};  

	protected void OnPreInit(EventArgs e)
	{
		ScriptManager oScriptManager = ScriptManager.GetCurrent(Page);
		if (oScriptManager == null)
		{
			oScriptManager = new ScriptManager();
			oScriptManager.ID = "ScriptManager1";
			Page.Controls.AddAt(0, oScriptManager);
		}
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		IntroText.Text = parameters.GetParameter<string>("IntroText");
		useCaptcha = parameters.GetParameter<bool>("UseCaptcha");
		ValidationSummary.HeaderText = FormsRenderer.GetFrontendString("Composite.Plugins.FormsRenderer", "Composite.Forms.ValidationSummary.HeaderText"); 
		var sendButtonLabel = parameters.GetParameter<string>("SendButtonLabel");
		if (sendButtonLabel != string.Empty)
			Send.Text = GetLocalized(sendButtonLabel);

		var resetButtonLabel = parameters.GetParameter<string>("ResetButtonLabel");
		Reset.Value = GetLocalized(resetButtonLabel);
		if (resetButtonLabel == string.Empty)
			Reset.Visible = false;

		FormsRenderer.InsertForm(this.Fields,parameters);
	}

	private static string GetLocalized(string text)
	{
		return text.Contains("${") ? StringResourceSystemFacade.ParseString(text) : text;
	}

	protected void Page_PreRender(object sender, EventArgs e)
	{
		InsertCssIntoHeader(FormsRenderer.FormsRendererWebPath + "Styles.css");
		InsertCssIntoHeader("http://ajax.microsoft.com/ajax/jquery.ui/1.8.7/themes/cupertino/jquery-ui.css");

		InsertScriptIntoHeader("http://ajax.aspnetcdn.com/ajax/jquery/jquery-1.5.min.js");
		InsertScriptIntoHeader("http://ajax.aspnetcdn.com/ajax/jquery.validate/1.7/jquery.validate.min.js");
		InsertScriptIntoHeader("http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.7/jquery-ui.js");

		if (i18n.Contains(CultureInfo.CurrentCulture.Name))
		{
			InsertScriptIntoHeader(string.Format("http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.7/i18n/jquery.ui.datepicker-{0}.js", CultureInfo.CurrentCulture.Name));
		}
		else if(i18n.Contains(CultureInfo.CurrentCulture.TwoLetterISOLanguageName))
		{
			InsertScriptIntoHeader(string.Format("http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.7/i18n/jquery.ui.datepicker-{0}.js", CultureInfo.CurrentCulture.TwoLetterISOLanguageName));
		}

		if (jquery_validate_localizations.Contains(CultureInfo.CurrentCulture.TwoLetterISOLanguageName))
		{
			InsertScriptIntoHeader(string.Format("http://ajax.aspnetcdn.com/ajax/jQuery.Validate/1.7/localization/messages_{0}.js", CultureInfo.CurrentCulture.TwoLetterISOLanguageName));
		}
		else if (jquery_validate_localizations.Contains(RegionInfo.CurrentRegion.TwoLetterISORegionName.ToLower()))
		{
			InsertScriptIntoHeader(string.Format("http://ajax.aspnetcdn.com/ajax/jQuery.Validate/1.7/localization/messages_{0}.js", RegionInfo.CurrentRegion.TwoLetterISORegionName.ToLower()));
		}


        //Localizing DatePicker
        HtmlGenericControl script = InsertScriptIntoHeader();
		script = new HtmlGenericControl("script");
		script.Attributes.Add("type", "text/javascript");
		script.Attributes.Add("language", "JavaScript");
		script.InnerHtml = @"
<!--

$('.InputDate').datepicker();


$(document).ready(function() {
	$.validator.addMethod(""integer"", function(value, element) {
		return /^\d+$/.test(value);;
	}, $.validator.messages[""digits""]);
	$.validator.addMethod(""number"", function(value, element) {
		return /^-?(?:\d+)(?:" + NumberDecimalSeparator() + @"\d+)?$/.test(value);
	}, $.validator.messages[""number""]);
	$(""form"").each(function() {
		$(this).validate();
	});
})

-->
";
		Controls.Add(script);

		if (useCaptcha)
		{
			var encryptedValue = Composite.Core.WebClient.Captcha.Captcha.CreateEncryptedValue(); ;
			CaptchaText.Text = FormsRenderer.GetFrontendString("Composite.Plugins.FormsRenderer", "Composite.Forms.Captcha.CaptchaText.label");
			CaptchaImage.ImageUrl = GetCaptchaImageUrl(encryptedValue);
			CaptchaInput.Text = "";
			Session["FormsRendererCaptcha"] = encryptedValue;
		}
		else
		{	
			Captcha.Visible = false;
		}
	}

	public string GetCaptchaImageUrl(string encryptedCaptchaValue)
	{
		var url = new UrlBuilder("/Renderers/Captcha.ashx");
		url["value"] = encryptedCaptchaValue;
		return url.ToString();
	}
	

	private string NumberDecimalSeparator()
	{
		return CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "." ? "\\." : CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
	}


	protected HtmlGenericControl InsertScriptIntoHeader(string link)
	{
		HtmlGenericControl script = InsertScriptIntoHeader();
		script.Attributes.Add("src", link);
		return script;
	}

	protected HtmlGenericControl InsertScriptIntoHeader()
	{
		HtmlGenericControl script = new HtmlGenericControl("script");
		script.Attributes.Add("type", "text/javascript");
		script.Attributes.Add("language", "JavaScript");
		Page.Header.Controls.Add(script);
		return script;
	}

	protected HtmlLink InsertCssIntoHeader(string link)
	{
		HtmlLink csslink = new HtmlLink();
		csslink.Href = link;
		csslink.Attributes.Add("type", "text/css");
		csslink.Attributes.Add("rel", "stylesheet");
		Page.Header.Controls.Add(csslink);
		return csslink;
	}

	protected void Send_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormsRenderer.SubmitForm(parameters, CaptchaInput.Text))
			{
				string responseText = parameters.GetParameter<string>("ResponseText");
				string responsePageId = parameters.GetParameter<string>("ResponseUrl");
				string responsePageUrl;

				try
				{
					PageStructureInfo.TryGetPageUrl(new Guid(responsePageId), out responsePageUrl);
					if (responsePageUrl != string.Empty)
					{
						Response.Redirect(responsePageUrl);
					}
				}
				catch { }
				IntroText.Text = responseText;
				Fields.Visible = false;
				FieldSet.Visible = false;
				Captcha.Visible = false;
			}
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}
	protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
	{
		if (errorMessage != string.Empty)
			args.IsValid = false;
	}

	static string addslashes(string text)
	{
		char[] chars = HttpUtility.HtmlEncode(text).ToCharArray();
		StringBuilder result = new StringBuilder(text.Length + (int)(text.Length * 0.1));

		foreach (char c in chars)
		{
			int value = Convert.ToInt32(c);
			if (value > 127 || value == 39)
				result.AppendFormat("&#{0};", value);
			else
				result.Append(c);
		}
		return result.ToString();
	}
}
