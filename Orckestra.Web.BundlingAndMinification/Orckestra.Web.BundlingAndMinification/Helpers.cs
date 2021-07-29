﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Composite.Core;
using static Orckestra.Web.BundlingAndMinification.CommonValues;

namespace Orckestra.Web.BundlingAndMinification
{
    /// <summary>
    /// Additional helper operations
    /// </summary>
    internal static class Helpers
    {
        internal static string GetMD5Hash(this string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            using (MD5 md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", "");
            }
        }

        //Kept as in the previous package version
        internal static string GetVirtualPath(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            if (url.StartsWith("//"))
            {
                url = $"http:{url}";
            }

            bool createRes = Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri uri);
            if (!createRes || (uri.IsAbsoluteUri && uri.Host != HttpContext.Current.Request.Url.Host)) return null;

            if (uri.IsAbsoluteUri)
            {
                url = uri.AbsolutePath;
            }

            return url.StartsWith("~") ? url : "~" + url;
        }
    }
}