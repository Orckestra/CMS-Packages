using System;
using System.Web.Optimization;
using NUglify;

namespace Orckestra.Web.BundlingAndMinification
{
	/// <summary>
	/// Class implementing styles minification through NUglify
	/// </summary>
	public class CustomCssMinify : IBundleTransform
	{
		private static string _cssContentType = "text/css";
		public virtual void Process(BundleContext context, BundleResponse response)
		{
			if (context == null) { throw new ArgumentNullException(nameof(context)); }
			if (response == null) { throw new ArgumentNullException(nameof(response)); }

			if (!context.EnableInstrumentation)
			{			
				response.Content = Uglify.Css(response.Content).Code;
			}
			response.ContentType = _cssContentType;
		}
	}
}