using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web.Hosting;
using Composite.Core.IO;

namespace Composite.Web.Css.Sass
{
    public static class CompressFiles
    {
        private static readonly string SassCompilerFilePath = HostingEnvironment.MapPath("~/Frontend/Composite/Web/Css/Sass/sassc.cmd");

        public static void CompressSass(string sassFilePath, string cssFilePath, DateTime folderLastUpdatedUtc)
        {
            var scriptProc = new Process();
            scriptProc.StartInfo.FileName = "\"" + SassCompilerFilePath + "\"";
            scriptProc.StartInfo.Arguments = sassFilePath;
            scriptProc.StartInfo.WorkingDirectory = Path.GetDirectoryName(sassFilePath);
            scriptProc.StartInfo.RedirectStandardOutput = true;
            scriptProc.StartInfo.RedirectStandardError = true;
            scriptProc.StartInfo.CreateNoWindow = true;
            scriptProc.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            scriptProc.StartInfo.UseShellExecute = false;
            scriptProc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            scriptProc.Start();

            string output = scriptProc.StandardOutput.ReadToEnd();
            string error = scriptProc.StandardError.ReadToEnd();

            if (scriptProc.ExitCode == 2)
            {
                throw new CssCompileException(output);
            }

            if (scriptProc.ExitCode != 0)
            {
                throw new InvalidOperationException("Compiling sass caused a scripting host error. " + error);
            }

            C1File.WriteAllText(cssFilePath, output);

            File.SetLastWriteTimeUtc(cssFilePath, folderLastUpdatedUtc);
        }
    }
}
