using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Composite.Core.IO;

namespace Composite.Web.Css.Less
{
    public static class CompressFiles
    {
        private static readonly string LessCompilerFilePath = HostingEnvironment.MapPath("~/Frontend/Composite/Web/Css/Less/lessc.cmd");

        public static void CompressLess(string lessFilePath, string cssFilePath, DateTime folderLastUpdatedUtc)
        {
            var scriptProc = InitProcess(lessFilePath);
            scriptProc.Start();

            string output = scriptProc.StandardOutput.ReadToEnd();
            string error = scriptProc.StandardError.ReadToEnd();

            if (scriptProc.ExitCode == 2)
            {
                throw new CssCompileException(error);
            }

            if (scriptProc.ExitCode != 0)
            {
                throw new InvalidOperationException("Compiling less caused a scripting host error. " + error);
            }

            C1File.WriteAllText(cssFilePath, output);

            File.SetLastWriteTimeUtc(cssFilePath, folderLastUpdatedUtc);
        }

        /// <summary>
        /// </summary>
        /// <param name="virtualLessFilePath">virtual path to the LESS file</param>
        /// <returns> virtual path to the compressed CSS file</returns>
        public static string CompressLess(string virtualLessFilePath)
        {
            var absouteLessFilePath = HttpContext.Current.Server.MapPath(virtualLessFilePath);
            var scriptProc = InitProcess(absouteLessFilePath);

            scriptProc.Start();

            string output = scriptProc.StandardOutput.ReadToEnd();
            string error = scriptProc.StandardError.ReadToEnd();

            if (scriptProc.ExitCode == 2)
            {
                throw new CssCompileException(error);
            }

            if (scriptProc.ExitCode != 0)
            {
                throw new InvalidOperationException("Compiling less caused a scripting host error. " + error);
            }

            var filePathCss = absouteLessFilePath.Substring(0, absouteLessFilePath.Length - ".less".Length) + ".min.css";
            C1File.WriteAllText(filePathCss, output);

            return virtualLessFilePath.Substring(0, virtualLessFilePath.Length - ".less".Length) + ".min.css";
        }

        private static Process InitProcess(string filePath)
        {
            return new Process
            {
                StartInfo =
                {
                    FileName = "\"" + LessCompilerFilePath + "\"",
                    Arguments = filePath,
                    WorkingDirectory = Path.GetDirectoryName(filePath),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
        }
    }
}
