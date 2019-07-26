using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Text.RegularExpressions;

namespace SRML.Console
{
    /// <summary>
    /// Controls the in-game console
    /// </summary>
    public class Console
    {
        // CONFIGURE SOME NUMBERS
        public const int MAX_ENTRIES = 100; // MAX ENTRIES TO SHOW ON CONSOLE (CAN'T GO ABOVE 100, TEXT MESH GENERATOR WILL BUG IF SO)
        public const int HISTORY = 10; // NUMBER OF COMMANDS TO KEEP ON HISTORY

        // LOG STUFF
        internal static string unityLogFile = Path.Combine(Application.persistentDataPath, "output_log.txt");
        internal static string srmlLogFile = Path.Combine(Application.persistentDataPath, "SRML/srml.log");
        private static readonly Console console = new Console();

        // COMMAND STUFF
        internal static Dictionary<string, ConsoleCommand> commands = new Dictionary<string, ConsoleCommand>();
        internal static Dictionary<string, ConsoleButton> cmdButtons = new Dictionary<string, ConsoleButton>();

        // LINES
        internal static List<string> lines = new List<string>();

        // COMMAND HISTORY
        internal static List<string> history = new List<string>(HISTORY);

        // RELOAD EVENT (THIS IS CALLED WHEN THE COMMAND RELOAD IS CALLED, USED TO RUN A RELOAD METHOD FOR A MOD, IF THE AUTHOR WISHES TO CREATE ONE)
        public delegate void ReloadAction(); // Creates the delegate here to prevent 'TypeNotFound' exceptions

        /// <summary>
        /// The event that triggers when the Reload Command is called
        /// </summary>
        public static event ReloadAction Reload;

        // DUMP ACTIONS
        // KEY = Dump Command Argument; VALUE = The method to run
        public delegate void DumpAction(StreamWriter writer);
        internal static Dictionary<string, DumpAction> dumpActions = new Dictionary<string, DumpAction>();

        // COMMAND CATCHER
        public delegate bool CommandCatcher(string cmd, string[] args);
        internal static List<CommandCatcher> catchers = new List<CommandCatcher>();

        /// <summary>
        /// Initializes the console
        /// </summary>
        internal static void Init()
        {
            Application.logMessageReceived += console.AppLog;

            Log("CONSOLE INITIALIZED!");
            Log("Patching SceneManager to attach window");

            RegisterCommand(new Commands.ClearCommand());
            RegisterCommand(new Commands.HelpCommand());
            RegisterCommand(new Commands.ReloadCommand());
            RegisterCommand(new Commands.ModsCommand());
            RegisterCommand(new Commands.DumpCommand());
            RegisterCommand(new Commands.AddButtonCommand());
            RegisterCommand(new Commands.RemoveButtonCommand());
            RegisterCommand(new Commands.EditButtonCommand());
            RegisterCommand(new Commands.SpawnCommand());
            RegisterCommand(new Commands.GiveCommand());
            RegisterCommand(new Commands.BindCommand());
            RegisterCommand(new Commands.ConfigCommand());

            RegisterButton("clear", new ConsoleButton("Clear Console", "clear"));
            RegisterButton("help", new ConsoleButton("Show Help", "help"));
            RegisterButton("mods", new ConsoleButton("Show Mods", "mods"));
            RegisterButton("reload", new ConsoleButton("Run Reload", "reload"));
            RegisterButton("dump.all", new ConsoleButton("Dump All Files", "dump all"));

            ConsoleBinder.ReadBinds();
            SceneManager.activeSceneChanged += ConsoleWindow.AttachWindow;
        }

        /// <summary>
        /// Registers a new command into the console
        /// </summary>
        /// <param name="cmd">Command to register</param>
        /// <returns>True if registered succesfully, false otherwise</returns>
        public static bool RegisterCommand(ConsoleCommand cmd)
        {
            if (commands.ContainsKey(cmd.ID.ToLowerInvariant()))
            {
                LogWarning($"Trying to register command with id '<color=white>{cmd.ID.ToLowerInvariant()}</color>' but the ID is already registered!");
                return false;
            }

            cmd.belongingMod = SRMod.GetCurrentMod();
            commands.Add(cmd.ID.ToLowerInvariant(), cmd);
            ConsoleWindow.cmdsText += $"{(ConsoleWindow.cmdsText.Equals(string.Empty) ? "" : "\n")}<color=#77DDFF>{ColorUsage(cmd.Usage)}</color> - {cmd.Description}";
            return true;
        }

        /// <summary>
        /// Registers a new console button
        /// </summary>
        /// <param name="id">The id of the button</param>
        /// <param name="button">Button to register</param>
        /// <returns>True if registered succesfully, false otherwise</returns>
        public static bool RegisterButton(string id, ConsoleButton button)
        {
            if (id.Equals("all"))
            {
                LogWarning($"Trying to register command button with id '<color=white>all</color>' but '<color=white>all</color>' is not a valid id!");
                return false;
            }

            if (cmdButtons.ContainsKey(id))
            {
                LogWarning($"Trying to register command button with id '<color=white>{id}</color>' but the ID is already registered!");
                return false;
            }

            cmdButtons.Add(id, button);
            return true;
        }

        /// <summary>
        /// Registers a new dump action for the dump command
        /// </summary>
        /// <param name="id">The id to use for the dump command argument</param>
        /// <param name="action">The dump action to run</param>
        /// <returns>True if registered succesfully, false otherwise</returns>
        public static bool RegisterDumpAction(string id, DumpAction action)
        {
            if (dumpActions.ContainsKey(id.Replace(" ", string.Empty)))
            {
                LogWarning($"Trying to register dump action with id '<color=white>{id.Replace(" ", string.Empty)}</color>' but the ID is already registered!");
                return false;
            }

            dumpActions.Add(id.Replace(" ", string.Empty), action);
            return true;
        }

        /// <summary>
        /// Registers a command catcher which allows commands to be processed and their execution controlled by outside methods
        /// </summary>
        /// <param name="catcher">The method to catch the commands</param>
        public static void RegisterCommandCatcher(CommandCatcher catcher)
        {
            catchers.Add(catcher);
        }

        /// <summary>
        /// Logs a info message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="logToFile">Should log to file?</param>
        public static void Log(string message, bool logToFile = true)
        {
            console.LogEntry(LogType.Log, message, logToFile);
        }

        /// <summary>
        /// Logs a success message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="logToFile">Should log to file?</param>
        public static void LogSuccess(string message, bool logToFile = true)
        {
            console.LogEntry(LogType.Log, $"<color=#AAFF99>{message}</color>", logToFile);
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="logToFile">Should log to file?</param>
        public static void LogWarning(string message, bool logToFile = true)
        {
            console.LogEntry(LogType.Warning, message, logToFile);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="logToFile">Should log to file?</param>
        public static void LogError(string message, bool logToFile = true)
        {
            console.LogEntry(LogType.Error, message, logToFile);
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

            try
            {
                Log("<color=cyan>Command: </color>" + command);

                bool spaces = command.Contains(" ");
                string cmd = spaces ? command.Substring(0, command.IndexOf(' ')) : command;

                if (commands.ContainsKey(cmd))
                {
                    bool executed = false;
                    bool keepExecution = true;
                    string[] args = spaces ? StripArgs(command) : null;

                    foreach (CommandCatcher catcher in catchers)
                    {
                        keepExecution = catcher.Invoke(cmd, args);

                        if (!keepExecution)
                            break;
                    }

                    if (keepExecution)
                    {
                        SRMod.ForceModContext(commands[cmd].belongingMod);
                        try
                        {
                            executed = commands[cmd].Execute(args);
                        }
                        finally
                        {
                            SRMod.ClearModContext();
                        }
                    }

                    if (!executed && keepExecution)
                        Console.Log($"<color=cyan>Usage:</color> <color=#77DDFF>{ColorUsage(commands[cmd].Usage)}</color>");
                }
                else
                {
                    LogError("Unknown command. Please use '<color=white>help</color>' for available commands or check the menu on the right");
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        internal static string[] StripArgs(string command, bool autoComplete = false)
        {
            MatchCollection result = Regex.Matches(command.Substring(command.IndexOf(' ') + 1), "[^'\"\\s\\n]+|'[^']+'?|\"[^\"]+\"?");
            List<string> args = new List<string>(result.Count);

            foreach (Match match in result)
                args.Add(autoComplete ? match.Value : Regex.Replace(match.Value, "'|\"", ""));

            if (autoComplete && command.EndsWith(" "))
                args.Add(string.Empty);

            return args.ToArray();
        }

        // CONVERTS LOG TYPE TO A SMALLER MORE READABLE TYPE
        private string TypeToText(LogType logType)
        {
            if (logType == LogType.Error || logType == LogType.Exception)
                return "ERRO";

            return logType == LogType.Warning ? "WARN" : "INFO";
        }

        // LOGS A NEW ENTRY
        private void LogEntry(LogType logType, string message, bool logToFile)
        {
            string type = TypeToText(logType);
            string color = "white";
            if (type.Equals("ERRO")) color = "#FFAAAA";
            if (type.Equals("WARN")) color = "#EEEE99";

            if (lines.Count >= MAX_ENTRIES)
                lines.RemoveRange(0, 10);

            lines.Add($"<color=cyan>[{DateTime.Now.ToString("HH:mm:ss")}]</color><color={color}>[{type}] {Regex.Replace(message, @"<material[^>]*>|<\/material>|<size[^>]*>|<\/size>|<quad[^>]*>|<b>|<\/b>", "")}</color>");

            if (logToFile)
                FileLogger.LogEntry(logType, message);

            ConsoleWindow.updateDisplay = true;
        }

        internal static string ColorUsage(string usage)
        {
            string result = string.Empty;
            MatchCollection matches = Regex.Matches(usage, @"[\w\d]+|\<[\w]+\>|\[[\w]+\]");

            foreach (Match match in matches)
            {
                if (match.Value.StartsWith("<") && match.Value.EndsWith(">"))
                {
                    result += $" <<color=white>{match.Value.Substring(1, match.Value.Length - 2)}</color>>";
                    continue;
                }

                if (match.Value.StartsWith("[") && match.Value.EndsWith("]"))
                {
                    result += $" <i>[<color=white>{match.Value.Substring(1, match.Value.Length - 2)}</color>]</i>";
                    continue;
                }

                result += " " + match.Value;
            }

            return result.TrimStart(' ');
        }

        // RUNS THE RELOAD COMMAND
        internal static void ReloadMods()
        {
            Reload?.Invoke();
        }

        private void AppLog(string message, string trace, LogType type)
        {
            if (message.Equals(string.Empty))
                return;

            string toDisplay = message;
            if (!trace.Equals(string.Empty))
                toDisplay += "\n" + trace;

            LogEntry(type, Regex.Replace(toDisplay, @"\[INFO]\s|\[ERROR]\s|\[WARNING]\s", ""), true);
        }
    }
}
