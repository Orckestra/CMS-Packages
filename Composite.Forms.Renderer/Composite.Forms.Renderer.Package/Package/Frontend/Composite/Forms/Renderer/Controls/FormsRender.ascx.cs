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
    static readonly string[] jquery_validate_localizations = { "cn", "cs", "da", "de", "es", "fr", "hu", "it", "kk", "nl", "no", "pl", "ptbr", "ro", "ru", "se", "sk", "tr", "tw", "ua" };

    protected void OnPreInit(EventArgs e)
    {
        ScriptManager oScriptManager = ScriptManager.GetCurrent(Page);
        if (oScriptManager == null)
        {
            oScriptManager = new ScriptManager { ID = "ScriptManager1" };
            Page.Controls.AddAt(0, oScriptManager);
        }

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        IntroText.Text = ExtractBodyFromHtmlString(parameters.GetParameter<string>("IntroText"));
        useCaptcha = parameters.GetParameter<bool>("UseCaptcha");
        ValidationSummary.HeaderText = StringResourceSystemFacade.GetString("Composite.Forms.Renderer", "Composite.Forms.ValidationSummary.HeaderText");
        var sendButtonLabel = parameters.GetParameter<string>("SendButtonLabel");
        if (sendButtonLabel != string.Empty)
            Send.Text = GetLocalized(sendButtonLabel);

        var resetButtonLabel = parameters.GetParameter<string>("ResetButtonLabel");
        Reset.Value = GetLocalized(resetButtonLabel);
        if (resetButtonLabel == string.Empty)
            Reset.Visible = false;

        FormsRenderer.InsertForm(this.Fields, parameters);
    }

    private static string GetLocalized(string text)
    {
        return text.Contains("${") ? StringResourceSystemFacade.ParseString(text) : text;
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        InsertCssIntoHeader("//code.jquery.com/ui/1.10.4/themes/cupertino/jquery-ui.css");
        InsertScriptIntoHeader("http://ajax.aspnetcdn.com/ajax/jquery.validate/1.7/jquery.validate.min.js");
        InsertScriptIntoHeader("//code.jquery.com/ui/1.10.4/jquery-ui.min.js");

        if (i18n.Contains(CultureInfo.CurrentCulture.Name))
        {
            InsertScriptIntoHeader(string.Format("http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.7/i18n/jquery.ui.datepicker-{0}.js", CultureInfo.CurrentCulture.Name));
        }
        else if (i18n.Contains(CultureInfo.CurrentCulture.TwoLetterISOLanguageName))
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
        var script = InsertScriptIntoHeader();
        script.InnerHtml = @"
<!--

$(document).ready(function() {
    $('.InputDate').datepicker();
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
            CaptchaText.Text = StringResourceSystemFacade.GetString("Composite.Forms.Renderer", "Composite.Forms.Captcha.CaptchaText.label");
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
        var url = new UrlBuilder(Composite.Core.WebClient.UrlUtils.PublicRootPath + "/Renderers/Captcha.ashx");
        url["value"] = encryptedCaptchaValue;
        return url.ToString();
    }


    private static string NumberDecimalSeparator()
    {
        return CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "." ? "\\." : CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
    }


    protected HtmlGenericControl InsertScriptIntoHeader(string link, string id = "")
    {
        HtmlGenericControl script = InsertScriptIntoHeader();
        script.Attributes.Add("src", link);

        if (!string.IsNullOrEmpty(id))
        {
            script.Attributes.Add("id", id);
        }
        return script;
    }

    protected HtmlGenericControl InsertScriptIntoHeader()
    {
        var script = new HtmlGenericControl("script");
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
                var responseText = parameters.GetParameter<string>("ResponseText");
                var responsePageId = parameters.GetParameter<string>("ResponseUrl");

                try
                {
                    string responsePageUrl;
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

    protected string ExtractBodyFromHtmlString(string htmlString)
    {
        if (htmlString.Contains("<body />"))
        {
            return string.Empty;
        }
        if (!string.IsNullOrEmpty(htmlString))
        {
            var startIndex = htmlString.IndexOf("<body>");
            var endIndex = htmlString.IndexOf("</body>");
            htmlString = htmlString.Substring(startIndex + 6, endIndex - startIndex - 6);
        }

        return htmlString;
    }
}
