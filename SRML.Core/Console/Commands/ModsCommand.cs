using System;
using SRML.Console;

namespace SRML.Console.Commands
{
	/// <summary>
	/// A command to display all mods
	/// </summary>
	public class ModsCommand : ConsoleCommand
	{
		public override string ID { get; } = "mods";
		public override string Usage { get; } = "mods";
		public override string Description { get; } = "Displays all mods loaded";

		public override bool Execute(string[] args)
		{
			if (args != null)
			{
				Console.Instance.LogError($"The '<color=white>{ID}</color>' command takes no arguments");
				return false;
			}

			Console.Instance.Log("<color=cyan>List of Mods Loaded:</color>");

			foreach (string line in ConsoleWindow.modsText.Split('\n'))
				Console.Instance.Log(line);

			return true;
		}
	}
}
