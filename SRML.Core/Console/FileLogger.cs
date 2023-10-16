using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;

namespace SRML
{
    /// <summary>
    /// The logger for the log file
    /// </summary>
    public static class FileLogger
    {
        // THE LOG FILE
        internal static string srmlLogFile = Path.Combine(Main.StorageProvider.SavePath(), "SRML/srml.log");

        /// <summary>
        /// Initializes the file logger (run this before Console.Init)
        /// </summary>
        internal static void Init()
        {
            if (!Directory.Exists(Path.GetDirectoryName(srmlLogFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(srmlLogFile));

            if (File.Exists(srmlLogFile))
                File.Delete(srmlLogFile);

            File.Create(srmlLogFile).Close();
        }

        /// <summary>
        /// Logs a info message
        /// </summary>
        /// <param name="message">Message to log</param>
        [Obsolete("Use ConsoleInstance.LogToFile instead!")]
        public static void Log(string message)
        {
            LogEntry(LogType.Log, message, Console.Console.GetLogName());
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="message">Message to log</param>
        [Obsolete("Use ConsoleInstance.LogErrorToFile instead!")]
        public static void LogWarning(string message)
        {
            LogEntry(LogType.Warning, message, Console.Console.GetLogName());
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">Message to log</param>
        [Obsolete("Use ConsoleInstance.LogErrorToFile instead!")]
        public static void LogError(string message)
        {
            LogEntry(LogType.Error, message, Console.Console.GetLogName());
        }

        private static string TypeToText(LogType logType)
        {
            if (logType == LogType.Error || logType == LogType.Exception)
                return "ERRO";

            return logType == LogType.Warning ? "WARN" : "INFO";
        }

        internal static void LogEntry(LogType logType, string message, string name)
        {
            using (StreamWriter writer = File.AppendText(srmlLogFile))
                writer.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] [{name}] [{TypeToText(logType)}] {Regex.Replace(message, @"<material[^>]*>|<\/material>|<size[^>]*>|<\/size>|<quad[^>]*>|<b>|<\/b>|<color=[^>]*>|<\/color>|<i>|<\/i>", "")}");
        }
    }
}
