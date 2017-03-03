using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Composite.Core.IO;
using Composite.Web.BundlingAndMinification.Api;

namespace Orckestra.Web.Css.Less
{
    public class LessCssCompiler: ICssCompiler
    {
        private static readonly string LessCompilerFilePath = HostingEnvironment.MapPath("~/Frontend/Orckestra/Web/Css/Less/lessc.cmd");
        private const string FileExtention = ".less";
        public bool SupportsExtension(string extension)
        {
            return string.Compare(extension, FileExtention, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public string CompileCss(string sourceFilePath)
        {
            var absouteLessFilePath = HttpContext.Current.Server.MapPath(sourceFilePath);
            var scriptProc = InitProcess(absouteLessFilePath);

            string output = GetOutput(scriptProc);

            var filePathCss = absouteLessFilePath.Substring(0, absouteLessFilePath.Length - FileExtention.Length) + ".min.css";
            C1File.WriteAllText(filePathCss, output);

            return sourceFilePath.Substring(0, sourceFilePath.Length - ".less".Length) + ".min.css";

        }

        public static void CompileCss(string lessFilePath, string cssFilePath, DateTime folderLastUpdatedUtc)
        {
            var scriptProc = InitProcess(lessFilePath);
            string output = GetOutput(scriptProc);

            C1File.WriteAllText(cssFilePath, output);

            File.SetLastWriteTimeUtc(cssFilePath, folderLastUpdatedUtc);
        }

        private static string GetOutput(Process process)
        {
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            if (process.ExitCode == 2)
            {
                throw new CssCompileException(error);
            }

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException("Compiling less caused a scripting host error. " + error);
            }

            return output;
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
