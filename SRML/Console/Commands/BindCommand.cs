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

        public override string Usage => "bind [key] [command]";

        public override string Description => "binds a command to a key";

        public override bool Execute(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Instance.LogError("Wrong number of arguments (try putting the command you're binding in quotes)!");
                return false;
            }

            Key key;

            try
            {
                key = (Key)Enum.Parse(typeof(Key), args[0], true);
            }
            catch
            {

                Console.Instance.LogError("Invalid key!");
                return false ;
            }

            KeyBindManager.CreateBinding(args[1], key);
            KeyBindManager.SaveBinds();
            return true;
        }

        public override List<string> GetAutoComplete(int argIndex, string argText)
        {
            if (argIndex == 0) return Enum.GetNames(typeof(Key)).ToList();
            return base.GetAutoComplete(argIndex, argText);
        }
    }
}
