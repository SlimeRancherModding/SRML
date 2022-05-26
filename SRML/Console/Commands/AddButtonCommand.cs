using System;
using System.Collections.Generic;
using SRML.Console;

namespace SRML.Console.Commands
{
	/// <summary>
	/// A command to add a user defined button to the command menu
	/// </summary>
	public class AddButtonCommand : ConsoleCommand
	{
		public override string ID { get; } = "addbutton";
		public override string Usage { get; } = "addbutton <id> <text> <command>";
		public override string Description { get; } = "Adds a user defined button to the command menu";

		public override string ExtendedDescription => 
			"<color=#77DDFF><id></color> - The id of the button. '<color=#77DDFF>all</color>' is not a valid id\n" +
			"<color=#77DDFF><text></color> - The text to display on the button\n" +
			"<color=#77DDFF><command></color> - The command the button will execute";

		public override bool Execute(string[] args)
		{
			if (args == null)
			{
				Console.Instance.LogError($"The '<color=white>{ID}</color>' command takes 3 arguments");
				return false;
			}

			if (ArgsOutOfBounds(args.Length, 3, 3))
				return false;

			if (args[0].Contains(" "))
			{
				Console.Instance.LogError($"The '<color=white><id></color>' argument cannot contain any spaces");
				return false;
			}

			if (args[0].Equals("all"))
			{
				Console.Instance.LogWarning($"Trying to register user defined button with id '<color=white>all</color>' but '<color=white>all</color>' is not a valid id!");
				return false;
			}

			if (Console.cmdButtons.ContainsKey("user." + args[0]))
			{
				Console.Instance.LogWarning($"Trying to register user defined button with id '<color=white>{args[0]}</color>' but the ID is already registered!");
				return false;
			}

			ConsoleBinder.RegisterBind(args[0], args[1], args[2]);
			Console.Instance.LogSuccess($"Added new user defined button '<color=white>{args[0]}</color>' with command '<color=white>{args[2]}</color>'");

			return true;
		}

		public override List<string> GetAutoComplete(int argIndex, string argText)
		{
			if (argIndex == 2)
				return new List<string>(Console.commands.Keys);
			else
				return base.GetAutoComplete(argIndex, argText);
		}
	}
}
