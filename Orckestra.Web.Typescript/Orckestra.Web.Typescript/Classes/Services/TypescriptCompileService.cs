using NUglify;
using NUglify.JavaScript;
using Orckestra.Web.Typescript.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using static Orckestra.Web.Typescript.Classes.Helper;

namespace Orckestra.Web.Typescript.Classes.Services
{
    public class TypescriptCompileService : TypescriptService, ITypescriptCompileService
    {
        private string _taskName;
        private string _baseDirPath;
        private int _compilerTimeOutSeconds;
        private string _absolutePathConfigFile;
        private string _pathOutFile;
        private bool _allowOverwrite;
        private bool _useMinification;
        private string _minifiedName;

        private readonly ReaderWriterLockSlim _lc = new ReaderWriterLockSlim();
        private bool _recompile = true;
        private static readonly char[] _invalidPathChars = Path.GetInvalidPathChars();

        public bool ConfigureService(
            string taskName,
            string baseDirPath,
            int compilerTimeOutSeconds,
            string pathConfigFile,
            bool allowOverwrite,
            bool useMinification,
            string minifiedName)
        {
            string warnMessage = ComposeExceptionInfo(nameof(ConfigureService), _taskName);

            _taskName = taskName;

            if (string.IsNullOrEmpty(baseDirPath))
            {
                RegisterException($"{warnMessage} Param {nameof(baseDirPath)} is null or empty.", typeof(ArgumentNullException));
                return false;
            }
            else if (!Directory.Exists(baseDirPath))
            {
                RegisterException($"{warnMessage} Directory {baseDirPath} does not exist.", typeof(DirectoryNotFoundException));
                return false;
            }
            _baseDirPath = baseDirPath;

            if (compilerTimeOutSeconds <= 0)
            {
                RegisterException($"{warnMessage} Param {compilerTimeOutSeconds} cannot be zero or negative.", typeof(ArgumentException));
                return false;
            }
            _compilerTimeOutSeconds = compilerTimeOutSeconds;

            if (string.IsNullOrEmpty(pathConfigFile))
            {
                RegisterException($"{warnMessage} Param {nameof(pathConfigFile)} is null or empty.", typeof(ArgumentNullException));
                return false;
            }

            _absolutePathConfigFile = HostingEnvironment.MapPath(pathConfigFile);
            if (string.IsNullOrEmpty(_absolutePathConfigFile))
            {
                RegisterException($"{warnMessage} Cannot get absolute path of {nameof(pathConfigFile)}.", typeof(ArgumentNullException));
                return false;
            }
            else if (!File.Exists(_absolutePathConfigFile))
            {
                RegisterException($"{warnMessage} File {_absolutePathConfigFile} does not exist.", typeof(FileNotFoundException));
                return false;
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
                return false;
            }
            _allowOverwrite = allowOverwrite;
            _useMinification = useMinification;
            _minifiedName = minifiedName;

            return true;
        }

        public bool InvokeService()
        {
            if (!IsSourceChanged())
            {
                return false;
            }
            else if (!_allowOverwrite && !string.IsNullOrEmpty(_absolutePathConfigFile) && File.Exists(_absolutePathConfigFile))
            {
                SetSourceLast();
                return false;
            }
            bool result = false;
            try
            {
                _lc.EnterWriteLock();
                if (!_recompile)
                {
                    return false;
                }
                result = ExecCompilation();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _lc.ExitWriteLock();
            }
            return result;
        }

        private bool ExecCompilation()
        {
            _recompile = false;
            string warnMessage = ComposeExceptionInfo(nameof(InvokeService), _taskName);

            string compilerPath = Path.Combine(_baseDirPath, "Bin", "TypescriptCompiler", "tsc.exe");
            if (!File.Exists(compilerPath))
            {
                RegisterException($"{warnMessage} Cannot find compiler executable on the path \"{compilerPath}\".", typeof(FileNotFoundException));
                return false;
            }

            Process tscProcess;
            
            try
            {
                tscProcess = Process.Start(new ProcessStartInfo()
                {
                    FileName = compilerPath,
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
                return false;
            }
            
            List<string> output = new List<string>();
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
                        return false;
                    }
                }
                RegisterException($"{warnMessage} Typescript compilation timeout expired. Config file: {_absolutePathConfigFile}.",
                    typeof(TimeoutException));
                return false;
            }

            if (tscProcess.ExitCode != 0)
            {
                if (!output.Any())
                {
                    RegisterException($"{warnMessage} Compilation ended with code {tscProcess.ExitCode}. No additional info.", typeof(Exception));
                    return false;
                }
                foreach (string el in output)
                {
                    Match match = Regex.Match(output[0], @"^(.+?\.ts)\(([0-9]+),([0-9]+)\)(.+)");
                    string tpFile = match?.Groups[1]?.Value;
                    
                    if (match.Success & !tpFile.Where(x=> _invalidPathChars.Contains(x)).Any())
                    {
                        string path = Path.Combine(_baseDirPath, match.Groups[1].Value.Replace("/", "\\"));
                        int line = int.Parse(match.Groups[2].Value);
                        HttpParseException hpe = new HttpParseException(string.Concat(
                            warnMessage,
                            "Message", match.Groups[4].Value, Environment.NewLine,
                            "Path: ", path, Environment.NewLine,
                            "Error line: ", line), null, path, null, line);

                        RegisterException(hpe);
                    }
                    else
                    {
                        RegisterException($"{warnMessage} Compilation ended with code {tscProcess.ExitCode}. " +
                            $"Config file: {_absolutePathConfigFile}. Additional info: {string.Join(Environment.NewLine, output)}", typeof(Exception));
                        break;
                    }
                }
                return false;
            }
            //kept minifying feature, but also BundlingAndMinification package could be used
            if (_useMinification)
            {
                if (string.IsNullOrEmpty(_minifiedName))
                {
                    RegisterException($"{warnMessage} Cannot minify file, no {nameof(_minifiedName)} value", typeof(ArgumentNullException));
                    return false;
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
                        throw new Exception(string.Join(Environment.NewLine, result.Errors));
                    }
                    File.WriteAllText(minifiedFilePath, result.Code);
                }
                catch (Exception ex)
                {
                    RegisterException(ex);
                    return false;
                }
            }
            return true;
        }

        public void SetSourceLast()
        {
            _lc.EnterWriteLock();
            _recompile = false;
            _lc.ExitWriteLock();
        }

        public void SetSourceChanged()
        {
            _lc.EnterWriteLock();
            _recompile = true;
            _lc.ExitWriteLock();
        }

        public bool IsSourceChanged()
        {
            _lc.EnterReadLock();
            bool result = _recompile;
            _lc.ExitReadLock();
            return result;
        }
    }
}
