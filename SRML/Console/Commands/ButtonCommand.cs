using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRML.Console.Commands
{
    public class ButtonCommand : ConsoleCommand
    {
        public override string ID => "button";

        public override string Usage => "button <operation> <id> [text] [command]";

        public override string Description => "Adds, modifies, or removes a user-defined button in the command menu.";

        public override string ExtendedDescription =>
            "<color=#77DDFF><operation></color> - Whether to add, remove, or modify a button.\n" +
            "<color=#77DDFF><id></color> - The id of the button. '<color=#77DDFF>all</color>' is not a valid id\n" +
            "<color=#77DDFF>[text]</color> - The text to display on the button\n" +
            "<color=#77DDFF>[command]</color> - The command the button will execute";

        public override bool Execute(string[] args)
        {
            if (args == null)
            {
                Console.Instance.LogError($"The '<color=white>{ID}</color>' command takes at least 2 arguments");
                return false;
            }
            if (args[1].Contains(" "))
            {
                Console.Instance.LogError($"The '<color=white><id></color>' argument cannot contain any spaces");
                return false;
            }

            switch (args[0])
            {
                case "add":
                    if (ArgsOutOfBounds(args.Length, 4, 4))
                        return false;

                    if (args[1].Equals("all"))
                    {
                        Console.Instance.LogWarning($"Trying to register user defined button with id '<color=white>all</color>' but '<color=white>all</color>' is not a valid id!");
                        return false;
                    }

                    if (Console.cmdButtons.ContainsKey("user." + args[1]))
                    {
                        Console.Instance.LogWarning($"Trying to register user defined button with id '<color=white>{args[1]}</color>' but the ID is already registered!");
                        return false;
                    }

                    ConsoleBinder.RegisterBind(args[1], args[2], args[3]);
                    Console.Instance.LogSuccess($"Added new user defined button '<color=white>{args[1]}</color>' with command '<color=white>{args[3]}</color>'");

                    return true;
                case "remove":
                    if (ArgsOutOfBounds(args.Length, 2, 2))
                        return false;

                    if (!args[1].Equals("all"))
                    {
                        if (ConsoleBinder.RemoveBind(args[1]))
                        {
                            Console.Instance.LogSuccess($"Removed user defined button '<color=white>{args[1]}</color>'");
                            return true;
                        }
                    }
                    else
                    {
                        ConsoleBinder.RemoveAll();
                        Console.Instance.LogSuccess($"Removed all user defined buttons");
                        return true;
                    }

                    Console.Instance.LogError($"The user defined button '<color=white>{args[1]}</color>' was not found");
                    return false;
                case "edit":
                    if (ArgsOutOfBounds(args.Length, 4, 4))
                        return false;

                    if (Console.cmdButtons.ContainsKey("user." + args[1]))
                    {
                        string text = args[2].Equals(".") ? Console.cmdButtons["user." + args[1]].textSafe : args[2];
                        string command = args[3].Equals(".") ? Console.cmdButtons["user." + args[1]].Command : args[3];

                        ConsoleBinder.RemoveBind(args[1]);
                        ConsoleBinder.RegisterBind(args[1], text, command);

                        Console.Instance.LogSuccess($"Edited user defined button '<color=white>{args[1]}</color>'. Text changed to '<color=white>{text}</color>' and command changed to '<color=#8ab7ff>{command}</color>'");
                        return true;
                    }

                    Console.Instance.LogError($"The user defined button '<color=white>{args[1]}</color>' was not found");
                    return false;
                default:
                    Console.Instance.LogError("Invalid '<color=white><operation></color>' argument");
                    return false;
            }
        }

        private static readonly List<string> OPERATIONS = new List<string>()
        {
            "add",
            "remove",
            "edit"
        };

        public override List<string> GetAutoComplete(int argIndex, string[] args)
        {
            if (argIndex == 0)
                return OPERATIONS;
            
            switch (args[0])
            {
                case "add":
                    if (argIndex == 3)
                        return new List<string>(Console.commands.Keys);
                    break;
                case "remove":
                    if (argIndex == 1)
                        return ConsoleBinder.GetAllBinds();
                    break;
                case "edit":
                    if (argIndex == 1)
                        return ConsoleBinder.GetAllBinds();
                    else if (argIndex == 3)
                        return new List<string>(Console.commands.Keys);
                    break;
            }

            return base.GetAutoComplete(argIndex, args);
        }
    }
}
