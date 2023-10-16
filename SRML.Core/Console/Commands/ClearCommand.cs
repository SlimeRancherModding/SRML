using System;
using SRML.Console;

namespace SRML.Console.Commands
{
	/// <summary>
	/// A command to clear the console
	/// </summary>
	public class ClearCommand : ConsoleCommand
	{
		public override string ID { get; } = "clear";
		public override string Usage { get; } = "clear";
		public override string Description { get; } = "Clears the console";

		public override bool Execute(string[] args)
		{
			if (args != null)
			{
				Console.Instance.LogError($"The '<color=white>{ID}</color>' command takes no arguments");
				return false;
			}

            Console.lines.Clear();
			return true;
		}
	}
}
