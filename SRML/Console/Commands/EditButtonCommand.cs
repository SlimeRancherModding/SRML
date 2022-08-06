using System;
using System.Collections.Generic;
using SRML.Console;

namespace SRML.Console.Commands
{
	/// <summary>
	/// A command to edit a user defined button
	/// </summary>
	public class EditButtonCommand : ConsoleCommand
	{
		public override string ID { get; } = "editbutton";
		public override string Usage { get; } = "editbutton <id> <text> <command>";
		public override string Description { get; } = "Edits a user defined button";

		public override string ExtendedDescription => 
			"<color=#77DDFF><id></color> - The id of the button being edited\n" +
			"<color=#77DDFF><text></color> - The text to replace the old one. '<color=#77DDFF>.</color>' to keep the value\n" +
			"<color=#77DDFF><command></color> - The command to replace the old one. '<color=#77DDFF>.</color>' to keep the value";

		public override bool Execute(string[] args)
		{
			if (args == null)
			{
				Console.Instance.LogError($"The '<color=white>{ID}</color>' command takes 2 arguments");
				return false;
			}

			if (ArgsOutOfBounds(args.Length, 3, 3))
				return false;

			if (args[0].Contains(" "))
			{
				Console.Instance.LogError($"The '<color=white><id></color>' argument cannot contain any spaces");
				return false;
			}

			ConsoleBinder.RemoveBind(args[0]);
			ConsoleBinder.RegisterBind(args[0], args[1].Equals(".") ? Console.cmdButtons["user." + args[0]].Text : args[1], args[2].Equals(".") ? Console.cmdButtons["user." + args[0]].Command : args[2]);
			Console.Instance.LogSuccess($"Edited user defined button '<color=white>{args[0]}</color>'. Text changed to '<color=white>{args[1]}</color>' and command changed to '<color=#8ab7ff>{args[2]}</color>'");

			return true;
		}

		public override List<string> GetAutoComplete(int argIndex, string argText)
		{
			if (argIndex == 0)
				return ConsoleBinder.GetAllBinds();
			else if (argIndex == 2)
				return new List<string>(Console.commands.Keys);
			else
				return base.GetAutoComplete(argIndex, argText);
		}
	}
}
