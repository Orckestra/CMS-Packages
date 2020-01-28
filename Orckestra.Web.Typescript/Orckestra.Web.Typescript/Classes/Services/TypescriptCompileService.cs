using Composite.Core;
using NUglify;
using NUglify.JavaScript;
using Orckestra.Web.Typescript.Classes;
using Orckestra.Web.Typescript.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Orckestra.Web.Typescript.Services
{
    public class TypescriptCompileService : ITypescriptCompileService
    {
        private string _baseDirPath;
        private int _compilerTimeOutSeconds;
        private string _pathConfigFile;
        private string _pathOutFile;
        private bool _cancelIfOutFileExist;
        private bool _useMinification;
        private string _minifiedName;

        public ITypescriptCompileService ConfigureService(
            string baseDirPath,
            int compilerTimeOutSeconds,
            string pathConfigFile,
            bool cancelIfOutFileExist,
            bool? useMinification,
            string minifiedName)
        {
            _baseDirPath = baseDirPath ?? throw new ArgumentNullException(nameof(baseDirPath));

            if (compilerTimeOutSeconds <= 0)
            {
                throw new ArgumentException("Compiler time out cannot be 0 or less, check package settings");
            }
            _compilerTimeOutSeconds = compilerTimeOutSeconds;

            _pathConfigFile = Helper.GetAbsoluteServerPath(pathConfigFile) ?? throw new ArgumentNullException(nameof(pathConfigFile));

            dynamic tsconfigObj = Newtonsoft.Json.JsonConvert
                .DeserializeAnonymousType<dynamic>(File.ReadAllText(_pathConfigFile), new { outFile = string.Empty });

            _pathOutFile = tsconfigObj?.compilerOptions?.outFile;
            if (string.IsNullOrEmpty(_pathOutFile) && useMinification == true)
            {
                throw new ArgumentException("Cannot use minification option if tsconfig.json outFile param is empty. " +
                    $"Set up outFile value in {_pathConfigFile} or use BundlingAndMinification package for bundling and minification instead.");
            }
            else if (useMinification == true && string.IsNullOrEmpty(minifiedName))
            {
                throw new ArgumentException("To use minification you have to set up minified name in package settings file");
            }

            _cancelIfOutFileExist = cancelIfOutFileExist;
            _useMinification = useMinification is null ? false : (bool)useMinification;
            _minifiedName = minifiedName;
            return this;
        }
        public ITypescriptCompileService InvokeService()
        {
            if (_cancelIfOutFileExist && !string.IsNullOrEmpty(_pathOutFile) && File.Exists(Helper.GetAbsoluteServerPath(_pathOutFile)))
            {
                Log.LogWarning(nameof(TypescriptCompileService), $"File {_pathOutFile} already exist, compilation cancelled, check settings");
                return this;
            }
            // Strict lock to avoid filesystem errors in case of too fast changes or dublicated filechanged events (writing in batches)
            lock (this)
            {
                Process tscProcess = Process.Start(new ProcessStartInfo()
                {
                    FileName = Path.Combine(_baseDirPath, "Bin", "TypescriptCompiler", "tsc.exe"),
                    Arguments = $"--project {_pathConfigFile}",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WorkingDirectory = _baseDirPath,
                    CreateNoWindow = true
                });

                StringBuilder output = new StringBuilder();
                //Output readings are separated to avoid deadlocks and perfomance issues
                while (tscProcess.StandardOutput.Peek() > -1)
                {
                    output.AppendLine(tscProcess.StandardOutput.ReadLine());
                }
                while (tscProcess.StandardError.Peek() > -1)
                {
                    output.AppendLine(tscProcess.StandardError.ReadLine());
                }

                bool tscProcessResult = tscProcess.WaitForExit(_compilerTimeOutSeconds * 1000);
                if (!tscProcessResult)
                {
                    if (!tscProcess.HasExited)
                    {
                        tscProcess.Kill();
                    }
                    string message = $"Typescript compiler timed out. Config file: {_pathConfigFile}";
                    throw new Exception(message);
                }

                if (tscProcess.ExitCode != 0)
                {
                    string message = $"Typescript compiler ended a task with errors. Config file: {_pathConfigFile}, output: {output.ToString()}";
                    throw new Exception(message);
                }
                //kept minifing feature, but also BundlingAndMinification package can be used
                if (_useMinification)
                {
                    string originalFilePath = Path.Combine(
                        _baseDirPath,
                        Path.GetDirectoryName(_pathConfigFile),
                        _pathOutFile);

                    string minifiedFilePath = Path.Combine(
                        Path.GetDirectoryName(originalFilePath),
                        _minifiedName);

                    string bundleContent = File.ReadAllText(originalFilePath);
                    UglifyResult result = Uglify.Js(bundleContent, new CodeSettings { MinifyCode = true });
                    if (result.HasErrors)
                    {
                        throw new Exception(string.Join(",", result.Errors));
                    }
                    File.WriteAllText(minifiedFilePath, result.Code);
                }
            }
            return this;
        }
    }
}
