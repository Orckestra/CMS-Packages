using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Orckestra.Web.BundlingAndMinification
{
    internal static class Helpers
    {
        internal static string GetMD5Hash(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            using (MD5 md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", "").ToLower();
            }
        }

        //Kept as in the previous package version
        internal static string GetVirtualPath(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            else if (url.StartsWith("//"))
            {
                url = $"http:{url}";
            }

            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri uri) 
                || (uri.IsAbsoluteUri && uri.Host != HttpContext.Current.Request.Url.Host))
            {
                return null;
            }

            if (uri.IsAbsoluteUri)
            {
                url = uri.AbsolutePath;
            }
            return url.StartsWith("~") ? url : "~" + url;
        }
    }
}