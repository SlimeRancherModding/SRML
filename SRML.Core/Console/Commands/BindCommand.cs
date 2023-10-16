using InControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRML.Console.Commands
{
    public class BindCommand : ConsoleCommand
    {
        public override string ID => "bind";

        public override string Usage => "bind <operation> <key> [command]";

        public override string Description => "binds a command to a key";

        public override bool Execute(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                Console.Instance.LogError("Wrong number of arguments (try putting the command you're binding in quotes)!");
                return false;
            }

            Key key;

            try
            {
                key = (Key)Enum.Parse(typeof(Key), args[1], true);
            }
            catch
            {
                Console.Instance.LogError("Invalid key!");
                return false;
            }

            switch (args[0])
            {
                case "add":
                    if (args.Length != 3)
                    {
                        Console.Instance.LogError("Wrong number of arguments (try putting the command you're binding in quotes)!");
                        return false;
                    }
                    KeyBindManager.CreateBinding(args[2], key);
                    break;
                case "remove":
                    KeyBindManager.RemoveBinding(key);
                    break;
                default:
                    Console.Instance.LogError("Invalid operation specified!");
                    return false;
            }
            KeyBindManager.SaveBinds();

            return true;
        }

        private static readonly List<string> OPERATIONS = new List<string>()
        {
            "add",
            "remove"
        };

        public override List<string> GetAutoComplete(int argIndex, string argText)
        {
            if (argIndex == 0) return OPERATIONS;
            else if (argIndex == 1) return Enum.GetNames(typeof(Key)).ToList();
            return base.GetAutoComplete(argIndex, argText);
        }
    }
}
