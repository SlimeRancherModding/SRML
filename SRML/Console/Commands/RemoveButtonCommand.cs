using System;
using System.Collections.Generic;
using SRML.Console;

namespace SRML.Console.Commands
{
	/// <summary>
	/// A command to remove a user defined button from the command menu
	/// </summary>
	public class RemoveButtonCommand : ConsoleCommand
	{
		public override string ID { get; } = "removebutton";
		public override string Usage { get; } = "removebutton <id>";
		public override string Description { get; } = "Removes a user defined button from the command menu";

		public override string ExtendedDescription => "<color=#77DDFF><id></color> - The id of the button to remove. '<color=#77DDFF>all</color>' will remove all buttons";

		public override bool Execute(string[] args)
		{
			if (args == null)
			{
				Console.LogError($"The '<color=white>{ID}</color>' command takes 1 argument");
				return false;
			}

			if (ArgsOutOfBounds(args.Length, 1, 1))
				return false;

			if (args[0].Contains(" "))
			{
				Console.LogError($"The '<color=white><id></color>' argument cannot contain any spaces");
				return false;
			}

			if (!args[0].Equals("all"))
			{
				if (ConsoleBinder.RemoveBind(args[0]))
				{
					Console.LogSuccess($"Removed user defined button '<color=white>{args[0]}</color>'");
					return true;
				}
			}
			else
			{
				ConsoleBinder.RemoveAll();
				Console.LogSuccess($"Removed all user defined buttons");
				return true;
			}

			Console.LogError($"The user defined button '<color=white>{args[0]}</color>' was not found");
			return false;
		}

		public override List<string> GetAutoComplete(int argIndex, string argText)
		{
			if (argIndex == 0)
				return ConsoleBinder.GetAllBinds();
			else
				return base.GetAutoComplete(argIndex, argText);
		}
	}
}
