using System;
using System.Text;
using System.Web.Optimization;
using Composite.Core;
using NUglify;
using static Orckestra.Web.BundlingAndMinification.CommonValues;

namespace Orckestra.Web.BundlingAndMinification.Customizations
{
    /// <summary>
    /// Class implementing styles minification with NUglify
    /// </summary>
    public class CustomCssMinify : IBundleTransform
    {
        private static readonly string _cssContentType = "text/css";
        public virtual void Process(BundleContext context, BundleResponse response)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (response == null) throw new ArgumentNullException(nameof(response));

            if (!context.EnableInstrumentation)
            {
                var uglifyResult = Uglify.Css(response.Content);
                if (uglifyResult.HasErrors && uglifyResult.Errors != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(var el in uglifyResult.Errors)
                    {
                        sb.AppendLine($"{el.ErrorCode} - {el.Message}");
                    }
                    Log.LogWarning(AppNameForLogs, sb.ToString());
                }
                response.Content = uglifyResult.Code;
            }
            response.ContentType = _cssContentType;
        }
    }
}