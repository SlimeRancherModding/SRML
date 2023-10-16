using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Text.RegularExpressions;
using SRML.SR.Utils.Debug;
using System.Linq;
using System.Reflection;

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
        internal static string unityLogFile = Path.Combine(Main.StorageProvider.SavePath(), "Player.log");
        internal static string srmlLogFile = Path.Combine(Main.StorageProvider.SavePath(), "SRML/srml.log");
        internal static string logHideFile = Path.Combine(FileSystem.GetConfigPath(null), "hiddeninstances");
        internal static readonly Console console = new Console();
        private static ConsoleInstance srmlInstance;
        private static ConsoleInstance unityInstance;
        public static ConsoleInstance Instance { get { return srmlInstance; } }

        // COMMAND STUFF
        internal static Dictionary<string, ConsoleCommand> commands = new Dictionary<string, ConsoleCommand>();
        internal static Dictionary<string, ConsoleButton> cmdButtons = new Dictionary<string, ConsoleButton>();
        internal static List<string> hideLogs = new List<string>();
        internal static Dictionary<string, List<ConsoleInstance>> instancesForMod = new Dictionary<string, List<ConsoleInstance>>();

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
            srmlInstance = new ConsoleInstance("SRML", Colors.lightblue, "internal.srml");
            unityInstance = new ConsoleInstance("Unity", Colors.yellow, "internal.unity");

            Instance.Log("CONSOLE INITIALIZED!");
            Instance.Log("Patching SceneManager to attach window");

            RegisterCommand(new Commands.ClearCommand());
            RegisterCommand(new Commands.HelpCommand());
            RegisterCommand(new Commands.ReloadCommand());
            RegisterCommand(new Commands.ModsCommand());
            RegisterCommand(new Commands.HideLogCommand());
            RegisterCommand(new Commands.DumpCommand());
            RegisterCommand(new Commands.ButtonCommand());
            RegisterCommand(new Commands.BindCommand());
            RegisterCommand(new Commands.SpawnCommand());
            RegisterCommand(new Commands.GiveCommand());
            RegisterCommand(new Commands.ConfigCommand());
            RegisterCommand(new Commands.KillAllCommand());
            RegisterCommand(new Commands.KillCommand());
            RegisterCommand(new Commands.NoclipCommand());
            RegisterCommand(new Commands.FastForwardCommand());
            RegisterCommand(new DebugCommand());

            RegisterButton("clear", new ConsoleButton("Clear Console", "clear"));
            RegisterButton("help", new ConsoleButton("Show Help", "help"));
            RegisterButton("mods", new ConsoleButton("Show Mods", "mods"));
            RegisterButton("reload", new ConsoleButton("Run Reload", "reload"));
            RegisterButton("dump.all", new ConsoleButton("Dump All Files", "dump all"));

            ConsoleBinder.ReadBinds();
            ReadLogHiders();
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
                Instance.LogWarning($"Trying to register command with id '<color=white>{cmd.ID.ToLowerInvariant()}</color>' but the ID is already registered!");
                return false;
            }

            // TODO: Update to new system
            //cmd.belongingMod = SRMod.GetCurrentMod();
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
                Instance.LogWarning($"Trying to register command button with id '<color=white>all</color>' but '<color=white>all</color>' is not a valid id!");
                return false;
            }

            if (cmdButtons.ContainsKey(id))
            {
                Instance.LogWarning($"Trying to register command button with id '<color=white>{id}</color>' but the ID is already registered!");
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
                Instance.LogWarning($"Trying to register dump action with id '<color=white>{id.Replace(" ", string.Empty)}</color>' but the ID is already registered!");
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

        public static bool RegisterLogHider(string id)
        {
            string[] parts = id.Split('.');
            if (parts.Length != 2)
            {
                LogWarning($"Trying to supress {id}, which is an invalid id.");
                return false;
            }
            if (!instancesForMod.ContainsKey(parts[0]))
            {
                LogWarning($"Trying to get invalid mod id '{parts[0]}'");
                return false;
            }
            if (!instancesForMod[parts[0]].TryGetValue(x => x.id == parts[1], out ConsoleInstance inst))
            {
                LogWarning($"Trying to get ConsoleInstance '{parts[1]}' from '{parts[0]}', but no such instance exists");
                return false;
            }

            hideLogs.Add(id);
            File.AppendAllText(logHideFile, $"{id}\n");
            inst.SetActive(false);
            return true;
        }

        internal static void ReadLogHiders()
        {
            if (!File.Exists(logHideFile))
                return;

            IEnumerable<ConsoleInstance> instances = instancesForMod.Values.SelectMany(x => x);

            foreach (string line in File.ReadAllLines(logHideFile))
            {
                if (!line.Contains("."))
                    return;

                instances.FirstOrDefault(x => x.id == line)?.SetActive(false);

                hideLogs.Add(line);
            }
        }

        /// <summary>
        /// Logs a info message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="logToFile">Should log to file?</param>
        public static void Log(string message, bool logToFile = true) => srmlInstance.Log(message, logToFile);

        /// <summary>
        /// Logs a success message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="logToFile">Should log to file?</param>
        public static void LogSuccess(string message, bool logToFile = true) => srmlInstance.LogSuccess(message, logToFile);

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="logToFile">Should log to file?</param>
        public static void LogWarning(string message, bool logToFile = true) => srmlInstance.LogWarning(message, logToFile);

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="logToFile">Should log to file?</param>
        public static void LogError(string message, bool logToFile = true) => srmlInstance.LogError(message, logToFile);


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
                Instance.Log("<color=cyan>Command: </color>" + command);

                string[] cmds = command.Split(';');
                foreach (string c in cmds)
                {
                    bool spaces = c.Contains(" ");
                    string cmd = spaces ? c.Substring(0, c.IndexOf(' ')) : c;

                    if (commands.ContainsKey(cmd))
                    {
                        bool executed = false;
                        bool keepExecution = true;
                        string[] args = spaces ? StripArgs(c) : null;

                        foreach (CommandCatcher catcher in catchers)
                        {
                            keepExecution = catcher.Invoke(cmd, args);

                            if (!keepExecution)
                                break;
                        }

                        if (keepExecution)
                        {
                            // TODO: Upgrade to new system
                            //SRMod.ForceModContext(commands[cmd].belongingMod);
                            try
                            {
                                executed = commands[cmd].Execute(args);
                            }
                            finally
                            {
                                //SRMod.ClearModContext();
                            }
                        }

                        if (!executed && keepExecution)
                            Instance.Log($"<color=cyan>Usage:</color> <color=#77DDFF>{ColorUsage(commands[cmd].Usage)}</color>");
                    }
                    else
                    {
                        Instance.LogError("Unknown command. Please use '<color=white>help</color>' for available commands or check the menu on the right");
                    }
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

        internal static string GetLogName()
        {
            // TODO: Upgrade to new system
            SRMod mod = null/*SRMod.GetCurrentMod()*/;
            if (mod != null) return mod.ModInfo.Name;
            return "SRML";
        }

        // LOGS A NEW ENTRY
        internal void LogEntry(LogType logType, string message, bool logToFile, string name, Colors nameCol)
        {
            string type = TypeToText(logType);
            string color = "white";
            if (type.Equals("ERRO")) color = "#FFAAAA";
            if (type.Equals("WARN")) color = "#EEEE99";

            if (lines.Count >= MAX_ENTRIES)
                lines.RemoveRange(0, 10);
            
            lines.Add($"<color=cyan>[{DateTime.Now.ToString("HH:mm:ss")}]</color> <color={nameCol}>[{name}]</color> <color={color}>[{type}] {Regex.Replace(message, @"<material[^>]*>|<\/material>|<size[^>]*>|<\/size>|<quad[^>]*>|<b>|<\/b>", "")}</color>");

            if (logToFile)
                FileLogger.LogEntry(logType, message, name);

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
            toDisplay = Regex.Replace(toDisplay, @"\[INFO]\s|\[ERROR]\s|\[WARNING]\s", "");

            switch (type)
            {
                case LogType.Error:
                    unityInstance.LogError(toDisplay);
                    break;
                case LogType.Exception:
                    unityInstance.LogError(toDisplay);
                    break;
                case LogType.Log:
                    unityInstance.Log(toDisplay);
                    break;
                case LogType.Assert:
                    unityInstance.Log(toDisplay);
                    break;
                case LogType.Warning:
                    unityInstance.LogWarning(toDisplay);
                    break;
            }
        }

        public class ConsoleInstance
        {
            public readonly string Name;
            internal Colors col = Colors.lime;
            internal string id;
            internal bool enabled = true;

            public static ConsoleInstance PopulateConsoleInstanceFromType(Type entryType, string modId)
            {
                Colors nameCol = Colors.lime;
                string text = null;
                foreach (Attribute customAttribute in entryType.GetCustomAttributes())
                {
                    if (customAttribute.GetType() == typeof(ConsoleAppearance))
                    {
                        ConsoleAppearance consoleAppearance = (ConsoleAppearance)customAttribute;
                        nameCol = consoleAppearance.consoleCol;
                        text = consoleAppearance.name;
                    }
                }

                return new ConsoleInstance(text ?? modId, nameCol, modId + ".main");
            }

            public void Log(object message, bool logToFile = true)
            {
                if (enabled)
                    console.LogEntry(LogType.Log, message.ToString(), logToFile, Name, col);
            }

            public void LogWarning(object message, bool logToFile = true)
            {
                if (enabled)
                    console.LogEntry(LogType.Warning, message.ToString(), logToFile, Name, col);
            }

            public void LogError(object message, bool logToFile = true)
            {
                if (enabled)
                    console.LogEntry(LogType.Error, message.ToString(), logToFile, Name, col);
            }

            public void LogSuccess(object message, bool logToFile = true)
            {
                if (enabled)
                    console.LogEntry(LogType.Log, $"<color=#AAFF99>{message}</color>", logToFile, Name, col);
            }

            public void LogToFile(object message)
            {
                if (enabled)
                    FileLogger.LogEntry(LogType.Log, message.ToString(), Name);
            }

            public void LogWarningToFile(object message)
            {
                if (enabled)
                    FileLogger.LogEntry(LogType.Warning, message.ToString(), Name);
            }

            public void LogErrorToFile(object message)
            {
                if (enabled)
                    FileLogger.LogEntry(LogType.Error, message.ToString(), Name);
            }

            public void SetActive(bool active) => enabled = active;

            public ConsoleInstance(string name) : this(name, Colors.lime) { }

            // TODO: Upgrade to new system
            public ConsoleInstance(string name, Colors nameCol) : this(name, nameCol, $"{/*SRMod.GetCurrentMod()?.ModInfo.Id ?? */"unknown"}.{name.ToLower().Replace(' ', '_')}") { }

            internal ConsoleInstance(string name, Colors nameCol, string id)
            {
                Name = name;
                col = nameCol;
                this.id = id;

                string modId = id.Split('.')[0];

                if (!instancesForMod.ContainsKey(modId))
                    instancesForMod.Add(modId, new List<ConsoleInstance>());
                instancesForMod[modId].Add(this);

                if (SRModLoader.CurrentLoadingStep == SRModLoader.LoadingStep.INITIALIZATION)
                    return;

                SetActive(!hideLogs.Contains(id));
            }
        }
    }
}
