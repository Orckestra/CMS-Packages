using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using LogViewer.C1LogService;

namespace LogViewer
{
    public static class LogFile
    {
        const char NonBreakingSpace = (char)160;

        public static void Save(string filePath, LogEntry[] logEntries)
        {
            using(var stream = File.Open(filePath, FileMode.Create))
            {
                WriteUTF8EncodingHeader(stream);
            
                using(var sw = new StreamWriter(stream))
                {
                    foreach(LogEntry logEntry in logEntries)
                    {
                        sw.WriteLine(ToString(logEntry));
                    }
                }
            }
        }

        public static LogEntry[] Load(string filePath)
        {
            var result = new List<LogEntry>();
            foreach(var logEntry in GetLogEntries(filePath))
            {
                result.Add(logEntry);
            }

            return result.ToArray();
        }

        private static IEnumerable<LogEntry> GetLogEntries(string filePath)
        {
            // Copied from Composite.StandardPlugins.Logging.LogTraceListeners.FileLogTraceListener.LogEntry.Parse(...)
            LogEntry previousEntry = null;
            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();

                    LogEntry entry = ParseLogEntry(line);
                    if (entry != null)
                    {
                        if (previousEntry != null)
                        {
                            yield return previousEntry;
                        }
                        previousEntry = entry;
                    }
                    else
                    {
                        if (previousEntry != null)
                        {
                            previousEntry.Message += "\n" + line;
                        }
                    }
                }
            }
            if (previousEntry != null)
            {
                yield return previousEntry;
            }
        }

        public static LogEntry ParseLogEntry(string serializedLogEntry)
        {
            string[] parts = serializedLogEntry.Split((char)65533);

            if (parts.Length != 8)
            {
                parts = serializedLogEntry.Split((char)160);
            }

            if (parts.Length != 8)
            {
                return null;
            }

            var result = new LogEntry();

            try
            {
                string date = parts[0] + parts[1];

                result.TimeStamp = DateTime.ParseExact(date, "yyyyMMddhh:mm:ss.ffff",
                                                       CultureInfo.InvariantCulture.DateTimeFormat);
                result.ApplicationDomainId = int.Parse(parts[2]);
                result.ThreadId = int.Parse(parts[3]);
                result.Severity = parts[4];
                result.Title = parts[5];
                result.DisplayOptions = parts[6];
                result.Message = parts[7];
            }
            catch (Exception)
            {
                return null;
            }

            return result;
        }

        private static string ToString(LogEntry logEntry)
        {
            // Copied from Composite.Logging.WCF.LogEntry.ToString()
            var sb = new StringBuilder();
            sb.Append(logEntry.TimeStamp.ToString("yyyyMMdd"));
            sb.Append(NonBreakingSpace).Append(logEntry.TimeStamp.ToString("hh:mm:ss.ffff"));
            sb.Append(NonBreakingSpace).Append(logEntry.ApplicationDomainId);
            sb.Append(NonBreakingSpace).Append(logEntry.ThreadId < 10 ? " " : string.Empty).Append(logEntry.ThreadId);
            sb.Append(NonBreakingSpace).Append(logEntry.Severity);
            sb.Append(NonBreakingSpace).Append(logEntry.Title);
            sb.Append(NonBreakingSpace).Append(logEntry.DisplayOptions);
            sb.Append(NonBreakingSpace).Append(logEntry.Message/*.Replace("\n", @"\n")*/);

            return sb.ToString();
        }

        private static void WriteUTF8EncodingHeader(Stream stream)
        {
            byte[] preamble = Encoding.UTF8.GetPreamble();
            stream.Write(preamble, 0, preamble.Length);
        }
    }
}
