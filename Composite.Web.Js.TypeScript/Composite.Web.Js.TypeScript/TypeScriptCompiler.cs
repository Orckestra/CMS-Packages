using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUglify;
using NUglify.JavaScript;

namespace Composite.Web.Js.TypeScript
{
    public static class TypeScriptCompiler
    {
        public static void Compile(TypeScriptConfig config, string baseDir)
        {
            var tscPath = Path.Combine(baseDir,"bin", "tsc", "tsc.exe");
            if (!File.Exists(tscPath))
            {
                throw new TypeScriptCompileException($"TypeScript compiler not found '{tscPath}'");
            }
                
            var arguments = string.Join(" ", config.Options);
            arguments += $" --rootDir {baseDir}";
            arguments += $" --outFile {config.BundleLocation}";
            var tscProcess = Process.Start( new ProcessStartInfo()
            {
                FileName = tscPath,
                Arguments = arguments,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = baseDir,
                CreateNoWindow = true,
            });
            var tscProcessResult = tscProcess.WaitForExit(5 * 60 * 1000);

            string output = null;
            try
            {
                var standardOutput = tscProcess.StandardOutput.ReadToEnd();
                var errorOutput = tscProcess.StandardError.ReadToEnd();
                output = standardOutput + Environment.NewLine + errorOutput;
            }
            catch
            {
                // ignored
            }

            if (!tscProcessResult)
            {
                tscProcess.Kill();
                throw new TypeScriptCompileException("Compilation timed out", output);
            }

            if (tscProcess.ExitCode != 0)
            {
                throw new TypeScriptCompileException("Compilation errors", output);
            }

            if (config.Minify)
            {
                var bundleFilePath = Path.Combine(baseDir, config.BundleLocation);
                var bundleContent = File.ReadAllText(bundleFilePath);
                var result = Uglify.Js(bundleContent, new CodeSettings { MinifyCode = true });
                if (result.HasErrors)
                    throw new TypeScriptCompileException("Minification error", string.Join("\n\r", result.Errors.Select(x => x.ToString())));

                File.WriteAllText(bundleFilePath, result.Code);
            }
        }
    };
}
