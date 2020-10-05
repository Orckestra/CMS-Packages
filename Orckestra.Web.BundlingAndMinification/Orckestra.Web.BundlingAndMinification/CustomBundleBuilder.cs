using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;

namespace Orckestra.Web.BundlingAndMinification
{
    public class CustomBundleBuilder : IBundleBuilder
    {
        public string BuildBundleContent(Bundle bundle, BundleContext context, IEnumerable<BundleFile> files)
        {
            if (files == null) return string.Empty;
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));
            StringBuilder stringBuilder = new StringBuilder();

            string str1 = null;
            if (!string.IsNullOrEmpty(bundle.ConcatenationToken))
            {
                str1 = bundle.ConcatenationToken;
            }

            foreach (BundleFile file in files)
            {
                string text = null;
                using (StreamReader streamReader = new StreamReader(HostingEnvironment.MapPath(file.IncludedVirtualPath)))
                {
                    text = streamReader.ReadToEnd();
                }

                if (string.IsNullOrWhiteSpace(text)) continue;

                text = ReplacePathsToAbsolute(file.VirtualFile.VirtualPath, text);

                if (file.Transforms != null && file.Transforms.Count > 0)
                {
                    foreach (IItemTransform transform in file.Transforms)
                    {
                        text = transform.Process(file.IncludedVirtualPath, text);
                    }
                }

                stringBuilder.Append(text);
                stringBuilder.Append(str1);
            }

            return stringBuilder.ToString();
        }

        private string ReplacePathsToAbsolute(string virtualPathOrigin, string text)
        {
            var pattern = "url\\s*\\((?!\\s*['\"]?\\s*(?:data:|http:|https:|\\/|.\\s*\\/)\\s*['\"]?)\\s*['\"]?\\s*(.+?)\\s*['\"]?\\s*\\)";
            string virtualDirectoryOrigin = Path.GetDirectoryName(virtualPathOrigin);
            text = Regex.Replace(text,
                pattern,
                m => m.Value.Replace(m.Groups[1].Value,
                VirtualPathUtility.ToAbsolute(Path.Combine(virtualDirectoryOrigin, m.Groups[1].Value))));
            return text;
        }
    }
}