<%@ WebHandler Language="C#" Class="CaptchaImageCreator" %>

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;

using Composite.Packages.FormsRenderer.Captcha;

public class CaptchaImageCreator : IHttpHandler
{
	#region private const
	private const string IMAGE_JPEG = "image/jpeg";
	private const string WIDTH = "Width";
	private const string HEIGHT = "Height";
	private const string CIPHER_TEXT = "CipherText";
	private const string BACKGROUND_COLOR = "BackGroundColor";
	private const string FONT_WARP = "FontWarp";
	private const string FONT_COLOR = "FontColor";
	private const string NOISE = "Noise";
	private const string NOISE_COLOR = "NoiseColor";
	private const string LINE_NOISE = "LineNoise";
	private const string LINE_NOISE_COLOR = "LineNoiseColor";
	#endregion

	#region private vars
	private ImageCreator ic;
	private HttpContext CurrentContext;
	private string _CipherText;
	private Color _BackgroundColor = Color.White;
	private FontWarpFactor _FontWarpFactor = FontWarpFactor.Low;
	private Color _FontColor = Color.Black;
	private NoiseLevel _NoiseLevel = NoiseLevel.Low;
	private Color _NoiseColor = Color.Black;
	private LineNoiseLevel _LineNoiseLevel = LineNoiseLevel.Low;
	private Color _LineNoiseColor = Color.Black;
	#endregion

	#region private properties
	private int Width
	{
		get
		{
			try
			{
				return int.Parse(CurrentContext.Request.Params[WIDTH]);
			}
			catch (ArgumentNullException)
			{
				return 160;
			}
		}
	}
	private int Height
	{
		get
		{
			try
			{
				return int.Parse(CurrentContext.Request.Params[HEIGHT]);
			}
			catch (ArgumentNullException)
			{
				return 40;
			}
		}
	}
	private string CipherText
	{
		get
		{
			if (String.IsNullOrEmpty(_CipherText))
			{
				_CipherText = CurrentContext.Request.Params[CIPHER_TEXT];
			}
			return _CipherText;
		}
	}
	#endregion

	public void ProcessRequest (HttpContext context)
	{
		CurrentContext = context;
		ic = new ImageCreator(Width, Height);
		_CipherText = String.Empty;
		ReadSettings();
		ConfigureCaptcha();
		Bitmap image = ic.CreateImage(CipherText);
		image.Save(CurrentContext.Response.OutputStream, ImageFormat.Jpeg);
		image.Dispose();
		CurrentContext.Response.ContentType = IMAGE_JPEG;
		CurrentContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
		CurrentContext.Response.StatusCode = 200;
		CurrentContext.Response.End();
	}

	private void ReadSettings()
	{
		try
		{
			if (!String.IsNullOrEmpty(CurrentContext.Request.QueryString[BACKGROUND_COLOR]))
			{
				_BackgroundColor = ColorTranslator.FromHtml(CurrentContext.Request.QueryString[BACKGROUND_COLOR]);
			}
		}
		catch (Exception) {}

		try
		{
			if (!String.IsNullOrEmpty(CurrentContext.Request.QueryString[FONT_WARP]))
			{
				_FontWarpFactor = (FontWarpFactor)Enum.Parse(typeof(FontWarpFactor), CurrentContext.Request.QueryString[FONT_WARP]);
			}
		}
		catch (ArgumentNullException) { }
		catch (ArgumentException) { }

		try
		{
			if (!String.IsNullOrEmpty(CurrentContext.Request.QueryString[FONT_COLOR]))
			{
				_FontColor = ColorTranslator.FromHtml(CurrentContext.Request.QueryString[FONT_COLOR]);
			}
		}
		catch (Exception) { }

		try
		{
			if (!String.IsNullOrEmpty(CurrentContext.Request.QueryString[NOISE]))
			{
				_NoiseLevel = (NoiseLevel)Enum.Parse(typeof(NoiseLevel), CurrentContext.Request.QueryString[NOISE]);
			}
		}
		catch (ArgumentNullException) { }
		catch (ArgumentException) { }

		try
		{
			if (!String.IsNullOrEmpty(CurrentContext.Request.QueryString[NOISE_COLOR]))
			{
				_NoiseColor = ColorTranslator.FromHtml(CurrentContext.Request.QueryString[NOISE_COLOR]);
			}
		}
		catch (Exception) { }

		try
		{
			if (!String.IsNullOrEmpty(CurrentContext.Request.QueryString[LINE_NOISE]))
			{
				_LineNoiseLevel = (LineNoiseLevel)Enum.Parse(typeof(LineNoiseLevel), CurrentContext.Request.QueryString[LINE_NOISE]);
			}
		}
		catch (ArgumentNullException) { }
		catch (ArgumentException) { }

		try
		{
			if (!String.IsNullOrEmpty(CurrentContext.Request.QueryString[LINE_NOISE_COLOR]))
			{
				_LineNoiseColor = ColorTranslator.FromHtml(CurrentContext.Request.QueryString[LINE_NOISE_COLOR]);
			}
		}
		catch (Exception) { }
	}

	private void ConfigureCaptcha()
	{
		ic.BackgroundColor = _BackgroundColor;
		ic.FontWarp = _FontWarpFactor;
		ic.FontColor = _FontColor;
		ic.Noise = _NoiseLevel;
		ic.NoiseColor = _NoiseColor;
		ic.LineNoise = _LineNoiseLevel;
		ic.LineNoiseColor = _LineNoiseColor;
	}

	public bool IsReusable
	{
		get
		{
			return false;
		}
	}
}