using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static Orckestra.Tool.TypescriptPreparing.Logger;

namespace Orckestra.Tool.TypescriptPreparing
{
    /// <summary>
    /// One time code to move all typescripts to target folder, and to update files references after moving.
    /// Have not noticed on some improvements moments as it is 1 time run
    /// </summary>
    internal class Program
    {
        private static string _srcPath;
        private static string _destPath;

        private static string _srcPathTypings;
        private static string _destPathTypings;

        private static string _destPathTypescripts;

        private static string _destPathTests;

        static void Main()
        {
            Log($"Before processing be sure you have backuped src folder", ConsoleColor.DarkYellow);
            Log("Please, pastle absolute path to the src folder of ReferenceApplication", ConsoleColor.DarkCyan);
            while(true)
            {
                string path = Console.ReadLine();
                if (!Directory.Exists(path))
                {
                    Log("Cannot find this location. Try again", ConsoleColor.DarkRed);
                    continue;
                }
                Directory.SetCurrentDirectory(path);
                break;
            }

            Log("#1. Checking paths", ConsoleColor.DarkGreen);
            CheckPaths();

            Log("#2. Getting typescripts to process", ConsoleColor.DarkGreen);
            List<string> typescripts = GetFilesToProcess("typescript");

            Log("#3. Transfering relative references inside typescripts to absolute according to the current paths", ConsoleColor.DarkGreen);
            SetAbsoluteInsideReferences(ref typescripts);

            Log("#4. Getting typings to process", ConsoleColor.DarkGreen);
            List<string> typings = GetFilesToProcess("typings");

            Log("#5. Transfering relative references inside typings to absolute according to the current paths", ConsoleColor.DarkGreen);
            SetAbsoluteInsideReferences(ref typings);

            Log("#6. Getting tests to process", ConsoleColor.DarkGreen);
            List<string> tests = GetFilesToProcess("tests");

            Log("#7. Transfering relative references inside tests to absolute according to the current paths", ConsoleColor.DarkGreen);
            SetAbsoluteInsideReferences(ref tests);

            Dictionary<string, string> movements = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            Log("#8. Moving typescripts to new destinations, saving moving history", ConsoleColor.DarkGreen);
            MoveTypescripts(typescripts, _srcPath, _destPathTypescripts, movements);

            Log("#9. Moving typings to new destinations, saving moving history", ConsoleColor.DarkGreen);
            MoveTypings(typings, _srcPathTypings, _destPathTypings, movements);

            Log("#10. Moving tests to new destinations, saving moving history", ConsoleColor.DarkGreen);
            MoveTests(tests, _srcPath, _destPathTests, movements);

            Log("#11. Replacing references according to the moving history, creating relative pathes", ConsoleColor.DarkGreen);
            SetRelativeInsideReferences(movements);

            Log("#12. Deleting source Typescripts folders", ConsoleColor.DarkGreen);
            DeleteSourceTypescriptsFolders(typescripts);

            Log("#10. Deleting source Typings folders", ConsoleColor.DarkGreen);
            DeleteSourceTypingsFolder();

            Log("Deleting source Tests folders", ConsoleColor.DarkGreen);
            DeleteSourceTestsFolders(tests);

            Log(new string('*', 30), ConsoleColor.DarkGreen);
            Log("Successfully completed, check the log", ConsoleColor.DarkGreen);
        }

        private static void CheckPaths()
        {
            _srcPath = GetAbsolutePath("./Composer");
            if (!Directory.Exists(_srcPath))
            {
                throw new FileNotFoundException(_srcPath);
            }

            _destPath = GetAbsolutePath("./Composer.CompositeC1/Composer.CompositeC1.Mvc");
            if (!Directory.Exists(_destPath))
            {
                throw new FileNotFoundException(_destPath);
            }
            _destPath = Path.Combine(_destPath, "UI.Package");
            if (!Directory.Exists(_destPath))
            {
                Log($"Directory {_destPath} does not exist. Creating it.", ConsoleColor.DarkCyan);
                Directory.CreateDirectory(_destPath);
            }


            #region typings
            _srcPathTypings = Path.Combine(_srcPath, "Composer.UI", "Source", "Typings"); 
            if (!Directory.Exists(_srcPathTypings))
            {
                throw new FileNotFoundException(_srcPathTypings);
            }

            _destPathTypings = Path.Combine(_destPath, "Typings");
            if (!Directory.Exists(_destPathTypings))
            {
                Log($"Directory {_destPathTypings} does not exist. Creating it.", ConsoleColor.DarkCyan);
                Directory.CreateDirectory(_destPathTypings);
            }
            #endregion typings

            #region typescripts
            _destPathTypescripts = Path.Combine(_destPath, "Typescript");
            if (!Directory.Exists(_destPathTypescripts))
            {
                Log($"Directory {_destPathTypescripts} does not exist. Creating it.", ConsoleColor.DarkCyan);
                Directory.CreateDirectory(_destPathTypescripts);
            }
            #endregion typescripts

            #region tests
            _destPathTests = Path.Combine(_destPath, "Tests");
            if (!Directory.Exists(_destPathTests))
            {
                Log($"Directory {_destPathTests} does not exist. Creating it.", ConsoleColor.DarkCyan);
                Directory.CreateDirectory(_destPathTests);
            }
            #endregion tests

            Log($"Successfully checked paths", ConsoleColor.DarkGreen);
        }

        private static List<string> GetFilesToProcess(string type)
        {
            List<string> files = Directory.GetFiles(_srcPath, "*.ts", SearchOption.AllDirectories).
                Where(r => Regex.Match(r,
                $@"{Regex.Escape(_srcPath)}\\.+?\.ui\\(.+?\\)?source\\{type}\\.+?\.ts", RegexOptions.IgnoreCase)
                .Success).ToList();

            if (files.Count.Equals(0))
            {
                throw new Exception("Nothing to process.");
            }
            Log($"Got {files.Count} files to process", ConsoleColor.DarkGreen);
            return files;
        }

        private static void SetAbsoluteInsideReferences(ref List<string> files)
        {
            int counter = default;
            foreach (string el in files)
            {
                string fileData = File.ReadAllText(el);
                if (string.IsNullOrEmpty(fileData))
                {
                    Log($"File {el} is null or empty.", ConsoleColor.DarkRed);
                    continue;
                }
                MatchCollection matches = Regex.Matches(fileData, "reference path=['|\"](.+?)['|\"]", RegexOptions.IgnoreCase);
                //just a simply control to catch possible errors
                MatchCollection regexControl = Regex.Matches(fileData, "reference path", RegexOptions.IgnoreCase);
                if (matches.Count != regexControl.Count)
                {
                    throw new Exception($"Regex pattern is wrong, some references in file {el} were skipped");
                }
                else if (regexControl.Count == 0)
                {
                    continue;
                }
                counter += regexControl.Count;
                foreach (Match dx in matches)
                {
                    string curRelativeRef = dx.Groups[1].Value;
                    string curAbsoluteRef = GetAbsolutePath(el, curRelativeRef);
                    if (!curAbsoluteRef.EndsWith(".ts", StringComparison.OrdinalIgnoreCase))
                    {
                        Log($"File {el} has bad reference {curAbsoluteRef} without extention. Fixing it", ConsoleColor.DarkRed);
                        curAbsoluteRef += ".ts";
                    }
                    if (!File.Exists(curAbsoluteRef))
                    {
                        throw new FileNotFoundException($"File {curAbsoluteRef} was not found");
                    }
                    fileData = fileData.Replace(curRelativeRef, curAbsoluteRef, StringComparison.OrdinalIgnoreCase);
                }
                File.WriteAllText(el, fileData);
            }
            files = files.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            Log($"{counter} references has been changed inside files to absolute", ConsoleColor.DarkGreen);
        }

        private static void MoveTypescripts(List<string> files, string sourcePath, string destPath, Dictionary<string, string> movements)
        {
            foreach (string el in files)
            {
                string newPath = el.Replace(sourcePath, destPath);
                Match uip = Regex.Match(newPath, "\\\\(Source\\\\Typescript\\\\)", RegexOptions.IgnoreCase);
                if (!uip.Success)
                {
                    throw new Exception("Cannot find source folder part");
                }
                newPath = newPath.Remove(uip.Groups[1].Index, uip.Groups[1].Value.Length);

                uip = Regex.Match(newPath, string.Concat(Regex.Escape(destPath), 
                    "\\\\(Composer\\.).+?(\\.UI)"), RegexOptions.IgnoreCase);
                if (!uip.Success)
                {
                    uip = Regex.Match(newPath, string.Concat(Regex.Escape(destPath), 
                    "\\\\(Composer\\.UI\\\\)"), RegexOptions.IgnoreCase);
                    if (!uip.Success)
                    {
                        throw new Exception("Cannot compine new path");
                    }
                    newPath = newPath.Remove(uip.Groups[1].Index, uip.Groups[1].Value.Length);
                }
                else
                {
                    newPath = newPath.Remove(uip.Groups[2].Index, uip.Groups[2].Value.Length);
                }

                if (File.Exists(newPath))
                {
                    throw new Exception("File already exist, files conflict");
                }

                DirectoryInfo neededFolder = Directory.GetParent(newPath);
                if (!neededFolder.Exists)
                {
                    Directory.CreateDirectory(neededFolder.FullName);
                }
                File.Move(el, newPath);
                movements.Add(el, newPath);
            }
            Log($"{files.Count()} files has been moved to new destination", ConsoleColor.DarkGreen);
        }

        private static void MoveTypings(List<string> files, string sourcePath, string destPath, Dictionary<string, string> movements)
        {
            foreach (string el in files)
            {
                string newPath = el.Replace(sourcePath, destPath);
           
                if (File.Exists(newPath))
                {
                    throw new Exception("File already exist, files conflict");
                }

                DirectoryInfo neededFolder = Directory.GetParent(newPath);
                if (!neededFolder.Exists)
                {
                    Directory.CreateDirectory(neededFolder.FullName);
                }
                File.Move(el, newPath);
                movements.Add(el, newPath);
            }
            Log($"{files.Count()} files has been moved to new destination", ConsoleColor.DarkGreen);
        }

        private static void MoveTests(List<string> files, string sourcePath, string destPath, Dictionary<string, string> movements)
        {
            foreach (string el in files)
            {
                string newPath = el.Replace(sourcePath, destPath);
                Match uip = Regex.Match(newPath, "\\\\(Source\\\\Tests\\\\unit\\\\)", RegexOptions.IgnoreCase);
                if (!uip.Success)
                {
                    throw new Exception("Cannot find source folder part");
                }
                newPath = newPath.Remove(uip.Groups[1].Index, uip.Groups[1].Value.Length);

                uip = Regex.Match(newPath, string.Concat(Regex.Escape(destPath),
                    "\\\\(Composer\\.).+?(\\.UI)"), RegexOptions.IgnoreCase);
                if (!uip.Success)
                {
                    uip = Regex.Match(newPath, string.Concat(Regex.Escape(destPath),
                    "\\\\(Composer\\.UI\\\\)"), RegexOptions.IgnoreCase);
                    if (!uip.Success)
                    {
                        throw new Exception("Cannot compine new path");
                    }
                    newPath = newPath.Remove(uip.Groups[1].Index, uip.Groups[1].Value.Length);
                }
                else
                {
                    newPath = newPath.Remove(uip.Groups[2].Index, uip.Groups[2].Value.Length);
                }

                if (File.Exists(newPath))
                {
                    throw new Exception("File already exist, files conflict");
                }

                DirectoryInfo neededFolder = Directory.GetParent(newPath);
                if (!neededFolder.Exists)
                {
                    Directory.CreateDirectory(neededFolder.FullName);
                }
                File.Move(el, newPath);
                movements.Add(el, newPath);
            }
            Log($"{files.Count()} files has been moved to new destination", ConsoleColor.DarkGreen);
        }

        private static void SetRelativeInsideReferences(Dictionary<string, string> movements)
        {
            int counter = default;
            foreach (string el in movements.Keys)
            {
                string movedToPath = movements[el];

                string fileData = File.ReadAllText(movedToPath);
                if (string.IsNullOrEmpty(fileData))
                {
                    continue;
                }
                MatchCollection matches = Regex.Matches(fileData, "reference path=['|\"](.+?)['|\"]", RegexOptions.IgnoreCase);
                MatchCollection regexControl = Regex.Matches(fileData, "reference path", RegexOptions.IgnoreCase);
                if (regexControl.Count == 0)
                {
                    continue;
                }
                else if (matches.Count != regexControl.Count)
                {
                    throw new Exception($"Regex pattern is wrong, some references in file {el} were skipped");
                }
                counter += regexControl.Count;
                foreach (Match dx in matches)
                {
                    string curAbsoluteRefOld = dx.Groups[1].Value;
                    if (!movements.ContainsKey(curAbsoluteRefOld))
                    {
                        throw new Exception("No movement history");
                    }
                    string curAbsoluteRefNew = movements[curAbsoluteRefOld];
                    string curRelativeRef = GetRelativePath(curAbsoluteRefNew, movedToPath);

                    string reverseValueForCheck = GetAbsolutePath(movedToPath, curRelativeRef);
                    if (!string.Equals(curAbsoluteRefNew, reverseValueForCheck))
                    {
                        throw new Exception("Something is wrong in paths transforming");
                    }
                    else if (!File.Exists(curAbsoluteRefNew))
                    {
                        throw new FileNotFoundException($"File {curAbsoluteRefNew} not found");
                    }
                    fileData = fileData.Replace(curAbsoluteRefOld, curRelativeRef, StringComparison.OrdinalIgnoreCase);
                }
                File.WriteAllText(movedToPath, fileData);
            }
            Log($"{counter} references has been changed inside files to relative according to new pathes", ConsoleColor.DarkGreen);
        }

        private static void DeleteSourceTypescriptsFolders(List<string> files)
        {
            foreach (string el in files)
            {
                string sdr = "\\Source\\Typescript\\";
                int sIndex = el.LastIndexOf(sdr, StringComparison.OrdinalIgnoreCase);
                if (sIndex == -1)
                {
                    throw new Exception("Cannot find path part");
                }
                string path = el.Remove(sIndex + sdr.Length);
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    Log($"Source directory {path} has been deleted", ConsoleColor.DarkGreen);
                }
            }
        }

        private static void DeleteSourceTestsFolders(List<string> files)
        {
            foreach (string el in files)
            {
                string sdr = "\\Source\\Tests\\";
                int sIndex = el.LastIndexOf(sdr, StringComparison.OrdinalIgnoreCase);
                if (sIndex == -1)
                {
                    throw new Exception("Cannot find path part");
                }
                string path = el.Remove(sIndex + sdr.Length);
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                    Log($"Source directory {path} has been deleted", ConsoleColor.DarkGreen);
                }
            }
        }

        private static void DeleteSourceTypingsFolder()
        {
            Directory.Delete(_srcPathTypings, true);
            Log($"Source directory {_srcPathTypings} has been deleted", ConsoleColor.DarkGreen);
        }

        private static string GetRelativePath(string relativeTo, string currentPath)
        {
            string path = Path.GetRelativePath(Directory.GetParent(currentPath).FullName, relativeTo);
            //a bit beautify
            if (path.IndexOf("..") != 0)
            {
                path = ".\\" + path;
            }
            path = path.Replace("\\", "/");
            return path;
        }

        private static string GetAbsolutePath(string currentPath, string relativePath)
        {
            Directory.SetCurrentDirectory(Directory.GetParent(currentPath).FullName);
            string absolutePath = Path.GetFullPath(relativePath);
            return absolutePath;
        }

        private static string GetAbsolutePath(string relativePath) => Path.GetFullPath(relativePath);
    }

    internal static class Logger
    {
        internal static void Log(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }   
}
