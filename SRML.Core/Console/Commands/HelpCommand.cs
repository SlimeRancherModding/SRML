using System;
using System.Collections.Generic;
using SRML.Console;

namespace SRML.Console.Commands
{
	/// <summary>
	/// A command to display all commands
	/// </summary>
	public class HelpCommand : ConsoleCommand
	{
		public override string ID { get; } = "help";
		public override string Usage { get; } = "help [cmdName]";
		public override string Description { get; } = "Displays all commands available, or an extended description of a command";

		public override string ExtendedDescription => 
			"<color=#77DDFF>[cmdName]</color> - The command you want to check the extended description for.\n" +
			"Running without an argument shows all commands";

		public override bool Execute(string[] args)
		{
			if (args != null && ArgsOutOfBounds(args.Length, 1, 1))
				return false;

			if (args == null)
			{
				Console.Instance.Log("<color=cyan>List of Commands Available:</color>");
				Console.Instance.Log("<i><> is a required argument; [] is an optional argument</i>");

				foreach (string line in ConsoleWindow.cmdsText.Split('\n'))
					Console.Instance.LogSuccess(line);
			}
			else
			{
				if (Console.commands.ContainsKey(args[0]))
				{
					ConsoleCommand cmd = Console.commands[args[0]];

					if (cmd.ExtendedDescription != null)
					{
						Console.Instance.Log($"<color=#77DDFF>{cmd.Usage}</color> - {cmd.Description}");
						foreach (string line in cmd.ExtendedDescription.Split('\n'))
							Console.Instance.LogSuccess(line);
					}
					else
					{
						Console.Instance.LogWarning($"No extended description was found for command '<color=#77DDFF>{cmd.ID}</color>'. Showing default description");
						Console.Instance.LogSuccess($"<color=#77DDFF>{Console.ColorUsage(cmd.Usage)}</color> - {cmd.Description}");
					}
				}
				else
				{
					Console.Instance.LogError($"Command '<color=white>{args[0]}</color>' not found");
				}
			}

			return true;
		}

		public override List<string> GetAutoComplete(int argIndex, string argText)
		{
			if (argIndex == 0)
				return new List<string>(Console.commands.Keys);
			else
				return base.GetAutoComplete(argIndex, argText);
		}
	}
}
