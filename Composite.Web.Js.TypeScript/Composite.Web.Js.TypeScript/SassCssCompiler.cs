using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using Composite.Core.IO;

namespace Composite.Web.Js.TypeScript
{
    public static class TypeScriptCompiler
    {
        public static void Compile(params string[] options)
        {
            //var rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //var tscPath = Path.Combine(rootPath, "tools", "tsc.exe");
            var process = Process.Start( new ProcessStartInfo()
            {
                FileName = "c:\\_\\Desktop\\TypeScriptCompile\\packages\\Microsoft.TypeScript.Compiler.3.1.5\\tools\\tsc.exe",
                Arguments = string.Join(" ", options ?? new string[0]),
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            });
            var res = process.WaitForExit(5 * 60 * 1000);
            string output = process.StandardOutput.ReadToEnd();
            string err = process.StandardError.ReadToEnd();

            if (!res)
            {
                process.Kill();
                throw new Exception("Compilation timed out");
            }

            if (process.ExitCode != 0)
            {
                throw new Exception("Compilation errors");
            }
        }
    };
    

    //public class SassCssCompiler : ICssCompiler
    //{
    //    private const string  FileExtention = ".scss";
    //    public bool SupportsExtension(string extension)
    //    {
    //        return string.Compare(extension, FileExtention, StringComparison.InvariantCultureIgnoreCase) == 0;
    //    }

    //    /// <summary>
    //    ///
    //    /// </summary>
    //    /// <param name="sourceFilePath">virtual path to the Sass file</param>
    //    /// <returns>virtual path to the Css file</returns>
    //    public string CompileCss(string sourceFilePath)
    //    {
    //        var absoluteSassFilePath = HttpContext.Current.Server.MapPath(sourceFilePath);
    //        var filePathCss = absoluteSassFilePath.Substring(0, absoluteSassFilePath.Length - FileExtention.Length) + ".min.css";

    //        CompileCss(absoluteSassFilePath, filePathCss);

    //        return sourceFilePath.Substring(0, sourceFilePath.Length - FileExtention.Length) + ".min.css";
    //    }

    //    public static void CompileCss(string sassFilePath, string cssFilePath, DateTime? folderLastUpdatedUtc = null)
    //    {
    //        var sassCompiler = new SassCompiler(new SassOptions
    //        {
    //            InputPath = sassFilePath,
    //            OutputStyle = SassOutputStyle.Compact,
    //            IncludeSourceComments = false,
    //            IncludeSourceMapContents = true
    //        });


    //        var result = sassCompiler.Compile();


    //        if (result.ErrorStatus != 0)
    //        {
    //            throw new CssCompileException("Compiling sass caused a scripting host error. " +
    //                string.Format("Error status: {0}. File: {1}. Line: {2}. Column: {3}. Message: {4}", result.ErrorStatus, result.ErrorFile, result.ErrorLine, result.ErrorColumn, result.ErrorMessage));
    //        }

    //        C1File.WriteAllText(cssFilePath, result.Output);

    //        if (folderLastUpdatedUtc.HasValue)
    //        {
    //            File.SetLastWriteTimeUtc(cssFilePath, folderLastUpdatedUtc.Value);
    //        }
    //    }

    //}
}
