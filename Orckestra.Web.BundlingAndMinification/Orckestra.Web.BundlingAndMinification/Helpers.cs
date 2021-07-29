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

        internal static bool ValidateFilePaths(HashSet<string> pageFilePaths)
        {
            foreach(var el in pageFilePaths)
            {
                var physicalPath = HostingEnvironment.MapPath(el);
                if (!File.Exists(physicalPath))
                {
                    PackageStateManager.SetCriticalState();
                    Log.LogError(AppNameForLogs, 
                        $"During the validation the virtual path {el} was mapped " +
                            $"to the physical path {physicalPath} and it does not exist.");
                    return false;
                }
            }
           
            return true;
        }

        //internal static bool IsFileSharingException(IOException iOException)
        //{
        //    if (iOException == null) return false;
        //    int errorCode = Marshal.GetHRForException(iOException) & ((1 << 16) - 1);
        //    //32 (ERROR_SHARING_VIOLATION) and 33 (ERROR_LOCK_VIOLATION) are file sharing issues in case of multiinstance
        //    if (errorCode == 32 || errorCode == 33)
        //    {
        //        Log.LogWarning(AppNameForLogs, iOException);
        //        return true;
        //    }
        //    return false;
        //}
    }
}