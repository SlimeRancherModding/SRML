using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Text.RegularExpressions;
using SRML.ConsoleSystem;

namespace SRML
{
	/// <summary>
	/// Controls the in-game console
	/// </summary>
	public class Console : ILogHandler
	{
		// CONFIGURE SOME NUMBERS
		public const int MAX_ENTRIES = 200; // MAX ENTRIES TO SHOW ON CONSOLE
		public const int HISTORY = 10; // NUMBER OF COMMANDS TO KEEP ON HISTORY

		// LOG STUFF
		internal static string unityLogFile = Path.Combine(Application.persistentDataPath, "output_log.txt");
		internal static string srmlLogFile = Path.Combine(Application.persistentDataPath, "SRML/srml.log");
		private static ILogHandler unityHandler = Debug.unityLogger.logHandler;
		private static Console console = new Console();

		// COMMAND STUFF
		private static Dictionary<string, ConsoleCommand> commands = new Dictionary<string, ConsoleCommand>();

		// LINE COUNTER
		private static int lines = 0;

		// COMMAND HISTORY
		internal static List<string> history = new List<string>(HISTORY);

		// RELOAD EVENT (THIS IS CALLED WHEN THE COMMAND RELOAD IS CALLED, USED TO RUN A RELOAD METHOD FOR A MOD, IF THE AUTHOR WISHES TO CREATE ONE)
		public delegate void ReloadAction(); // Creates the delegate here to prevent 'TypeNotFound' exceptions
		public static event ReloadAction Reload;

		/// <summary>
		/// Initializes the console
		/// </summary>
		public static void Init()
		{
			Debug.unityLogger.logHandler = console;

			if (!Directory.Exists(srmlLogFile.Substring(0, srmlLogFile.LastIndexOf('/'))))
				Directory.CreateDirectory(srmlLogFile.Substring(0, srmlLogFile.LastIndexOf('/')));

			if (File.Exists(srmlLogFile))
				File.Delete(srmlLogFile);

			File.Create(srmlLogFile).Close();

			Log("CONSOLE INITIALIZED!");
			Log("Patching SceneManager to attach window");

			RegisterCommand(new Commands.HelpCommand());
			RegisterCommand(new Commands.ReloadCommand());
			RegisterCommand(new Commands.ModsCommand());

			SceneManager.activeSceneChanged += ConsoleWindow.AttachWindow;
		}

		/// <summary>
		/// Registers a new command into the console
		/// </summary>
		/// <param name="cmd">Command to register</param>
		public static void RegisterCommand(ConsoleCommand cmd)
		{
			if (commands.ContainsKey(cmd.ID))
			{
				LogWarning($"Trying to register command with id '{cmd.ID}' but the ID is already registered!");
				return;
			}

			commands.Add(cmd.ID, cmd);
			ConsoleWindow.cmdsText += $"{(ConsoleWindow.cmdsText.Equals(string.Empty) ? "" : "\n")}<color=#8ab7ff>{cmd.Usage}</color> - {cmd.Description}";
		}

		/// <summary>
		/// Logs a info message
		/// </summary>
		/// <param name="message">Message to log</param>
		public static void Log(string message)
		{
			unityHandler.LogFormat(LogType.Log, null, message, string.Empty);
			console.LogEntry(LogType.Log, message);
		}

		/// <summary>
		/// Logs a warning message
		/// </summary>
		/// <param name="message">Message to log</param>
		public static void LogWarning(string message)
		{
			unityHandler.LogFormat(LogType.Warning, null, message, string.Empty);
			console.LogEntry(LogType.Warning, message);
		}

		/// <summary>
		/// Logs an error message
		/// </summary>
		/// <param name="message">Message to log</param>
		public static void LogError(string message)
		{
			unityHandler.LogFormat(LogType.Error, null, message, string.Empty);
			console.LogEntry(LogType.Error, message);
		}

		// PROCESSES THE TEXT FROM THE CONSOLE INPUT
		internal static void ProcessInput(string command, bool forced = false)
		{
			if (command.Equals(string.Empty))
				return;

			if (!forced)
			{
				if (history.Count == HISTORY)
					history.RemoveAt(0);

				history.Add(command);
			}

			Log("<color=cyan>CMD: </color>" + command);

			bool spaces = command.Contains(" ");
			string cmd = spaces ? command.Substring(0, command.IndexOf(' ')) : command;
			
			if (commands.ContainsKey(cmd))
			{
				commands[cmd].Execute(spaces ? StripArgs(command) : null);
			}
			else
			{
				LogError("Unknown command. Please use 'help' for available commands or check the menu on the right");
			}
		}

		private static string[] StripArgs(string command)
		{
			MatchCollection result = Regex.Matches(command.Substring(command.IndexOf(' ')+1), "\\w+|\'[^']+\'|\"[^\"]+\"");
			List<string> args = new List<string>(result.Count);

			foreach (Match match in result)
				args.Add(Regex.Replace(match.Value, "'|\"", ""));

			return args.ToArray();
		}

		// CONVERTS LOG TYPE TO A SMALLER MORE READABLE TYPE
		private string TypeToText(LogType logType)
		{
			if (logType == LogType.Error || logType == LogType.Exception)
				return "ERRO";

			if (logType == LogType.Warning)
				return "WARN";

			return "INFO";
		}

		// LOGS A NEW ENTRY
		private void LogEntry(LogType logType, string message)
		{
			string type = TypeToText(logType);
			string color = "white";
			if (type.Equals("ERRO")) color = "#ff7070";
			if (type.Equals("WARN")) color = "yellow";

			if (lines == MAX_ENTRIES)
				ConsoleWindow.fullText = ConsoleWindow.fullText.Substring(ConsoleWindow.fullText.IndexOf('\n'));
			else
				lines++;

			ConsoleWindow.fullText += $"{(ConsoleWindow.fullText.Equals(string.Empty) ? "" : "\n")}<color=cyan>[{DateTime.Now.ToString("HH:mm:ss")}]</color><color={color}>[{type}] {Regex.Replace(message, @"<material[^>]*>|<\/material>|<size[^>]*>|<\/size>|<quad[^>]*>|<b>|</b>", "")}</color>";

			using (StreamWriter writer = File.AppendText(srmlLogFile))
				writer.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{type}] {Regex.Replace(message, @"\<[a-z=]+\>|\<\/[a-z]+\>", "")}");

			ConsoleWindow.updateDisplay = true;
		}

		// RUNS THE RELOAD COMMAND
		internal static void ReloadMods()
		{
			Reload?.Invoke();
		}

		// THE TWO FOLLOWING METHODS CAN BE IGNORED, THEY TAP INTO UNITY'S SYSTEM TO LISTEN TO THE LOG
		void ILogHandler.LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
		{
			unityHandler.LogFormat(logType, context, format, args);
			LogEntry(logType, Regex.Replace(string.Format(format, args), @"\[INFO]\s|\[ERROR]\s|\[WARNING]\s", ""));
		}

		void ILogHandler.LogException(Exception exception, UnityEngine.Object context)
		{
			System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(exception, true);

			unityHandler.LogException(exception, context);
			LogEntry(LogType.Exception, exception.Message + "\n" + trace.ToString());
		}
	}
}
