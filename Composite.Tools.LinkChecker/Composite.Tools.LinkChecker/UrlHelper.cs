using System;
using Composite.Core.Extensions;
using Composite.Core.WebClient;

namespace Composite.Tools.LinkChecker
{
    internal static class UrlHelper
    {
        public static bool IsHttpLink(string link)
        {
            if (string.IsNullOrEmpty(link) || link.StartsWith("#")) return false;

            if (link.StartsWith("/")
                || IsAbsoluteLink(link))
            {
                return true;
            }

            string[] parts = link.Split('/');
            return !parts[0].Contains(":");
        }

        public static bool IsAbsoluteLink(string path)
        {
            return path.StartsWith("//")
                   || path.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                   || path.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }

        public static string ToAbsoluteUrl(string link, string baseUrl, string serverUrl/*, bool isSecure*/)
        {
            if (link.StartsWith("//"))
            {
                bool isSecure = serverUrl != null &&
                                serverUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

                return "http{0}://{1}".FormatWith(isSecure ? "s" : "", link);
            }

            if (IsAbsoluteLink(link))
            {
                return link;
            }

            if (!link.StartsWith("/"))
            {
                if (!IsAbsoluteLink(baseUrl))
                {
                    baseUrl = UrlUtils.Combine(serverUrl, baseUrl);
                }

                return UrlUtils.Combine(baseUrl, link);
            }

            return UrlUtils.Combine(serverUrl, link);
        }
    }
}
