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
		internal static string srmlLogFile = Path.Combine(Application.persistentDataPath, "SRML/srml.log");

		/// <summary>
		/// Logs a info message
		/// </summary>
		/// <param name="message">Message to log</param>
		public static void Log(string message)
		{
			LogEntry(LogType.Log, message);
		}

		/// <summary>
		/// Logs a warning message
		/// </summary>
		/// <param name="message">Message to log</param>
		public static void LogWarning(string message)
		{
			LogEntry(LogType.Warning, message);
		}

		/// <summary>
		/// Logs an error message
		/// </summary>
		/// <param name="message">Message to log</param>
		public static void LogError(string message)
		{
			LogEntry(LogType.Error, message);
		}

		private static string TypeToText(LogType logType)
		{
			if (logType == LogType.Error || logType == LogType.Exception)
				return "ERRO";

			return logType == LogType.Warning ? "WARN" : "INFO";
		}

		internal static void LogEntry(LogType logType, string message)
		{
			using (StreamWriter writer = File.AppendText(srmlLogFile))
				writer.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{TypeToText(logType)}] {Regex.Replace(message, @"\<[a-z=]+\>|\<\/[a-z]+\>", "")}");
		}
	}
}
