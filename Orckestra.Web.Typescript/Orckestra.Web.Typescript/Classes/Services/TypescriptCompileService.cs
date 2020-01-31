using NUglify;
using NUglify.JavaScript;
using Orckestra.Web.Typescript.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using static Orckestra.Web.Typescript.Classes.Helper;

namespace Orckestra.Web.Typescript.Classes.Services
{
    public class TypescriptCompileService : TypescriptService, ITypescriptCompileService
    {
        private readonly object _locker = new object();

        private string _taskName;
        private string _baseDirPath;
        private int _compilerTimeOutSeconds;
        private string _absolutePathConfigFile;
        private string _pathOutFile;
        private bool _allowOverwrite;
        private bool _useMinification;
        private string _minifiedName;
        
        public void ConfigureService(
            string taskName,
            string baseDirPath,
            int compilerTimeOutSeconds,
            string pathConfigFile,
            bool allowOverwrite,
            bool useMinification,
            string minifiedName)
        {
            _configured = false;
            _invoked = false;

            string warnMessage = ComposeExceptionInfo(nameof(ConfigureService), _taskName);

            _taskName = taskName;

            if (string.IsNullOrEmpty(baseDirPath))
            {
                RegisterException($"{warnMessage} Param {nameof(baseDirPath)} is null or empty.", typeof(ArgumentNullException));
                return;
            }
            else if (!Directory.Exists(baseDirPath))
            {
                RegisterException($"{warnMessage} Directory {baseDirPath} does not exist.", typeof(DirectoryNotFoundException));
                return;
            }
            _baseDirPath = baseDirPath;

            if (compilerTimeOutSeconds <= 0)
            {
                RegisterException($"{warnMessage} Param {compilerTimeOutSeconds} cannot be zero or negative.", typeof(ArgumentException));
                return;
            }
            _compilerTimeOutSeconds = compilerTimeOutSeconds;

            if (string.IsNullOrEmpty(pathConfigFile))
            {
                RegisterException($"{warnMessage} Param {nameof(pathConfigFile)} is null or empty.", typeof(ArgumentNullException));
                return;
            }

            _absolutePathConfigFile = HostingEnvironment.MapPath(pathConfigFile);
            if (string.IsNullOrEmpty(_absolutePathConfigFile))
            {
                RegisterException($"{warnMessage} Cannot get absolute path of {nameof(pathConfigFile)}.", typeof(ArgumentNullException));
                return;
            }
            else if (!File.Exists(_absolutePathConfigFile))
            {
                RegisterException($"{warnMessage} File {_absolutePathConfigFile} does not exist.", typeof(FileNotFoundException));
                return;
            }

            try
            {
                dynamic tsconfigObj = Newtonsoft.Json.JsonConvert
                    .DeserializeAnonymousType<dynamic>(File.ReadAllText(_absolutePathConfigFile), null);
                _pathOutFile = tsconfigObj?.compilerOptions?.outFile;
            }
            catch (Exception ex)
            {
                RegisterException(ex);
                return;
            }
            _allowOverwrite = allowOverwrite;
            _useMinification = useMinification;
            _minifiedName = minifiedName;

            _configured = true;
        }

        public void InvokeService()
        {
            _invoked = false;

            string warnMessage = ComposeExceptionInfo(nameof(InvokeService), _taskName);

            if (!_configured)
            {
                RegisterException($"{warnMessage} Service is not configured.", typeof(InvalidOperationException));
                return;
            }
            else if (!_allowOverwrite && !string.IsNullOrEmpty(_absolutePathConfigFile) && File.Exists(_absolutePathConfigFile))
            {
                _invoked = true;
                return;
            }

            string compilatorPath = Path.Combine(_baseDirPath, "Bin", "TypescriptCompiler", "tsc.exe");
            if (!File.Exists(compilatorPath))
            {
                RegisterException($"{warnMessage} Cannot find compilator exetuable on the path {compilatorPath}.", typeof(FileNotFoundException));
                return;
            }
            // Strict lock to avoid filesystem errors in case of too fast changes or dublicated filechanged events (OS batches writing)
            lock (_locker)
            {
                Process tscProcess;
                try
                {
                    tscProcess = Process.Start(new ProcessStartInfo()
                    {
                        FileName = compilatorPath,
                        Arguments = $"--project {_absolutePathConfigFile}",
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        WorkingDirectory = _baseDirPath,
                        CreateNoWindow = true
                    });
                }
                catch (Exception ex)
                {
                    RegisterException(ex);
                    return;
                }

                List<string> output = new List<string>();
                //Output readings are separated to avoid deadlocks and perfomance issues
                while (tscProcess.StandardOutput.Peek() > -1)
                {
                    string line = tscProcess.StandardOutput.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    output.Add(line);
                }
                while (tscProcess.StandardError.Peek() > -1)
                {
                    string line = tscProcess.StandardError.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    output.Add(line);
                }

                bool tscProcessResult = tscProcess.WaitForExit(_compilerTimeOutSeconds * 1000);
                if (!tscProcessResult)
                {
                    if (!tscProcess.HasExited)
                    {
                        try
                        {
                            tscProcess.Kill();
                        }
                        catch (Exception ex)
                        {
                            RegisterException(ex);
                            return;
                        }
                    }
                    RegisterException($"{warnMessage} Typescript timeouted. Config file: {_absolutePathConfigFile}.", 
                        typeof(TimeoutException));
                    return;
                }

                if (tscProcess.ExitCode != 0)
                {
                    foreach(string el in output)
                    {
                        Match cl = Regex.Match(el, @"^(.+?\.ts)\(([0-9]+),([0-9]+)\)(.+)");
                        if (cl.Success)
                        {
                            ConfigurationErrorsException cee = new ConfigurationErrorsException(
                                    string.Concat(warnMessage, " Details ", cl.Groups[4].Value), 
                                    Path.Combine(_baseDirPath, cl.Groups[1].Value.Replace("/", "\\")),
                                    int.Parse(cl.Groups[2].Value));                            
                            RegisterException(cee);
                        }
                        else
                        {
                            RegisterException($"{warnMessage} Compilation ended with code {tscProcess.ExitCode}. " +
                                $"Config file: {_absolutePathConfigFile}. Additional info: {string.Join(",", output)}", typeof(Exception));
                        }
                    }
                    return;
                }
                //kept minifing feature, but also BundlingAndMinification package could be used
                if (_useMinification)
                {
                    if (string.IsNullOrEmpty(_minifiedName))
                    {
                        RegisterException($"{warnMessage} Cannot minify file, no {nameof(_minifiedName)} value", typeof(ArgumentNullException));
                        return;
                    }
                    try
                    {
                        string originalFilePath = Path.Combine(
                            _baseDirPath,
                            Path.GetDirectoryName(_absolutePathConfigFile),
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
                    catch (Exception ex)
                    {
                        RegisterException(ex);
                        return;
                    }
                }
            }
            _invoked = true;
        }

        public void ResetInvokeState() => _invoked = false;
    }
}