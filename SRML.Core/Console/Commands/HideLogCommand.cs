using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRML.Console.Commands
{
    public class HideLogCommand : ConsoleCommand
    {
        public override string ID => "logger";

        public override string Usage => "logger <mod> <id> <action>";

        public override string Description => "Shows or hides a logger.";

        public override bool Execute(string[] args)
        {
            if (ArgsOutOfBounds(3, 3))
                return false;

            if (!Console.instancesForMod.ContainsKey(args[0]))
            {
                Console.LogWarning("Invalid mod id specified!");
                return false;
            }

            bool active;
            if (args[2] == "show")
                active = true;
            else if (args[2] == "hide")
                active = false;
            else
            {
                Console.LogWarning("Invalid operation!");
                return false;
            }

            if (args[1] == "all")
            {
                foreach (Console.ConsoleInstance inst in Console.instancesForMod[args[0]])
                    inst.SetActive(active);
            }
            else
            {
                Console.ConsoleInstance inst = Console.instancesForMod[args[0]].FirstOrDefault(x => x.id == $"{args[0]}.{args[1]}");
                if (inst == null)
                {
                    Console.LogWarning($"Logger id '{args[1]}' not found in mod '{args[0]}'!");
                    return false;
                }

                inst.SetActive(active);
            }

            return true;
        }

        internal static readonly List<string> OPERATIONS = new List<string>()
        {
            "show",
            "hide"
        };

        internal static readonly List<string> ALL = new List<string>()
        {
            "all"
        };

        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            switch (argIndex)
            {
                case 0:
                    return Console.instancesForMod.Keys.ToList();
                case 1:
                    if (Console.instancesForMod.TryGetValue(args[0], out var insts))
                        return ALL.Concat(insts.Select(x => x.id.Split('.').Last())).ToList();
                    break;
                case 2:
                    return OPERATIONS;
            }

            return base.GetAutoComplete(argIndex, args);
        }
    }
}
