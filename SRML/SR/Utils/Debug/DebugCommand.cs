using System.Collections.Generic;
using SRML.Console;

namespace SRML.SR.Utils.Debug
{
    public class DebugCommand : ConsoleCommand
    {
        public static bool DebugMode = false;

        public override string ID { get; } = "debug";
        public override string Usage { get; } = "debug <mode>";
        public override string Description { get; } = "Sets the debug mode to <mode>";

        public override bool Execute(string[] args)
        {
            if (args == null || ArgsOutOfBounds(args.Length, 1, 1))
                return false;

            DebugMode = bool.Parse(args[0]);
            Console.Console.Instance.LogSuccess($"Changed the debug mode to <color=white>{DebugMode}</color>");

            return true;
        }

        public override List<string> GetAutoComplete(int argIndex, string argText)
        {
            return new List<string>() { "true", "false" };
        }
    }
}
