using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Composite.Web.Js.TypeScript
{
    public static class TypeScriptCompiler
    {
        public static void Compile(string baseDir, string outputFile, params string[] options)
        {
            var tscPath = Path.Combine(baseDir,"bin", "tsc", "tsc.exe");
            if (!File.Exists(tscPath))
            {
                throw new TypeScriptCompileException($"TypeScript compiler not found '{tscPath}'");
            }
                
            
            var arguments = string.Join(" ", options ?? new string[0]);
            arguments += $" --rootDir {baseDir}";
            var process = Process.Start( new ProcessStartInfo()
            {
                FileName = tscPath,
                Arguments = arguments,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = baseDir,
                CreateNoWindow = true,
            });
            var res = process.WaitForExit(5 * 60 * 1000);

            string output = null;
            try
            {
                var standardOutput = process.StandardOutput.ReadToEnd();
                var errorOutput = process.StandardError.ReadToEnd();
                output = standardOutput + Environment.NewLine + errorOutput;
            }
            catch
            {
            }
            
            if (!res)
            {
                process.Kill();
                throw new TypeScriptCompileException("Compilation timed out", output);
            }

            if (process.ExitCode != 0)
            {
                throw new TypeScriptCompileException("Compilation errors", output);
            }
        }
    };
}
